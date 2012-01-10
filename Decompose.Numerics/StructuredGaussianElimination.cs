using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int rowsOrig;
        private int colsOrig;
        private Ancestor[] ancestors;

#if false
        private IBitMatrix matrixOrig;
#endif

        public StructuredGaussianElimination(int threads)
        {
            this.threads = threads != 0 ? threads : 1;
            this.solver = new GaussianElimination<TArray>(threads);
        }

        public IEnumerable<IBitArray> Solve(IBitMatrix matrix)
        {
#if false
            matrixOrig = (IBitMatrix)Activator.CreateInstance(typeof(TMatrix), matrix);
#endif
            rowsOrig = matrix.Rows;
            colsOrig = matrix.Cols;
#if false
            var timer = new Stopwatch();
            timer.Restart();
#endif
            matrix = CompactMatrix(matrix);
#if false
            Console.WriteLine("compaction: {0:F3}", (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000);
#endif
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
            Debug.Assert(GaussianElimination<TArray>.IsSolutionValid(matrixOrig, v))
#endif
            return v;
        }

        private IBitMatrix CompactMatrix(IBitMatrix matrix)
        {
            var deletedRows = new bool[matrix.Rows];
            var deletedCols = new bool[matrix.Cols];
            var weights = new int[matrix.Rows];
            ancestors = new Ancestor[matrix.Cols];
            for (int i = 0; i < matrix.Cols; i++)
                ancestors[i] = new Ancestor { Column = i };
            for (int i = 0; i < matrix.Rows; i++)
                weights[i] = matrix.GetRowWeight(i);
            while (true)
            {
                int deleted = 0;
                for (int n = matrix.Rows - 1; n >= 0; n--)
                {
                    if (deletedRows[n])
                        continue;
                    var weight = weights[n];
                    if (weight == 0)
                    {
                        deletedRows[n] = true;
                        ++deleted;
#if false
                        Console.WriteLine("deleted empty row {0}", n);
#endif
                    }
                    else if (weight == 1)
                    {
                        var col = matrix.GetNonZeroIndices(n).First();
                        deletedRows[n] = true;
                        ClearColumn(matrix, weights, col);
                        deletedCols[col] = true;
                        ++deleted;
#if false
                        Console.WriteLine("deleted row {0} and column {1}", n, col);
#endif
                    }
                    else if (weight == 2)
                    {
                        var cols = matrix.GetNonZeroIndices(n).ToArray();
                        var col1 = cols[0];
                        var col2 = cols[1];
                        MergeColumns(matrix, weights, col1, col2);
                        var ancestor = ancestors[col1];
                        while (ancestor.Next != null)
                            ancestor = ancestor.Next;
                        ancestor.Next = ancestors[col2];
                        ancestors[col2] = null;
                        deletedRows[n] = true;
                        deletedCols[col2] = true;
                        ++deleted;
#if false
                        Console.WriteLine("merged column {0} into {1}", col2, col1);
#endif
                    }
                }
                if (deleted == 0)
                    break;
#if false
                Console.WriteLine("removed {0} rows", deleted);
#endif
            }
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

        private void ClearColumn(IBitMatrix matrix, int[] weights, int col)
        {
            int rows = matrix.Rows;
            if (threads == 1 || rows < multiThreadedCutoff)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, col])
                    {
                        --weights[i];
                        matrix[i, col] = false;
                    }
                    Debug.Assert(weights[i] == matrix.GetRowWeight(i));
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
                            --weights[i];
                            matrix[i, col] = false;
                        }
                        Debug.Assert(weights[i] == matrix.GetRowWeight(i));
                    }
                });
            }
        }

        private void MergeColumns(IBitMatrix matrix, int[] weights, int col1, int col2)
        {
            int rows = matrix.Rows;
            if (threads == 1 || rows < multiThreadedCutoff)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, col2])
                    {
                        var old = matrix[i, col1];
                        if (old)
                            weights[i] -= 2;
                        matrix[i, col1] = !old;
                        matrix[i, col2] = false;
                        Debug.Assert(weights[i] == matrix.GetRowWeight(i));
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
                                weights[i] -= 2;
                            matrix[i, col1] = !old;
                            matrix[i, col2] = false;
                            Debug.Assert(weights[i] == matrix.GetRowWeight(i));
                        }
                    }
                });
            }
        }
    }
}
