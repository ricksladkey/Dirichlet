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

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;

            var sum = (BigInteger)0;

            xmin = IntegerMath.FloorRoot(n, 3);
            xmax = IntegerMath.FloorRoot(n, 2);

            if (diag)
            {
                for (var x = xmin; x <= xmax; x++)
                {
                    var y = n / x;
                    Console.WriteLine("x = {0}, y = {1}", x, y);
                }
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
                var x1a = IntegerMath.FloorRoot(n / m1, 2);
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

                if (r1a != r1b && x1b < xintersect)
                {
                    var adjustment = xintersect - x1b;
                    sum -= adjustment;
                    if (diag)
                        Console.WriteLine("wedge: x1 = {0}, adjustment = {1}", x1b, adjustment);
                }

                sum += ProcessRegion(x1b, y1b, m1, r1b, x0, y0, m0, r0);

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
            // Sub-divide the new hyperbolic region.
            if (x1 >= x0)
                return 0;

            Debug.Assert(x1 < x0);
            Debug.Assert(y1 > y0);
            Debug.Assert(r1 > r0);
            Debug.Assert(m1 > m0);

            // Determine intersection of L0 and L1.
            var x01 = (BigInteger)((r1 - r0) / (m1 - m0));
            var y01 = (BigInteger)(r0 - m0 * x01);
            Debug.Assert(r0 - m0 * x01 == r1 - m1 * x01);
            if (x01 < x1 || x01 > x0)
                return 0;

            // L2 is the line with the mediant of the slopes of L0 and L1
            // passing through the point on or below the hyperbola nearest that slope.
            var m2 = Rational.Mediant(m0, m1);
            var x2a = (BigInteger)IntegerMath.FloorRoot(n / m2, 2);
            var y2a = n / x2a;
            var r2a = y2a + m2 * x2a;
            var x2b = x2a + 1;
            var y2b = n / x2b;
            var r2b = y2b + m2 * x2b;
            if (x2a < x1 || x2a > x0)
                return 0;
            Debug.Assert(x1 <= x2a && x2a <= x0);

            Debug.Assert((x2a - 1) * (r2a - m1 * (x2a - 1)) <= n);
            Debug.Assert((x2b + 1) * (r2b - m1 * (x2b + 1)) <= n);

            // Determine intersection of L1 and L2.
            var x12 = (BigInteger)((r1 - r2a) / (m1 - m2));
            var y12 = (BigInteger)(r2a - m2 * x12);
            Debug.Assert(r2a - m2 * x12 == r1 - m1 * x12);
            if (x12 >= x01)
                return 0;

#if false
            if (diag)
                Console.WriteLine("m1 = {0,5}, m2 = {1,5}, m0 = {2,5}, x1 = {3,4}, x2 = {4,4}, x0 = {5,4}, dx = {6}", m1, m2, m0, x1, x2, x0, x0 - x1);
#endif

            var height = (x01 - x12) / m1.Denominator;
            var sum = (BigInteger)0;

            // Add the triangle defined L0, L1, and L2.
            var area = height * (height - 1) / 2;
            sum += area;
            if (diag)
                Console.WriteLine("corner: m2 = {0}, area = {1}", m2, area);

            if (r2a != r2b && x2b < x12)
            {
                var adjustment = x12 - x2b;
                sum -= adjustment;
                if (diag)
                    Console.WriteLine("corner: x1 = {0}, adjustment = {1}", x2b, adjustment);
            }

            // Process right region.
            sum += ProcessRegion(x2b, y2b, m2, r2b, x0, y0, m0, r0);

            // Process left region.
            sum += ProcessRegion(x1, y1, m1, r1, x2a, y2a, m2, r2a);

            return sum;
        }
    }
}
