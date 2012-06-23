using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Diagnostics;

namespace Decompose.Numerics
{
    public class DivisorSummatoryFunctionOdd
    {
        private struct Region
        {
            public Region(BigInteger w, BigInteger h, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
            {
                this.w = w; this.h = h; this.a1 = a1; this.b1 = b1; this.c1 = c1; this.a2 = a2; this.b2 = b2; this.c2 = c2;
            }
            public BigInteger w;
            public BigInteger h;
            public BigInteger a1;
            public BigInteger b1;
            public BigInteger c1;
            public BigInteger a2;
            public BigInteger b2;
            public BigInteger c2;
        }

        public static readonly BigInteger C1 = 5;
        public static readonly BigInteger C2 = 10;

        private BigInteger n;
        private Stack<Region> stack;

        public DivisorSummatoryFunctionOdd()
        {
            stack = new Stack<Region>();
        }

        public BigInteger Evaluate(BigInteger n)
        {
            var xmax = IntegerMath.FloorSquareRoot(n);
            var s = Evaluate(n, 1, (long)xmax);
            return 2 * s - xmax * xmax;
        }

        public BigInteger Evaluate(BigInteger n, long x0, long xmax)
        {
            this.n = n;
            var ymin = n / xmax;
            var xmin = IntegerMath.Max(x0, IntegerMath.Min(C1 * IntegerMath.CeilingRoot(2 * n, 3), xmax));
#if DEBUG
            Console.WriteLine("n = {0}, xmin = {1}, xmax = {2}", n, xmin, xmax);
#endif
            var s = (BigInteger)0;
            var a2 = (BigInteger)1;
            var x2 = (BigInteger)xmax;
            var y2 = ymin;
            var c2 = a2 * x2 + y2;
            while (true)
            {
                var a1 = a2 + 1;
                var x4 = IntegerMath.FloorSquareRoot(n / a1);
                var y4 = n / x4;
                var c4 = a1 * x4 + y4;
                var x5 = x4 + 1;
                var y5 = n / x5;
                var c5 = a1 * x5 + y5;
                if (x4 < xmin)
                    break;
                s += Triangle(c4 - c2 - xmin) - Triangle(c4 - c2 - x5) + Triangle(c5 - c2 - x5);
                s += ProcessRegion(a1 * x2 + y2 - c5, a2 * x5 + y5 - c2, a1, 1, c5, a2, 1, c2);
                while (stack.Count > 0)
                {
                    var r = stack.Pop();
                    s += ProcessRegion(r.w, r.h, r.a1, r.b1, r.c1, r.a2, r.b2, r.c2);
                }
                a2 = a1;
                x2 = x4;
                y2 = y4;
                c2 = c4;
            }
            s += (xmax - xmin + 1) * ymin + Triangle(xmax - xmin);
            for (var x = xmin; x < x2; x++)
                s += n / x - (a2 * (x2 - x) + y2);
            for (var x = x0; x < xmin; x++)
                s += n / x;
            return s;
        }

        public BigInteger ProcessRegion(BigInteger w, BigInteger h, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
#if DEBUG
            Console.WriteLine("ProcessRegion: w = {0}, h = {1}, a1/b1 = {2}/{3}, a2/b2 = {4}/{5}, c1 = {6}, c2 = {7}", w, h, a1, b1, a2, b2, c1, c2);
#endif
            var s = (BigInteger)0;
            var a3 = a1 + a2;
            var b3 = b1 + b2;
            if (h > 0 && H(w, 1, a1, b1, c1, a2, b2, c2) <= n)
            {
                s += w;
                ++c2;
                --h;
            }
            if (w > 0 && H(1, h, a1, b1, c1, a2, b2, c2) <= n)
            {
                s += h;
                ++c1;
                --w;
            }
            if (w <= C2 || h <= C2)
                return s + ProcessRegionManual(w, h, a1, b1, c1, a2, b2, c2);
            var u4 = UTan(a1, b1, c1, a2, b2, c2);
            if (u4 == 0)
                return s + ProcessRegionManual(w, h, a1, b1, c1, a2, b2, c2);
            var v4 = VFloor(u4, a1, b1, c1, a2, b2, c2);
            var u5 = u4 + 1;
            var v5 = VFloor(u5, a1, b1, c1, a2, b2, c2);
            var v6 = u4 + v4;
            var u7 = u5 + v5;
            if (u7 >= w)
                return s + ProcessRegionManual(w, h, a1, b1, c1, a2, b2, c2);
            s += Triangle(v6 - 1) - Triangle(v6 - u5) + Triangle(u7 - u5);
            stack.Push(new Region(u4, h - v6, a1, b1, c1, a3, b3, c1 + c2 + v6));
            stack.Push(new Region(w - u7, v5, a3, b3, c1 + c2 + u7, a2, b2, c2));
#if DEBUG
            Console.WriteLine("ProcessRegion: s = {0}", s);
#endif
            return s;
        }

        public BigInteger ProcessRegionManual(BigInteger w, BigInteger h, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            return w < h ? ProcessRegionHorizontal(w, h, a1, b1, c1, a2, b2, c2) : ProcessRegionVertical(w, h, a1, b1, c1, a2, b2, c2);
        }

        public BigInteger ProcessRegionHorizontal(BigInteger w, BigInteger h, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            var s = (BigInteger)0;
            for (var u = (BigInteger)1; u < w; u++)
                s += VFloor(u, a1, b1, c1, a2, b2, c2);
#if DEBUG
            Console.WriteLine("ProcessRegionHorizontal: s = {0}", s);
#endif
            return s;
        }

        public BigInteger ProcessRegionVertical(BigInteger w, BigInteger h, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            var s = (BigInteger)0;
            for (var v = (BigInteger)1; v < h; v++)
                s += UFloor(v, a1, b1, c1, a2, b2, c2);
#if DEBUG
            Console.WriteLine("ProcessRegionVertical: s = {0}", s);
#endif
            return s;
        }

        public BigInteger H(BigInteger u, BigInteger v, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            return (b2 * (u + c1) - b1 * (v + c2)) * (a1 * (v + c2) - a2 * (u + c1));
        }

        public BigInteger UTan(BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            return IntegerMath.FloorSquareRoot(BigInteger.Pow(a1 * b2 + b1 * a2 + 2 * a1 * b1, 2) * n / ((a1 + a2) * (b1 + b2))) - c1;
        }

        public BigInteger UFloor(BigInteger v, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            return ((a1 * b2 + b1 * a2) * (v + c2) - IntegerMath.CeilingSquareRoot(BigInteger.Pow(v + c2, 2) - 4 * a2 * b2 * n)) / (2 * a2 * b2) - c1;
        }

        public BigInteger VFloor(BigInteger u, BigInteger a1, BigInteger b1, BigInteger c1, BigInteger a2, BigInteger b2, BigInteger c2)
        {
            return ((a1 * b2 + b1 * a2) * (u + c1) - IntegerMath.CeilingSquareRoot(BigInteger.Pow(u + c1, 2) - 4 * a1 * b1 * n)) / (2 * a1 * b1) - c2;
        }

        public static BigInteger Triangle(BigInteger a)
        {
            return a * (a + 1) / 2;
        }

        public BigInteger S1(BigInteger n, BigInteger x1, BigInteger x2)
        {
            var s = (BigInteger)0;
            var x = x2;
            var beta = n / (x + 1);
            var eps = n - (x + 1) * beta;
            var delta = n / x - beta;
            var gamma = beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                if (eps >= x)
                {
                    ++delta;
                    gamma -= x;
                    eps -= x;
                    if (eps >= x)
                    {
                        ++delta;
                        gamma -= x;
                        eps -= x;
                        if (eps >= x)
                            break;
                    }
                }
                else if (eps < 0)
                {
                    --delta;
                    gamma += x;
                    eps += x;
                }
                gamma += 2 * delta;
                beta += delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (x - 1) * delta);

                s += beta;
                --x;
            }
            eps = n - (x + 1) * beta;
            delta = n / x - beta;
            gamma = beta - x * delta;
            while (x >= x1)
            {
                eps += gamma;
                var delta2 = eps >= 0 ? eps / x : (eps - x + 1) / x;
                delta += delta2;
                var a = x * delta2;
                eps -= a;
                gamma += 2 * delta - a;
                beta += delta;

                Debug.Assert(eps == n % x);
                Debug.Assert(beta == n / x);
                Debug.Assert(delta == beta - n / (x + 1));
                Debug.Assert(gamma == beta - (x - 1) * delta);

                s += beta;
                --x;
            }
            while (x >= x1)
            {
                s += n / x;
                --x;
            }
            return s;
        }
    }
}
