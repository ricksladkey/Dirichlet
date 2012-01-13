using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class StructuredGaussianElimination<TArray, TMatrix> : INullSpaceAlgorithm<IBitArray, IBitMatrix>
        where TArray : IBitArray
        where TMatrix : IBitMatrix
    {
        private class Ancestor
        {
            public int Column { get; set; }
            public Ancestor Next { get; set; }
        }

        private const int multiThreadedCutoff = 256;
        private const int mergeLimit = 3;
        private int threads;
        private bool diagnostics;
        private Stopwatch timer;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int colsOrig;
        private IBitMatrix matrix;
        private Ancestor[] ancestors;
        int[] rowWeights;
        int[] colWeights;


#if DEBUG
        private IBitMatrix matrixOrig;
#endif

        public StructuredGaussianElimination(int threads, bool diagnostics)
        {
            this.threads = threads != 0 ? threads : 1;
            this.diagnostics = diagnostics;
            this.solver = new GaussianElimination<TArray>(threads);
        }

        public IEnumerable<IBitArray> Solve(IBitMatrix matrix)
        {
#if DEBUG
            matrixOrig = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), matrix);
#endif
            if (diagnostics)
            {
                Console.WriteLine("original matrix: {0} rows, {1} cols", matrix.Rows, matrix.Cols);
                timer = new Stopwatch();
                timer.Restart();
            }
            colsOrig = matrix.Cols;
            this.matrix = matrix;
            var compactMatrix = CompactMatrix();
            if (diagnostics)
            {
                Console.WriteLine("compaction: {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("matrix compacted: {0} rows, {1} cols", compactMatrix.Rows, compactMatrix.Cols);
            }
            foreach (var v in solver.Solve(compactMatrix))
                yield return GetOriginalSolution(v);
        }

        private IBitArray GetOriginalSolution(IBitArray w)
        {
            var v = (IBitArray)Activator.CreateInstance(typeof(TArray), colsOrig);
            for (int i = 0; i < w.Length; i++)
            {
                if (w[i])
                {
                    for (var ancestor = ancestors[i]; ancestor != null; ancestor = ancestor.Next)
                        v[ancestor.Column] = !v[ancestor.Column];
                }
            }
#if DEBUG
            Debug.Assert(GaussianElimination<TArray>.IsSolutionValid(matrixOrig, v));
#endif
            return v;
        }

        private IBitMatrix CompactMatrix()
        {
            var deletedRows = new bool[matrix.Rows];
            var deletedCols = new bool[matrix.Cols];
            ancestors = new Ancestor[matrix.Cols];
            for (int i = 0; i < matrix.Cols; i++)
                ancestors[i] = new Ancestor { Column = i };
            rowWeights = matrix.GetRowWeights().ToArray();
            colWeights = matrix.GetColWeights().ToArray();
            Debug.Assert(rowWeights.Sum() == colWeights.Sum());
            int surplusCols = 0;
            int pass = 1;
            if (diagnostics)
                Console.WriteLine("initial density = {1:F3}/col", pass, (double)colWeights.Sum() / matrix.Rows);
            while (true)
            {
                int deleted = 0;
                for (int n = matrix.Rows - 1; n >= 0; n--)
                {
                    if (deletedRows[n])
                        continue;
                    var weight = rowWeights[n];
                    if (weight == 0)
                    {
                        deletedRows[n] = true;
                        ++surplusCols;
                        ++deleted;
                        continue;
                    }
                    if (weight == 1)
                    {
                        var col = matrix.GetNonZeroIndices(n).First();
                        deletedRows[n] = true;
                        ClearColumn(col);
                        deletedCols[col] = true;
                        ++deleted;
                        continue;
                    }
                    if (weight > mergeLimit && surplusCols > 0 && weight - surplusCols <= mergeLimit)
                    {
                        while (weight > 2)
                        {
                            var col = matrix.GetNonZeroIndices(n)
                                .OrderByDescending(index => colWeights[index])
                                .First();
                            ClearColumn(col);
                            deletedCols[col] = true;
                            ++deleted;
                            --surplusCols;
                            --weight;
                        }
                    }
                    if (weight > mergeLimit)
                        continue;
                    if (weight == 2)
                    {
                        var cols = matrix.GetNonZeroIndices(n)
                            .OrderBy(index => colWeights[index])
                            .ToArray();
                        var col1 = cols[0];
                        var col2 = cols[1];
                        MergeColumns(col1, col2);
                        var ancestor = ancestors[col1];
                        while (ancestor.Next != null)
                            ancestor = ancestor.Next;
                        ancestor.Next = ancestors[col2];
                        ancestors[col2] = null;
                        deletedRows[n] = true;
                        deletedCols[col2] = true;
                        ++deleted;
                        continue;
                    }
                    if (weight == 3)
                    {
                        var cols = matrix.GetNonZeroIndices(n)
                            .OrderBy(index => colWeights[index])
                            .ToArray();
                        var col1 = cols[0];
                        var col2 = cols[1];
                        var col3 = cols[2];
                        MergeColumns(col1, col2, col3);
                        for (var ancestor = ancestors[col3]; ancestor != null; ancestor = ancestor.Next)
                        {
                            ancestors[col1] = new Ancestor { Column = ancestor.Column, Next = ancestors[col1] };
                            ancestors[col2] = new Ancestor { Column = ancestor.Column, Next = ancestors[col2] };
                        }
                        ancestors[col3] = null;
                        deletedRows[n] = true;
                        deletedCols[col3] = true;
                        ++deleted;
                        continue;
                    }
                }
                if (deleted == 0 && surplusCols > 0)
                {
                    for (int col = matrix.Cols - 1; surplusCols > 0; col--)
                    {
                        if (deletedCols[col])
                            continue;
                        ClearColumn(col);
                        deletedCols[col] = true;
                        ++deleted;
                        --surplusCols;
                    }
                }
                if (deleted == 0)
                    break;
                if (diagnostics)
                    Console.WriteLine("pass {0}: deleted {1} rows", pass, deleted);
                ++pass;
            }
            Debug.Assert(rowWeights.Sum() == colWeights.Sum());

            // Compute mapping between original matrix and compact matrix.
            ancestors = deletedCols
                .Select((deleted, index) => deleted ? null : ancestors[index])
                .Where(ancestor => ancestor != null)
                .ToArray();
            var rowMap = deletedRows
                .Select((deleted, index) => deleted ? -1 : index)
                .Where(index => index != -1)
                .ToArray();
            var colMap = deletedCols
                .Select((deleted, index) => deleted ? -1 : index)
                .Where(index => index != -1)
                .ToArray();

            // Permute columns to sort by increasing column weight.
            var order = Enumerable.Range(0, colMap.Length)
                .OrderBy(index => colWeights[colMap[index]])
                .ToArray();
            ancestors = Enumerable.Range(0, colMap.Length)
                .Select(index => ancestors[order[index]])
                .ToArray();
            colMap = Enumerable.Range(0, colMap.Length)
                .Select(index => colMap[order[index]])
                .ToArray();

            // Create compact matrix.
            var revColMap = new int[matrix.Cols];
            for (int i = 0; i < colMap.Length; i++)
                revColMap[colMap[i]] = i;
            var compactMatrix = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rowMap.Length, colMap.Length);
            for (int i = 0; i < rowMap.Length; i++)
            {
                int row = rowMap[i];
                foreach (var col in matrix.GetNonZeroIndices(row))
                    compactMatrix[i, revColMap[col]] = true;
            }

            if (diagnostics)
            {
                Console.WriteLine("completed compaction in {0} passes", pass);
                Console.WriteLine("final density = {0:F3}/col", (double)colWeights.Sum() / compactMatrix.Rows);
            }
            return compactMatrix;
        }

        private void ClearColumn(int col)
        {
            if (threads == 1 || matrix.Rows < multiThreadedCutoff)
                ClearColumn(col, 0, 1);
            else
                Parallel.For(0, threads, thread => ClearColumn(col, thread, threads));
            colWeights[col] = 0;
            Debug.Assert(colWeights[col] == matrix.GetColWeight(col));
        }

        private void MergeColumns(int col1, int col2)
        {
            var colIndices = new[] { col1 };
            var deltas = new int[1];
            if (threads == 1 || matrix.Rows < multiThreadedCutoff)
                MergeColumns(colIndices, col2, deltas, 0, 1);
            else
                Parallel.For(0, threads, thread => MergeColumns(colIndices, col2, deltas, thread, threads));
            colWeights[col1] += deltas[0];
            colWeights[col2] = 0;
            Debug.Assert(colWeights[col1] == matrix.GetColWeight(col1));
            Debug.Assert(colWeights[col2] == matrix.GetColWeight(col2));
        }

        private void MergeColumns(int col1, int col2, int col3)
        {
            var colIndices = new[] { col1, col2 };
            var deltas = new int[2];
            if (threads == 1 || matrix.Rows < multiThreadedCutoff)
                MergeColumns(colIndices, col3, deltas, 0, 1);
            else
                Parallel.For(0, threads, thread => MergeColumns(colIndices, col3, deltas, thread, threads));
            colWeights[col1] += deltas[0];
            colWeights[col2] += deltas[1];
            colWeights[col3] = 0;
            Debug.Assert(colWeights[col1] == matrix.GetColWeight(col1));
            Debug.Assert(colWeights[col2] == matrix.GetColWeight(col2));
            Debug.Assert(colWeights[col3] == matrix.GetColWeight(col3));
        }

        public void ClearColumn(int srcCol, int start, int incr)
        {
            int rows = matrix.Rows;
            for (int i = start; i < rows; i += incr)
            {
                if (matrix[i, srcCol])
                {
                    matrix[i, srcCol] = false;
                    --rowWeights[i];
                    Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                }
            }
        }

        public void MergeColumns(int[] colIndices, int srcCol, int[] deltas, int start, int incr)
        {
            int rows = matrix.Rows;
            int cols = colIndices.Length;
            for (int i = start; i < rows; i += incr)
            {
                if (matrix[i, srcCol])
                {
                    for (int j = 0; j < cols; j++)
                    {
                        int col = colIndices[j];
                        var old = matrix[i, col];
                        matrix[i, col] = !old;
                        rowWeights[i] += old ? -1 : 1;
                        if (old)
                            Interlocked.Decrement(ref deltas[j]);
                        else
                            Interlocked.Increment(ref deltas[j]);
                    }

                    matrix[i, srcCol] = false;
                    --rowWeights[i];

                    Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                }
            }
        }
    }
}
