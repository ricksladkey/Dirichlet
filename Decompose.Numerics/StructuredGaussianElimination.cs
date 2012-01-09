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

        private int threads;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private int rowsOrig;
        private int colsOrig;
        private Ancestor[] ancestors;

        public StructuredGaussianElimination(int threads)
        {
            this.threads = threads != 0 ? threads : 1;
            this.solver = new GaussianElimination<TArray>(threads);
        }

        public IEnumerable<IBitArray> Solve(IBitMatrix matrix)
        {
            rowsOrig = matrix.Rows;
            colsOrig = matrix.Cols;
            matrix = CompactMatrix(matrix);
            foreach (var v in solver.Solve(matrix))
                yield return GetOriginalSolution(v);
        }

        private IBitArray GetOriginalSolution(IBitArray w)
        {
            var v = (IBitArray)Activator.CreateInstance(typeof(TArray), rowsOrig);
            for (int i = 0; i < w.Length; i++)
            {
                if (w[i])
                {
                    for (var ancestor = ancestors[i]; ancestor != null; ancestor = ancestor.Next)
                        v[ancestor.Column] = true;
                }
            }
            return v;
        }

        private IBitMatrix CompactMatrix(IBitMatrix bitMatrix)
        {
            var matrix = bitMatrix as HashSetBitMatrix;
            bool[] deletedRows = new bool[matrix.Rows];
            bool[] deletedCols = new bool[matrix.Cols];
            ancestors = new Ancestor[matrix.Cols];
            for (int i = 0; i < matrix.Cols; i++)
                ancestors[i] = new Ancestor { Column = i };
            while (true)
            {
                int deleted = 0;
                for (int n = matrix.Rows - 1; n >= 0; n--)
                {
                    if (deletedRows[n])
                        continue;
                    var weight = matrix.GetRowWeight(n);
                    if (weight == 0)
                    {
                        deletedRows[n] = true;
                        ++deleted;
                    }
                    else if (weight == 1)
                    {
                        var col = matrix.GetNonZeroCols(n).First();
                        deletedRows[n] = true;
                        for (int i = 0; i < matrix.Rows; i++)
                            matrix[i, col] = false;
                        deletedCols[col] = true;
                        ++deleted;
                    }
                    else if (weight == 2)
                    {
                        var cols = matrix.GetNonZeroCols(n).ToArray();
                        var col1 = cols[0];
                        var col2 = cols[1];
                        for (int i = 0; i < matrix.Rows; i++)
                        {
                            if (matrix[i, col2])
                            {
                                matrix[i, col1] = !matrix[i, col1];
                                matrix[i, col2] = false;
                            }
                        }
                        ancestors[col1] = new Ancestor { Column = col2, Next = ancestors[col1] };
                        deletedRows[n] = true;
                        deletedCols[col2] = true;
                        ++deleted;
                    }
                }
                if (deleted == 0)
                    break;
                Console.WriteLine("removed {0} rows", deleted);
            }
            ancestors = deletedCols
                .Select((deleted, index) => deleted ? null : ancestors[index])
                .Where(ancestor => ancestors != null)
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
            var compact = (IBitMatrix)Activator.CreateInstance(typeof(TArray), rowMap.Length, colMap.Length);
            for (int i = 0; i < rowMap.Length; i++)
            {
                int row = rowMap[i];
                foreach (var col in matrix.GetNonZeroCols(row))
                    compact[i, revColMap[col]] = true;
            }
            return compact;
        }
    }
}
