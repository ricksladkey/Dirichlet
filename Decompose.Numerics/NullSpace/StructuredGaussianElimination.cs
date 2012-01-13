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
        private const int mergeLimit = 4;
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
            if (diagnostics)
                Console.WriteLine("initial density = {0:F3}/col", (double)colWeights.Sum() / matrix.Rows);

            int surplusCols = 0;
            int pass = 1;
            while (true)
            {
                int deleted = 0;
                for (int n = matrix.Rows - 1; n >= 0; n--)
                {
                    if (deletedRows[n])
                        continue;
                    var weight = rowWeights[n];

                    // Delete entirely empty rows.
                    if (weight == 0)
                    {
                        deletedRows[n] = true;
                        ++surplusCols;
                        ++deleted;
                        continue;
                    }
                    
                    // Delete rows with a single non-zero entry.
                    if (weight == 1)
                    {
                        var col = matrix.GetNonZeroIndices(n).First();
                        deletedRows[n] = true;
                        MergeColumns(col);
                        deletedCols[col] = true;
                        ++deleted;
                        continue;
                    }

                    // Use surplus rows to bring weight down to merge limit.
                    if (weight > mergeLimit && surplusCols > 0 && weight - surplusCols <= mergeLimit)
                    {
                        while (weight > mergeLimit)
                        {
                            var col = matrix.GetNonZeroIndices(n)
                                .OrderByDescending(index => colWeights[index])
                                .First();
                            MergeColumns(col);
                            deletedCols[col] = true;
                            ++deleted;
                            --surplusCols;
                            --weight;
                        }
                    }

                    // Merge low weight rows.
                    if (weight <= mergeLimit)
                    {
                        var cols = matrix.GetNonZeroIndices(n)
                            .OrderByDescending(index => colWeights[index])
                            .ToArray();
                        Debug.Assert(cols.Length == weight);
                        var srcCol = cols[0];
                        MergeColumns(cols);
                        for (var ancestor = ancestors[srcCol]; ancestor != null; ancestor = ancestor.Next)
                        {
                            for (int j = 1; j < weight; j++)
                                ancestors[cols[j]] = new Ancestor { Column = ancestor.Column, Next = ancestors[cols[j]] };
                        }
                        deletedRows[n] = true;
                        ancestors[srcCol] = null;
                        deletedCols[srcCol] = true;
                        ++deleted;
                        continue;
                    }
                }
                if (deleted == 0 && surplusCols > 0)
                {
                    if (diagnostics)
                        Console.WriteLine("deleting {0} surplus columns", surplusCols);
                    for (int col = matrix.Cols - 1; surplusCols > 0; col--)
                    {
                        if (deletedCols[col])
                            continue;
                        MergeColumns(col);
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

        private void MergeColumns(params int[] colIndices)
        {
            int cols = colIndices.Length;
            var deltas = new int[cols];
            if (threads == 1 || matrix.Rows < multiThreadedCutoff)
                MergeColumns(colIndices, deltas, 0, 1);
            else
                Parallel.For(0, threads, thread => MergeColumns(colIndices, deltas, thread, threads));
            for (int i = 1; i < cols; i++)
                colWeights[colIndices[i]] += deltas[i];
            colWeights[colIndices[0]] = 0;
            Debug.Assert(Enumerable.Range(1, cols - 1).All(col => colWeights[col] == matrix.GetColWeight(col)));
        }

        public void MergeColumns(int[] colIndices, int[] deltas, int start, int incr)
        {
            int rows = matrix.Rows;
            int cols = colIndices.Length;
            int srcCol = colIndices[0];
            for (int i = start; i < rows; i += incr)
            {
                if (matrix[i, srcCol])
                {
                    for (int j = 1; j < cols; j++)
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
