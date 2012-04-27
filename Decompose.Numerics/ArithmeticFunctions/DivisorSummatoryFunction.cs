using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

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

            xmax = IntegerMath.FloorRoot(n, 2);
            xmin = IntegerMath.Min(IntegerMath.FloorRoot(n, 3) * minimumMultiplier, xmax);

            var m0 = (long)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m0 * x0;
            var width = x0 - xmin;
            Debug.Assert(r0 - m0 * x0 == y0);

            // Add the bottom rectangle.
            sum += (width + 1) * y0;

            // Add the isosceles right triangle from the initial skew.
            sum += width * (width + 1) / 2;

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
                        sum += n / x - (r0 - m0 * x);
                    break;
                }

                Debug.Assert((x1a - 1) * (r1a - m1 * (x1a - 1)) <= n);
                Debug.Assert((x1b + 1) * (r1b - m1 * (x1b + 1)) <= n);

                // Add the triangular wedge above the previous slope and below the new one.
                var xintersect = (BigInteger)((r1a - r0) / (m1 - m0));
                width = xintersect - xmin;
                sum += width * (width + 1) / 2;

                if (r1a != r1b && x1a < xintersect)
                {
                    // Remove the old triangle and add the new triangle.
                    var ow = x1a - xintersect;
                    var dr = r1a - r1b;
                    sum += dr * (2 * ow + dr + 1) / 2; // (ow+dr)*(ow+dr+1)/2 - ow*(ow+1)/2
                }

                // Determine intersection of L0 and L1.
                var x01 = (BigInteger)((r1b - r0) / (m1 - m0));
                var y01 = (BigInteger)(r0 - m0 * x01);
                Debug.Assert(r0 - m0 * x01 == r1b - m1 * x01);

                // Calculate width and height of parallelogram.
                var w = (long)((y0 - y01) + m1 * (x0 - x01));
                var h = (long)((y1b - y01) + m0 * (x1b - x01));

                // Process the hyperbolic sub-region.
                sum += ProcessRegion(w, h, m1, 1, m0, 1, x01, y01);

                // Advance to the next region.
                m0 = m1;
                x0 = x1a;
                y0 = y1a;
                r0 = r1a;
            }

            // Process values one up to xmin.
            for (var x = (BigInteger)1; x < xmin; x++)
            {
                sum += n / x;
            }

            // Account for the first octant.
            sum = 2 * sum - xmax * xmax;

            return sum;
        }

        private BigInteger ProcessRegion(long  w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01)
        {
            // The hyperbola is defined by H(x, y): x * y = n.
            // Line L0 has slope m0 = -m0n/m0d.
            // Line L1 has slope m1 = -m1n/m1d.
            // Both lines pass through P01 = (x01, y01).
            // The region is a parallelogram with the left side bounded L1,
            // the bottom bounded by L0, with width w and height h.
            // The lower-left corner is P01 and represents (u, v) = (0, 0).
            // Both w and h are counted in terms of lattice points, not length.

            // Note that m0d*m1n - m0n*m1d = 1 because
            // m0 and m1 are Farey neighbors.

            // The equations that define (u, v) in terms of (x, y) are:
            // u = m1d*(y-y01)+m1n*(x-x01)
            // v = m0d*(y-y01)+m0n*(x-x01)
 
            // Conversely, the equations that define (x, y) in terms of (u, v) are:
            // x = x01-m1d*v+m0d*u
            // y = y01+m1n*v-m0n*u

            // Since all parameters are integers and m0d*m1n - m0n*m1d = 1,
            // every lattice point in (x, y) is a lattice point in (u, v)
            // and vice-versa.

            // Geometrically, the UV coordinate system is the composition
            // of a translation and two shear mappings.

            // Sub-divide the new hyperbolic region and sum the lattice points.
            var sum = (BigInteger)0;

            // Process regions on the stack.
            while (true)
            {
                // Process regions iteratively.
                while (true)
                {
                    // Nothing left process.
                    if (w <= 0 || h <= 0)
                        break;

                    // Check whether the point at (w, 1) is inside the hyperbola.
                    if ((m0d * w - m1d + x01) * (m1n - m0n * w + y01) <= n)
                    {
                        // Remove the first row.
                        sum += w;
                        x01 -= m1d;
                        y01 += m1n;
                        --h;
                        if (h == 0)
                            break;
                    }

                    // Check whether the point at (1, h) is inside the hyperbola.
                    if ((m0d - m1d * h + x01) * (m1n * h - m0n + y01) <= n)
                    {
                        // Remove the first column.
                        sum += h;
                        x01 += m0d;
                        y01 -= m0n;
                        --w;
                        if (w == 0)
                            break;
                    }

                    // Invariants for the remainder of the processing of the region:
                    // H(u,v) at v=h, 0 <= u < 1
                    // H(u,v) at u=w, 0 <= v < 1
                    // -du/dv at v=h >= 0
                    // -dv/du at u=w >= 0
                    // In other words: the hyperbola is less than one unit away
                    // from the axis at P0 and P1 and the distance from the axis
                    // to the hyperbola increases monotonically as you approach
                    // (u, v) = (0, 0).
                    Debug.Assert((m0d - m1d * h + x01) * (+m1n * h - m0n + y01) > n);
                    Debug.Assert((m0d * w - m1d + x01) * (m1n -  m0n * w + y01) > n);
                    Debug.Assert(m0d * m1n - m0n * m1d == 1);

                    // Find the pair of points (u2a, v2a) and (u2b, v2b) below H(u,v) where:
                    // -dv/du at u=u2a >= 1
                    // -dv/du at u=u2b <= 1
                    // u2b = u2a + 1
                    // Specifically, solve:
                    // (x01 - m1d*v + m0d*u)*(y01 + m1n*v - m0n*u) = n at dv/du = -1
                    // and solve for the line tan = u + v tangent passing through that point.
                    // Then u2a = floor(u) and u2b = u2a + 1.
                    // Finally compute v2a and v2b from u2a and u2b using the tangent line
                    // which may result in a value too small by at most one.

                    // m2nd = m2d*m2n, mxy1 = m1d*y01+m1n*x01, mxy2 = m2d*y01+m2n*x01
                    // u = floor((2*m1d*m2n+1)*sqrt(m2nd*n)/m2nd-mxy1)
                    // v = floor(-u+2*sqrt(m2nd*n)-mxy2)
                    var m2n = m0n + m1n;
                    var m2d = m0d + m1d;
                    var m2nd = m2n * m2d;
                    var mxy1 = m1n * x01 + m1d * y01;
                    var mxy2 = m2n * x01 + m2d * y01;
                    var sqrtcoef = 2 * m1d * m2n + 1;
                    var tan = (long)(IntegerMath.FloorSquareRoot(2 * 2 * m2nd * n) - mxy2);
                    var u2a = (long)(IntegerMath.FloorSquareRoot(sqrtcoef * sqrtcoef * n / m2nd) - mxy1);
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

                    // Compute the V intercept of L2a and L2b.  Since the lines are diagonal the intercept
                    // is the same on both U and V axes and v12a = u02a and v12b = u02b.
                    var v12a = u2a + v2a;
                    var v12b = u2b + v2b;

                    // Count points horizontally or vertically if one axis collapses (or is below our cutoff)
                    // or if the triangle exceeds the bounds of the rectangle.
                    if (u2a <= smallRegionCutoff || v2b <= smallRegionCutoff || IntegerMath.Max(v12a, v12b) > IntegerMath.Min(w, h))
                    {
                        if (h > w)
                            sum += CountPoints(true, w, m0n, m0d, m1n, m1d, x01, y01);
                        else
                            sum += CountPoints(false, h, m1n, m1d, m0n, m0d, x01, y01);
                        break;
                    }

                    // Add the triangle defined L0, L1, and smaller of L2a and L2b.
                    var v12 = IntegerMath.Min(v12a, v12b);
                    sum += v12 * (v12 - 1) / 2;

                    // Adjust for the difference (if any) between L2a and L2b.
                    if (v12a != v12b)
                        sum += v12a > v12b ? u2a : v2b;

                    // Push left region onto the stack.
                    stack.Push(new Region(u2a, h - v12a, m1n, m1d, m2n, m2d, x01 - m1d * v12a, y01 + m1n * v12a));

                    // Process right region iteratively (no change to m0n and m0d).
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
                w = region.w;
                h = region.h;
                m1n = region.m1n;
                m1d = region.m1d;
                m0n = region.m0n;
                m0d = region.m0d;
                x01 = region.x01;
                y01 = region.y01;
            }

            return sum;
        }

        private long CountPoints(bool horizontal, long max, long m0n, long m0d, long m1n, long m1d, BigInteger x01, BigInteger y01)
        {
            // Count points under the hyperbola:
            // (x01 - m1d*v + m0d*u)*(y01 + m1n*v - m0n*u) = n
            // Horizontal: For u = 1 to max calculate v in terms of u.
            // vertical: For v = 1 to max calculate u in terms of v.

            // m0nd = m0d*m0n, m1nd = m1d*m1n, 
            // m01s = m0d*m1n+m0n*m1d, mxy0d = m0d*y01-m0n*x01,
            // mxy1d = m1n*x01-m1d*y01,
            // mxy0 = m0d*y01+m0n*x01, mxy1 = m1d*y01+m1n*x01
            // v = floor((-sqrt((u+mxy1)^2-4*m1nd*n)+m01s*u+mxy1d)/(2*m1nd))
            // u = floor((-sqrt((v+mxy0)^2-4*m0nd*n)+m01s*v+mxy0d)/(2*m0nd))
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
                sum += (long)((b - IntegerMath.CeilingSquareRoot(a)) / denom);
            }
            return sum;
        }

    }
}
