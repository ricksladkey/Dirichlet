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
        public struct Hyperbola
        {
            public Rational A;
            public Rational B;
            public Rational C;
            public Rational D;
            public Rational E;
            public Rational F;

            public Hyperbola SkewX(Rational r)
            {
                var h = new Hyperbola();
                h.A = A;
                h.B = B - 2 * A;
                h.C = C - B + A;
                h.D = 2 * A * r + D;
                h.E = E - D + r * B + 2 * r * A;
                h.F = F + r * D + r * r * A;
                return h;
            }

            public Hyperbola SkewY(Rational r)
            {
                var h = new Hyperbola();
                h.A = C - B + A;
                h.B = B - 2 * C;
                h.C = C;
                h.D = (B - 2 * C) * r - E + D;
                h.E = 2 * C * r + E;
                h.F = C * r * r + E * r + F;
                return h;
            }

            public double CriticalPoint()
            {
                var p = (double)(2 * C - B);
                var q = (double)((-4 * A * C * C + (B * B + 4 * A * B - 4 * A * A) * C - B * B * B + A * B * B) * F
                    + (A * C - A * B + A * A) * E * E + (-B * C + B * B - A * B) * D * E
                    + (C * C + (A - B) * C) * D * D);
                var r = (double)((-B * C + B * B - A * B) * E + (2 * C * C + (2 * A - 2 * B) * C) * D);
                var s = (double)(4 * A * C * C + (-B * B - 4 * A * B + 4 * A * A) * C + B * B * B - A * B * B);
                var root1 = (r + p * Math.Sqrt(q)) / s;
                var root2 = (r - p * Math.Sqrt(q)) / s;
                return Math.Max(root1, root2);
            }
        };

        private bool diag;
        private BigInteger n;
        private BigInteger xmin;
        private BigInteger xmax;

        public DivisorSummatoryFunction(bool diag)
        {
            this.diag = diag;
        }

        private void CheckDeltaY(Rational m)
        {
            var x1 = (BigInteger)IntegerMath.FloorSquareRoot(n / m);
            var y1 = n / x1;
            var r1 = y1 + m * x1;
#if false
            if ((x1 + 1) * (r1 - m * (x1 + 1)) > n)
                Console.WriteLine("x + 1 is over: m = {0}", m);
#endif
            if ((x1 + 2) * (r1 - m * (x1 + 2)) > n)
                Console.WriteLine("x + 2 is over: m = {0}", m);
        }

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;

            var sum = (BigInteger)0;

            xmin = IntegerMath.FloorRoot(n, 3);
            xmax = IntegerMath.FloorRoot(n, 2);

            if (diag)
            {
                Console.WriteLine("n = {0}", n);
#if true
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
#endif
#if false
                for (var m = (Rational)1; m <= xmin; m++)
                {
                    CheckDeltaY(m);
                    CheckDeltaY(m + (Rational)1 / 2);
                }
                return 0;
#endif
            }

            var m0 = (BigInteger)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m0 * x0;
            var width = x0 - xmin;
            Debug.Assert(r0 - m0 * x0== y0);

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

            while (true)
            {
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
                    for (var x = (BigInteger)xmin; x < x0; x++)
                    {
                        var delta = n / x - Rational.Floor(r0 - m0 * x);
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

                var region = ProcessRegion(x1b, y1b, m1, r1b, x0, y0, m0, r0);
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

        private BigInteger ProcessRegion(BigInteger x1, BigInteger y1, Rational m1, Rational r1, BigInteger x0, BigInteger y0, Rational m0, Rational r0)
        {
#if DEBUG
            var expected = (BigInteger)0;
            if (diag)
            {
                expected = ProcessRegionGrid(x1, y1, m1, r1, x0, y0, m0, r0, false);
                var region = ProcessRegionDivide(x1, y1, m1, r1, x0, y0, m0, r0);
                if (region != expected)
                {
                    Console.WriteLine("failed validation: actual = {0}, expected = {1}", region, expected);
                }
                return region;
            }
#endif
            return ProcessRegionDivide(x1, y1, m1, r1, x0, y0, m0, r0);
        }

        private BigInteger ProcessRegionDivide(BigInteger x1, BigInteger y1, Rational m1, Rational r1, BigInteger x0, BigInteger y0, Rational m0, Rational r0)
        {
            // Sub-divide the new hyperbolic region.
            if (x1 >= x0)
                return 0;

            Debug.Assert(x1 < x0);
            Debug.Assert(y1 > y0);
            Debug.Assert(r1 > r0);
            Debug.Assert(m1 > m0);

            if (x0 - x1 <= 20)
                return ProcessRegionLine(x1, y1, m1, r1, x0, y0, m0, r0, diag);

            var sum = (BigInteger)0;

            // Determine intersection of L0 and L1.
            var x01 = (BigInteger)((r1 - r0) / (m1 - m0));
            var y01 = (BigInteger)(r0 - m0 * x01);
            Debug.Assert(r0 - m0 * x01 == r1 - m1 * x01);

            // Check point at (1, 1).
            var xtest = x01 + m0.Denominator - m1.Denominator;
            var ytest = y01 - m0.Numerator + m1.Numerator;
            if (xtest * ytest > n)
                return sum;

            // L2 is the line with the mediant of the slopes of L0 and L1
            // passing through the point on or below the hyperbola nearest that slope.
            var m0n = m0.Numerator;
            var m0d = m0.Denominator;
            var m1n = m1.Numerator;
            var m1d = m1.Denominator;
            var m2n = m0n + m1n;
            var m2d = m0d + m1d;
            var m2nd = m2n*m2d;
            var mxy1 = m1n * x01 + m1d * y01;
            var mxy2 = m2n * x01 + m2d * y01;
            var mult = (2 * m1d * m2n + 1);
            var sqrt = IntegerMath.FloorSquareRoot(4 * mult * mult * m2nd * n);
            var sqrt1 = sqrt / 2;
            var sqrt2 = sqrt / mult;
            var u2a = (sqrt1 - m2nd * mxy1) / m2nd;
            var v2a = sqrt2 - u2a - mxy2;
            var u2b = u2a + 1;
            var v2b = sqrt2 - u2b - mxy2;

            // Check for under-estimate of v.
            if ((x01 - m1d * (v2a + 1) + m0d * u2a) * (y01 + m1n * (v2a + 1) - m0n * u2a) <= n)
                ++v2a;
            if ((x01 - m1d * (v2b + 1) + m0d * u2b) * (y01 + m1n * (v2b + 1) - m0n * u2b) <= n)
                ++v2b;

            var m2 = new Rational(m2n, m2d);
            var v12a = u2a + v2a;
            var v12b = u2b + v2b;

            // Transform back to x, y coordinate system.
            var x2a = x01 - m1d * v2a + m0d * u2a;
            var y2a = y01 + m1n * v2a - m0n * u2a;
            var r2a = y2a + m2 * x2a;
            var x2b = x01 - m1d * v2b + m0d * u2b;
            var y2b = y01 + m1n * v2b - m0n * u2b;
            var r2b = y2b + m2 * x2b;

            // Intersections of L1 with L2a and L2b.
            var x12a = x01 - m1d * v12a;
            var y12a = y01 + m1n * v12a;
            var x12b = x01 - m1d * v12b;
            var y12b = y01 + m1n * v12b;

#if true
            if (diag)
            {
                Console.WriteLine("m1 = {0,5}, m2 = {1,5}, m0 = {2,5}, x1 = {3,4}, x2 = {4,4}, x0 = {5,4}, dx = {6}", m1, m2, m0, x1, x2a, x0, x0 - x1);
                Console.WriteLine("x0, y0   = ({0}, {1}), m0 = {2}, r0 = {3}", x0, y0, m0, r0);
                Console.WriteLine("x1, y1   = ({0}, {1}), m1 = {2}, r1 = {3}", x1, y1, m1, r1);
                Console.WriteLine("x01, y01 = ({0}, {1})", x01, y01);
                Console.WriteLine("x2a, y2a = ({0}, {1}), m2 = {2}, r2a = {3}", x2a, y2a, m2, r2a);
                Console.WriteLine("x2b, y2b = ({0}, {1}), m2 = {2}, r2b = {3}", x2b, y2b, m2, r2b);
                Console.WriteLine("u2a, v2a = ({0}, {1})", u2a, v2a);
                Console.WriteLine("u2b, v2b = ({0}, {1})", u2b, v2b);
            }
#endif

            // Add the triangle defined L0, L1, and L2b.
            var area = v12b * (v12b - 1) / 2;
            sum += area;
            if (diag)
            {
                Console.WriteLine("corner: m2 = {0}, area = {1}", m2, area);
                Console.WriteLine("v12a = {0}, v12b = {1}", v12a, v12b);
            }

            if (v12a != v12b)
            {
                var adjustment = u2a * (v12a - v12b);
                sum += adjustment;
                if (diag)
                    Console.WriteLine("corner: adjustment = {0}", adjustment);
            }

            // Process right region.
            sum += ProcessRegion(x2b, y2b, m2, r2b, x0, y0, m0, r0);

            // Process left region.
            sum += ProcessRegion(x1, y1, m1, r1, x2a, y2a, m2, r2a);

            return sum;
        }

        private BigInteger ProcessRegionGrid(BigInteger x1, BigInteger y1, Rational m1, Rational r1, BigInteger x0, BigInteger y0, Rational m0, Rational r0, bool verbose)
        {
            // Determine intersection of L0 and L1.
            var x01 = (BigInteger)((r1 - r0) / (m1 - m0));
            var y01 = (BigInteger)(r0 - m0 * x01);
            Debug.Assert(r0 - m0 * x01 == r1 - m1 * x01);

            if (x01 < x1 || x01 > x0)
                return 0;

            // Just count the remaining lattice points inside the parallelogram.
            var count = 0;
            var h = (int)((x01 - x1) / m1.Denominator);
            var w = (int)((x0 - x01) / m0.Denominator);
            for (var i = 1; i <= w; i++)
            {
                var xrow = x01 + i * m0.Denominator;
                var yrow = y01 - i * m0.Numerator;
                for (var j = 1; j <= h; j++)
                {
                    var x = xrow - j * m1.Denominator;
                    var y = yrow + j * m1.Numerator;
                    if (x * y <= n)
                        ++count;
                }
            }
            if (verbose)
                Console.WriteLine("region: count = {0}", count);
            return count;
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
    }
}
