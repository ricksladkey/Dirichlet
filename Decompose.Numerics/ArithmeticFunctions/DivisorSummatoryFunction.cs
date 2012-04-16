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

        private BigInteger n;
        private BigInteger xmin;
        private BigInteger xmax;

        public BigInteger Evaluate(BigInteger n)
        {
            this.n = n;

            var sum = (BigInteger)0;

            xmin = IntegerMath.FloorRoot(n, 3);
            xmax = IntegerMath.FloorRoot(n, 2);

            var m0 = (BigInteger)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m0 * x0;
            var width = x0 - xmin;
            Debug.Assert(-m0 * x0 + r0 == y0);

            // Add the bottom rectangle.
            sum += (width + 1) * x0;

            // Add the isosceles right triangle from the initial skew.
            sum += width * (width + 1) / 2;

            while (true)
            {
                var m1 = m0 + 1;
                var x1 = IntegerMath.FloorRoot(n / m1, 2);
                var y1 = n / x1;
                var r1 = y1 + m1 * x1;
                Debug.Assert(-m1 * x1 + r1 == y1);

                if (x1 < xmin)
                {
                    for (var x = (BigInteger)xmin; x < x0; x++)
                    {
                        var delta = n / x - Rational.Floor(-m0 * x + r0);
                        sum += delta;
                    }
                    break;
                }

                // Add the triangular wedge above the previous slope and below the new one.
                var xintersect = (BigInteger)((r1 - r0) / (m1 - m0));
                width = xintersect - xmin;
                sum += width * (width + 1) / 2;

                sum += ProcessRegion(x1, y1, m1, r1, x0, y0, m0, r0);

                m0 = m1;
                x0 = x1;
                y0 = y1;
                r0 = r1;
            }

            // Process values one thru xmin.
            for (var x = (BigInteger)1; x < xmin; x++)
                sum += n / x;

            // Account for the first octant.
            sum = 2 * sum - xmax * xmax;

            return sum;
        }

        private BigInteger ProcessRegion(BigInteger x1, BigInteger y1, Rational m1, Rational r1, BigInteger x0, BigInteger y0, Rational m0, Rational r0)
        {
            if (x1 == x0)
                return 0;
            Debug.Assert(x1 < x0);
            Debug.Assert(y1 > y0);
            Debug.Assert(r1 > r0);
            Debug.Assert(m1 > m0);

            // Sub-divide the new hyperbolic region.
            var sum = (BigInteger)0;

            var xintersect = (BigInteger)((r1 - r0) / (m1 - m0));
            var yintersect = (BigInteger)(-m0 * xintersect + r0);
            Debug.Assert(-m0 * xintersect + r0 == -m1 * xintersect + r1);
            if (xintersect < x1 || xintersect > x0)
                return 0;

            // L2 is the line with the mediant of the slopes of L0 and L1
            // passing through the point on or below the hyperbola nearest that slope.
            var m2 = Rational.Mediant(m0, m1);
            var x2 = (BigInteger)IntegerMath.FloorRoot(n / m2, 2);
            var y2 = n / x2;
            var r2 = y2 + m2 * x2;
            Debug.Assert(x1 <= x2 && x2 <= x0);

#if false
            Console.WriteLine("m1 = {0,5}, m2 = {1,5}, m0 = {2,5}, x1 = {3,4}, x2 = {4,4}, x0 = {5,4}, dx = {6}", m1, m2, m0, x1, x2, x0, x0 - x1);
#endif

            // Determine the coordinates of the inscribed parallelogram.
            var xleft = (BigInteger)((y2 + m0 * x2 - r1) / (m0 - m1));
            var yleft = (BigInteger)(-m1 * xleft + r1);
            var height = (xintersect - xleft) / m1.Denominator;
            var width = x2 - xleft;
            //Debug.Assert(x1 < xleft && xleft < x0);
            Debug.Assert(xleft == x2 && yleft == y2 || new Rational(yleft - y2, xleft - x2) == -m0);


            if (width > 0 && height > 0)
            {
                // Add the triangle defined L0, L1, and L2.
                sum += 2 * width * height;
#if true
                // Process right region.
                sum += ProcessRegion(x2, y2, m2, r2, x0, y0, m0, r0);
#endif

#if true
                // Process left region.
                sum += ProcessRegion(x1, y1, m1, r1, x2, y2, m2, r2);
#endif
            }

            return sum;
        }
    }
}
