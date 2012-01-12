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
                        v[ancestor.Column] = !v[ancestor.Column];
                }
            }
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
                    if (weight > mergeLimit && surplusCols > 0 && weight - surplusCols <= mergeLimit)
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
                    if (weight > mergeLimit)
                        continue;
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
                        MergeColumns(matrix, rowWeights, colWeights, col1, col2, col3);
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

        private void MergeColumns(IBitMatrix matrix, int[] rowWeights, int[] colWeights, int col1, int col2, int col3)
        {
            int rows = matrix.Rows;
            int delta1 = 0;
            int delta2 = 0;
            if (threads == 1 || rows < multiThreadedCutoff)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, col3])
                    {
                        var old1 = matrix[i, col1];
                        matrix[i, col1] = !old1;
                        rowWeights[i] += old1 ? -1 : 1;
                        delta1 += old1 ? -1 : 1;
                        var old2 = matrix[i, col2];
                        rowWeights[i] += old2 ? -1 : 1;
                        matrix[i, col2] = !old2;
                        delta2 += old2 ? -1 : 1;
                        matrix[i, col3] = false;
                        --rowWeights[i];
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
                        if (matrix[i, col3])
                        {
                            var old1 = matrix[i, col1];
                            matrix[i, col1] = !old1;
                            rowWeights[i] += old1 ? -1 : 1;
                            if (old1)
                                Interlocked.Decrement(ref delta1);
                            else
                                Interlocked.Increment(ref delta1);

                            var old2 = matrix[i, col2];
                            matrix[i, col2] = !old2;
                            rowWeights[i] += old2 ? -1 : 1;
                            if (old2)
                                Interlocked.Decrement(ref delta2);
                            else
                                Interlocked.Increment(ref delta2);

                            matrix[i, col3] = false;
                            --rowWeights[i];

                            Debug.Assert(rowWeights[i] == matrix.GetRowWeight(i));
                        }
                    }
                });
            }
            colWeights[col1] += delta1;
            colWeights[col2] += delta2;
            colWeights[col3] = 0;
            Debug.Assert(colWeights[col1] == matrix.GetColWeight(col1));
            Debug.Assert(colWeights[col2] == matrix.GetColWeight(col2));
            Debug.Assert(colWeights[col3] == matrix.GetColWeight(col3));
        }
    }
}
