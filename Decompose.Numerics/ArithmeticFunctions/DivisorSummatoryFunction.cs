using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Decompose.Numerics
{
    public class DivisorSummatoryFunction
    {
        private struct Region
        {
            public Region(long w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01)
            {
                this.w = w; this.h = h; this.m1n = m1n; this.m1d = m1d; this.m0n = m0n; this.m0d = m0d; this.x01 = x01; this.y01 = y01;
            }
            public long w;
            public long h;
            public long m1n;
            public long m1d;
            public long m0n;
            public long m0d;
            public BigInteger x01;
            public BigInteger y01;
        }

        private readonly BigInteger smallRegionCutoff = 20;
        private readonly BigInteger minimumMultiplier = 5;

        private bool diag;
        private BigInteger n;
        private BigInteger xmin;
        private BigInteger xmax;

        private Stack<Region> stack;

        public DivisorSummatoryFunction(bool diag)
        {
            this.diag = diag;
            stack = new Stack<Region>();
        }

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;

            var sum = (BigInteger)0;

            xmin = IntegerMath.FloorRoot(n, 3) * minimumMultiplier;
            xmax = IntegerMath.FloorRoot(n, 2);

            if (diag)
            {
                Console.WriteLine("n = {0}", n);
                //PrintValuesInRange();
            }

            var m0 = (long)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m0 * x0;
            var width = x0 - xmin;
            Debug.Assert(r0 - m0 * x0 == y0);

            // Add the bottom rectangle.
            var square = (width + 1) * y0;
            sum += square;
            if (diag)
                Console.WriteLine("square: area = {1}", m0, square);

            // Add the isosceles right triangle from the initial skew.
            var triangle = width * (width + 1) / 2;
            sum += triangle;
            if (diag)
                Console.WriteLine("wedge: m0 = {0}, area = {1}", m0, triangle);

            // Process regions between integral slopes 1 & 2, 2 & 3, etc.
            // until we reach xmin.
            while (true)
            {
                // Find the largest point (x1a, y1a) where -H'(X) >= the new slope.
                var m1 = m0 + 1;
                var x1a = IntegerMath.FloorSquareRoot(n / m1);
                var y1a = n / x1a;
                var r1a = y1a + m1 * x1a;
                var x1b = x1a + 1;
                var y1b = n / x1b;
                var r1b = y1b + m1 * x1b;
                Debug.Assert(r1a - m1 * x1a == y1a);
                Debug.Assert(r1b - m1 * x1b == y1b);

                if (x1a < xmin)
                {
                    // Process the last few values above xmin as the number of
                    // points above the last L0.
                    for (var x = (BigInteger)xmin; x < x0; x++)
                    {
                        var delta = n / x - (r0 - m0 * x);
                        sum += delta;
                        if (diag)
                            Console.WriteLine("singleton >= xmin: x = {0}, delta = {1}", x, delta);
                    }
                    break;
                }

                Debug.Assert((x1a - 1) * (r1a - m1 * (x1a - 1)) <= n);
                Debug.Assert((x1b + 1) * (r1b - m1 * (x1b + 1)) <= n);

                // Add the triangular wedge above the previous slope and below the new one.
                var xintersect = (BigInteger)((r1a - r0) / (m1 - m0));
                width = xintersect - xmin;
                var area = width * (width + 1) / 2;
                sum += area;
                if (diag)
                    Console.WriteLine("wedge: x1 = {0}, m1 = {1}, area = {2}", x1a, m1, area);

                if (r1a != r1b && x1a < xintersect)
                {
                    // Remove the old triangle and add the new triangle.
                    var ow = x1a - xintersect;
                    var dr = r1a - r1b;
                    var adjustment = dr * (2 * ow + dr + 1) / 2; // (ow+dr)*(ow+dr+1)/2 - ow*(ow+1)/2
                    sum += adjustment;
                    if (diag)
                        Console.WriteLine("wedge: x1 = {0}, adjustment = {1}", x1b, adjustment);
                }

                // Determine intersection of L0 and L1.
                var x01 = (BigInteger)((r1b - r0) / (m1 - m0));
                var y01 = (BigInteger)(r0 - m0 * x01);
                Debug.Assert(r0 - m0 * x01 == r1b - m1 * x01);

                var w = (long)((y0 - y01) + m1 * (x0 - x01));
                var h = (long)((y1b - y01) + m0 * (x1b - x01));

                var region = ProcessRegion(w, h, m1, 1, m0, 1, x01, y01);
                sum += region;

                m0 = m1;
                x0 = x1a;
                y0 = y1a;
                r0 = r1a;
            }

            // Process values one thru xmin.
            for (var x = (BigInteger)1; x < xmin; x++)
            {
                var y = n / x;
                sum += y;
                if (diag)
                    Console.WriteLine("singleton < xmin: x = {0}, y = {1}", x, y);
            }

            // Account for the first octant.
            sum = 2 * sum - xmax * xmax;

            return sum;
        }

        private BigInteger ProcessRegion(long  w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01)
        {
            var sum = (BigInteger)0;

            // Sub-divide the new hyperbolic region.
            while (true)
            {
                while (true)
                {
                    if (w <= 0 || h <= 0)
                        break;

                    // Check for row removal.
                    {
                        // Check point at (w, 1).
                        if ((m0d * w - m1d + x01) * (m1n - m0n * w + y01) <= n)
                        {
                            // Remove the first row.
                            sum += w;
                            if (diag)
                                Console.WriteLine("removed: row = {0}", w);

                            x01 -= m1d;
                            y01 += m1n;
                            --h;
                            if (h == 0)
                                break;
                        }
                    }

                    // Check for column removal.
                    {
                        // Check point at (1, h).
                        if ((m0d - m1d * h + x01) * (m1n * h - m0n + y01) <= n)
                        {
                            // Remove the first column.
                            sum += h;
                            if (diag)
                                Console.WriteLine("removed: column = {0}", h);

                            x01 += m0d;
                            y01 -= m0n;
                            --w;
                            if (w == 0)
                                break;
                        }
                    }

                    // Invariants:
                    // H(u,v) at v=h, 0 <= u < 1
                    // H(u,v) at u=w, 0 <= v < 1
                    // -du/dv at v=h >= 0
                    // -dv/du at u=w >= 0
                    // In other words: the hyperbola is less than one unit away
                    // from P0 and P1 and the distance to the hyperbola
                    // increases monotonically as you approach (u, v) = (0, 0).
                    Debug.Assert((m0d - m1d * h + x01) * (+m1n * h - m0n + y01) > n);
                    Debug.Assert((m0d * w - m1d + x01) * (m1n -  m0n * w + y01) > n);

                    // Find the pair of points (u2a, v2a) and (u2b, v2b) below H(u,v) where:
                    // -dv/du at u=u2a >= 1
                    // -dv/du at u=u2b <= 1
                    // u2b = u2a + 1
                    var m2n = m0n + m1n;
                    var m2d = m0d + m1d;
                    var m2nd = m2n * m2d;
                    var mxy1 = m1n * x01 + m1d * y01;
                    var mxy2 = m2n * x01 + m2d * y01;
                    var sqrtcoef = 2 * m1d * m2n + 1;
                    var u2a = (long)(IntegerMath.FloorSquareRoot(sqrtcoef * sqrtcoef * n / m2nd) - mxy1);
                    var tan = (long)(IntegerMath.FloorSquareRoot(2 * 2 * m2nd * n) - mxy2);
                    var v2a = u2a != 0 ? tan - u2a : h;
                    var u2b = u2a < w ? u2a + 1 : w;
                    var v2b = tan - u2b;

                    // Check for under-estimate of v2a and/or v2b.
                    if (u2a != 0)
                    {
                        var v2aplus = v2a + 1;
                        if ((m0d * u2a - m1d * v2aplus + x01) * (m1n * v2aplus - m0n * u2a + y01) <= n)
                        ++v2a;
                    }
                    var v2bplus = v2b + 1;
                    if ((m0d * u2b - m1d * v2bplus + x01) * (m1n * v2bplus - m0n * u2b + y01) <= n)
                        ++v2b;

                    // V intercept of L2a and L2b.  Since the lines are diagonal the intercept
                    // is the same on both U and V axes, so v12a = u02a and v12b = u02b.
                    var v12a = u2a + v2a;
                    var v12b = u2b + v2b;

                    // Count points horizontally or vertically if one axis collapses
                    // or if the triangle exceeds the bounds of the rectangle.
                    if (u2a <= smallRegionCutoff || v2b <= smallRegionCutoff || IntegerMath.Max(v12a, v12b) > IntegerMath.Min(w, h))
                    {
                        if (h > w)
                            sum += CountPoints(true, w, m0n, m0d, m1n, m1d, x01, y01);
                        else
                            sum += CountPoints(false, h, m1n, m1d, m0n, m0d, x01, y01);
                        break;
                    }

                    // Add the triangle defined L0, L1, and L2b.
                    var v12 = IntegerMath.Min(v12a, v12b);
                    var area = v12 * (v12 - 1) / 2;
                    sum += area;
                    if (diag)
                        Console.WriteLine("corner: m1 = {0}, m0 = {1}, area = {2}", new Rational(m1n, m1d), new Rational(m0n, m0d), area);

                    if (v12a != v12b)
                    {
                        var adjustment = v12a > v12b ? u2a : v2b;
                        sum += adjustment;
                        if (diag)
                            Console.WriteLine("corner: adjustment = {0}", adjustment);
                    }

                    // Push left region onto the stack.
                    stack.Push(new Region(u2a, h - v12a, m1n, m1d, m2n, m2d, x01 - m1d * v12a, y01 + m1n * v12a));

                    // Process right region (no change to m0n and m0d).
                    w -= v12b;
                    h = v2b;
                    m1n = m2n;
                    m1d = m2d;
                    x01 = x01 + m0d * v12b;
                    y01 = y01 - m0n * v12b;
                }

                if (stack.Count == 0)
                    break;

                // Pop a region off the stack for processing.
                var region = stack.Pop();
                m1n = region.m1n;
                m1d = region.m1d;
                m0n = region.m0n;
                m0d = region.m0d;
                x01 = region.x01;
                y01 = region.y01;
                w = region.w;
                h = region.h;
            }

            return sum;
        }

        private long CountPoints(bool horizontal, long max, long m0n, long m0d, long m1n, long m1d, BigInteger x01, BigInteger y01)
        {
            var sum = (long)0;
            var mx1 = m1n * x01;
            var my1 = m1d * y01;
            var mxy1 = mx1 + my1;
            var m01s = m0d * m1n + m0n * m1d;
            var denom = 2 * m1n * m1d;
            var a = mxy1 * mxy1 - 2 * denom * n;
            var b = horizontal ? mx1 - my1 : my1 - mx1;
            var da = 2 * mxy1 - 1;
            for (var i = (long)1; i <= max; i++)
            {
                da += 2;
                a += da;
                b += m01s;
                var sqrt = IntegerMath.CeilingSquareRoot(a);
                sum += (long)((b - sqrt) / denom);
            }
            return sum;
        }

#if false
        private BigInteger ProcessRegionChecked(long w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01)
        {
            var expected = (BigInteger)0;
            if (diag)
            {
                expected = ProcessRegionGrid(w, h, m1n, m1d, m0n, m0d, x01, y01, false);
                var region = ProcessRegion(w, h, m1n, m1d, m0n, m0d, x01, y01);
                if (region != expected)
                {
                    Console.WriteLine("failed validation: actual = {0}, expected = {1}", region, expected);
                }
                return region;
            }
            return ProcessRegion(w, h, m1n, m1d, m0n, m0d, x01, y01);
        }

        private BigInteger ProcessRegionGrid(long w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01, bool verbose)
        {
            // Just count the remaining lattice points inside the parallelogram.
            var count = 0;
            for (var u = 1; u <= w; u++)
            {
                var xrow = x01 + u * m0d;
                var yrow = y01 - u * m0n;
                for (var v = 1; v <= h; v++)
                {
                    var x = xrow - v * m1d;
                    var y = yrow + v * m1n;
                    if (x * y <= n)
                        ++count;
                }
            }
            if (verbose)
                Console.WriteLine("region: count = {0}", count);
            return count;
        }

        private void UV2XY(ref Region r, BigInteger u, BigInteger v, out BigInteger x, out BigInteger y)
        {
            x = r.x01 - r.m1d * v + r.m0d * u;
            y = r.y01 + r.m1n * v - r.m0n * u;
        }

        private void XY2UV(ref Region r, BigInteger x, BigInteger y, out BigInteger u, out BigInteger v)
        {
            var dx = x - r.x01;
            var dy = y - r.y01;
            u = r.m1d * dy + r.m1n * dx;
            v = r.m0d * dy + r.m0n * dx;
        }

        private BigInteger ProcessRegionLine(BigInteger x1, BigInteger y1, Rational m1, Rational r1, BigInteger x0, BigInteger y0, Rational m0, Rational r0, bool verbose)
        {
            var sum = (BigInteger)0;
            for (var x = x1; x <= x0; x++)
            {
                var y = n / x;
                sum += IntegerMath.Min(IntegerMath.Max(y - Rational.Floor(r0 - m0 * x), 0), IntegerMath.Max(y - Rational.Floor(r1 - m1 * x), 0));
            }
            if (verbose)
                Console.WriteLine("region: sum = {0}", sum);
            return sum;
        }

        private void PrintValuesInRange()
        {
            if (xmax < 100)
            {
                for (var x = xmin; x <= xmax; x++)
                {
                    var y = n / x;
                    var s = "";
                    for (var i = 0; i < y; i++)
                        s += "*";
                    Console.WriteLine("{0,5} {1}", x, s);
                }
                for (var x = xmin; x <= xmax; x++)
                {
                    var y = n / x;
                    Console.WriteLine("x = {0}, y = {1}", x, y);
                }
            }
        }
#endif
    }
}
