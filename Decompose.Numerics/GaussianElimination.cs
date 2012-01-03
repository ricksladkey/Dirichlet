using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class GaussianElimination<TArray> : INullSpaceAlgorithm<IBitArray, IBitMatrix> where TArray : IBitArray, new()
    {
        private int threads;

        public GaussianElimination(int threads)
        {
            this.threads = threads;
        }

        public IEnumerable<IBitArray> Solve(IBitMatrix matrix)
        {
#if false
            PrintMatrix("initial:", matrix);
#endif
            int rows = Math.Min(matrix.Rows, matrix.Cols);
            int cols = matrix.Cols;
#if false
            int removed = 0;
            for (int i = rows - 1; i >= 0; i--)
            {
                if (matrix.IsRowEmpty(i))
                {
                    matrix.RemoveRow(i);
                    --rows;
                    ++removed;
                }
            }
#endif
            var c = new int[cols];
            var cInv = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                c[j] = -1;
                cInv[j] = -1;
            }
            for (int k = 0; k < cols; k++)
            {
                int j = -1;
                for (int i = 0; i < rows; i++)
                {
                    if (matrix[i, k] && c[i] < 0)
                    {
                        j = i;
                        break;
                    }
                }
                if (j != -1)
                {
                    ZeroColumn(matrix, rows, j, k);
                    c[j] = k;
                    cInv[k] = j;
                }
                else
                {
                    var v = new TArray();
                    v.Length = cols;
                    int ones = 0;
                    for (int jj = 0; jj < rows; jj++)
                    {
                        int s = cInv[jj];
                        bool value;
                        if (s != -1)
                            value = matrix[s, k];
                        else if (jj == k)
                            value = true;
                        else
                            value = false;
                        v[jj] = value;
                        if (value)
                            ++ones;
                    }
#if DEBUG
                    Debug.Assert(VerifySolution(matrix, 0, matrix.Rows, v));

#endif
                    if (VerifySolution(matrix, rows, matrix.Rows, v))
                        yield return v;
                }
#if false
                PrintMatrix(string.Format("k = {0}", k), matrix);
#endif
            }
        }

        private void ZeroColumn(IBitMatrix matrix, int rows, int j, int k)
        {
            if (rows < 256)
            {
                for (int i = 0; i < rows; i++)
                {
                    if (i == j || !matrix[i, k])
                        continue;
                    matrix.XorRows(i, j);
                }
            }
            else
            {
                int range = (rows + threads - 1) / threads;
                Parallel.For(0, threads, thread =>
                {
                    int beg = thread * range;
                    int end = Math.Min(beg + range, rows);
                    for (int i = beg; i < end; i++)
                    {
                        if (i != j && matrix[i, k])
                            matrix.XorRows(i, j);
                    }
                });
            }
        }

        private bool VerifySolution(IBitMatrix matrix, int rowMin, int rowMax, IBitArray solution)
        {
            int cols = matrix.Cols;
            for (int i = rowMin; i < rowMax; i++)
            {
                bool row = false;
                for (int j = 0; j < cols; j++)
                {
                    row ^= solution[j] & matrix[i, j];
                }
                if (row)
                    return false;
            }
            return true;
        }

        private void PrintMatrix(string label, List<List<int>> matrix)
        {
            Console.WriteLine(label);
            for (int i = 0; i < matrix.Count; i++)
                Console.WriteLine(string.Join(" ", matrix[i].ToArray()));
        }
    }
}
