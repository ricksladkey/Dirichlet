using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace Decompose.Numerics
{
    public class MertensRange
    {
        private MobiusRange mobius;
        private long u;
        private long[] m;
        private long[] mx;

        public MertensRange(MobiusRange mobius)
        {
            this.mobius = mobius;

            var nmax = mobius.Size - 1;
            u = (long)IntegerMath.FloorPower((BigInteger)nmax, 2, 3);
            m = new long[u + 1];
            var values = new sbyte[u + 1];
            mobius.GetValues(0, u + 1, values);
            var t = (long)0;
            for (var i = 0; i <= u; i++)
            {
                t += values[i];
                m[i] = t;
            }
            mx = new long[nmax / u + 1];
        }

        public long Evaluate(long n)
        {
            var threads = mobius.Threads;
            var v = Math.Max(1, n / u);
            if (threads <= 1)
            {
                for (var i = 1; i <= v; i++)
                    UpdateMx(n, v, i);
            }
            else
            {
                var batchSize = (v + threads - 1) / threads;
                var tasks = new Task[threads];
                for (var thread = 0; thread < threads; thread++)
                {
                    var offset = thread + 1;
                    tasks[thread] = Task.Factory.StartNew(() =>
                        {
                            for (var i = offset; i <= v; i += threads)
                                UpdateMx(n, v, i);
                        });
                }
                Task.WaitAll(tasks);
            }
            for (var i = v; i >= 1; i--)
            {
                var s = (long)0;
                var ijmax = v / i * i;
                for (var ij = 2 * i; ij <= ijmax; ij += i)
                    s += mx[ij];
                mx[i] -= s;
            }
            return mx[1];
        }

        private void UpdateMx(long n, long v, long i)
        {
            var ni = n / i;
            var s = (long)0;
            var jmax = IntegerMath.FloorSquareRoot(ni);
            var kmax = ni / jmax;
            var jmin = v / i;
            for (var j = jmin + 1; j <= jmax; j++)
                s += m[ni / j];
            var current = ni;
            for (var k = 1; k < kmax; k++)
            {
                var next = ni / (k + 1);
                s += (current - next) * m[k];
                current = next;
            }
            mx[i] = 1 - s;
        }
    }
}
