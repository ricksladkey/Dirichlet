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
        private int threads;
        private bool diagnostics;
        private Stopwatch timer;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int rowsOrig;
        private int colsOrig;
        private Ancestor[] ancestors;

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
            rowsOrig = matrix.Rows;
            colsOrig = matrix.Cols;
            if (diagnostics)
            {
                Console.WriteLine("original matrix: {0} rows, {1} cols", matrix.Rows, matrix.Cols);
                timer = new Stopwatch();
                timer.Restart();
            }
            matrix = CompactMatrix(matrix);
            if (diagnostics)
            {
                Console.WriteLine("compaction: {0:F3} msec", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
                Console.WriteLine("matrix compacted: {0} rows, {1} cols", matrix.Rows, matrix.Cols);
            }
            foreach (var v in solver.Solve(matrix))
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
                        v[ancestor.Column] = true;
                }
            }
#if false
            Console.WriteLine("w = {0}", string.Join(" ", w.GetNonZeroIndices()));
            Console.WriteLine("v = {0}", string.Join(" ", v.GetNonZeroIndices()));
#endif
#if DEBUG
            Debug.Assert(GaussianElimination<TArray>.IsSolutionValid(matrixOrig, v));
#endif
            return v;
        }

        private IBitMatrix CompactMatrix(IBitMatrix matrix)
        {
            var deletedRows = new bool[matrix.Rows];
            var deletedCols = new bool[matrix.Cols];
            ancestors = new Ancestor[matrix.Cols];
            for (int i = 0; i < matrix.Cols; i++)
                ancestors[i] = new Ancestor { Column = i };
            var rowWeights = matrix.GetRowWeights().ToArray();
            var colWeights = matrix.GetColWeights().ToArray();
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
                        ClearColumn(matrix, rowWeights, colWeights, col);
                        deletedCols[col] = true;
                        ++deleted;
                        continue;
                    }
                    if (weight > 2 && surplusCols > 0 && weight - surplusCols <= 2)
                    {
                        while (weight > 2)
                        {
                            var col = matrix.GetNonZeroIndices(n)
                                .OrderByDescending(index => colWeights[index])
                                .First();
                            ClearColumn(matrix, rowWeights, colWeights, col);
                            deletedCols[col] = true;
                            ++deleted;
                            --surplusCols;
                            --weight;
                        }
                    }
                    if (weight == 2)
                    {
                        var cols = matrix.GetNonZeroIndices(n)
                            .OrderBy(index => colWeights[index])
                            .ToArray();
                        var col1 = cols[0];
                        var col2 = cols[1];
                        MergeColumns(matrix, rowWeights, colWeights, col1, col2);
                        var ancestor = ancestors[col1];
                        while (ancestor.Next != null)
                            ancestor = ancestor.Next;
                        ancestor.Next = ancestors[col2];
                        ancestors[col2] = null;
                        deletedRows[n] = true;
                        deletedCols[col2] = true;
                        ++deleted;
                    }
                }
                if (deleted == 0 && surplusCols > 0)
                {
                    for (int col = matrix.Cols - 1; surplusCols > 0; col--)
                    {
                        if (deletedCols[col])
                            continue;
                        ClearColumn(matrix, rowWeights, colWeights, col);
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
            var revColMap = new int[matrix.Cols];
            for (int i = 0; i < colMap.Length; i++)
                revColMap[colMap[i]] = i;
            var compact = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), rowMap.Length, colMap.Length);
            for (int i = 0; i < rowMap.Length; i++)
            {
                int row = rowMap[i];
                foreach (var col in matrix.GetNonZeroIndices(row))
                    compact[i, revColMap[col]] = true;
            }
            if (diagnostics)
            {
                Console.WriteLine("completed compaction in {0} passes", pass);
                Console.WriteLine("final density = {0:F3}/col", (double)colWeights.Sum() / compact.Rows);
            }
#if false
            for (int i = 0; i < ancestors.Length; i++)
            {
                Console.Write("ancestors[{0}]:", i);
                for (var ancestor = ancestors[i]; ancestor != null; ancestor = ancestor.Next)
                    Console.Write(" {0}", ancestor.Column);
                Console.WriteLine();
            }
#endif
            return compact;
        }

        private void ClearColumn(IBitMatrix matrix, int[] rowWeights, int[] colWeights, int col)
        {
            int rows = matrix.Rows;
            if (threads == 1 || rows < multiThreadedCutoff)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, col])
                    {
                        --rowWeights[i];
                        matrix[i, col] = false;
                    }
                    Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                }
            }
            else
            {
                Parallel.For(0, threads, thread =>
                {
                    for (int i = thread; i < rows; i += threads)
                    {
                        if (matrix[i, col])
                        {
                            --rowWeights[i];
                            matrix[i, col] = false;
                        }
                        Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                    }
                });
            }
            colWeights[col] = 0;
            Debug.Assert(colWeights[col] == matrix.GetColWeight(col));
        }

        private void MergeColumns(IBitMatrix matrix, int[] rowWeights, int[] colWeights, int col1, int col2)
        {
            int rows = matrix.Rows;
            int delta = 0;
            if (threads == 1 || rows < multiThreadedCutoff)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, col2])
                    {
                        var old = matrix[i, col1];
                        if (old)
                            rowWeights[i] -= 2;
                        matrix[i, col1] = !old;
                        matrix[i, col2] = false;
                        delta += old ? -1 : 1;
                        Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                    }
                }
            }
            else
            {
                Parallel.For(0, threads, thread =>
                {
                    for (int i = thread; i < rows; i += threads)
                    {
                        if (matrix[i, col2])
                        {
                            var old = matrix[i, col1];
                            if (old)
                                rowWeights[i] -= 2;
                            matrix[i, col1] = !old;
                            matrix[i, col2] = false;
                            if (old)
                                Interlocked.Decrement(ref delta);
                            else
                                Interlocked.Increment(ref delta);
                            Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                        }
                    }
                });
            }
            colWeights[col1] += delta;
            colWeights[col2] = 0;
            Debug.Assert(colWeights[col1] == matrix.GetColWeight(col1));
            Debug.Assert(colWeights[col2] == matrix.GetColWeight(col2));
        }
    }
}
