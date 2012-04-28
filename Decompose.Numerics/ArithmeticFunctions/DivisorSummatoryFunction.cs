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

            // Count lattice points under the hyperbola x*y = n.
            var sum = (BigInteger)0;

            // Compute the range of values over which we will apply the
            // geometric algorithm.
            xmax = IntegerMath.FloorRoot(n, 2);
            xmin = IntegerMath.Min(IntegerMath.FloorRoot(n, 3) * minimumMultiplier, xmax);

            // Calculate the line tangent to the hyperbola at the x = sqrt(n).
            var m0 = (long)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m0 * x0;
            var width = x0 - xmin;
            Debug.Assert(r0 - m0 * x0 == y0);

            // Add the bottom rectangle.
            sum += (width + 1) * y0;

            // Add the isosceles right triangle corresponding to the initial
            // line L0 with -slope = 1.
            sum += width * (width + 1) / 2;

            // Process regions between tangent lines with integral slopes 1 & 2,
            // 2 & 3, etc. until we reach xmin.  This provides a first
            // approximation to the hyperbola and accounts for the majority
            // of the lattice points between xmin and max.  The remainder of
            // the points are computed by processing the regions bounded
            // by the two tangent lines and the hyperbola itself.
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

                // Handle left-overs.
                if (x1a < xmin)
                {
                    // Process the last few values above xmin as the number of
                    // points above the last L0.
                    for (var x = (BigInteger)xmin; x < x0; x++)
                        sum += n / x - (r0 - m0 * x);
                    break;
                }

                // Invariants:
                // The value before x1a along L1a is on or below the hyperbola.
                // The value after x1b along l2b is on or below the hyperbola.
                // The new slope is one greater than the old slope.
                Debug.Assert((x1a - 1) * (r1a - m1 * (x1a - 1)) <= n);
                Debug.Assert((x1b + 1) * (r1b - m1 * (x1b + 1)) <= n);
                Debug.Assert(m1 - m0 == 1);

                // Add the triangular wedge above the previous slope and below the new one
                // and bounded on the left by xmin.
                var x01a = (BigInteger)(r1a - r0);
                width = x01a - xmin;
                sum += width * (width + 1) / 2;

                // Account for a drop or rise from L1a to L1b.
                if (r1a != r1b && x1a < x01a)
                {
                    // Remove the old triangle and add the new triangle.
                    // The formula is (ow+dr)*(ow+dr+1)/2 - ow*(ow+1)/2.
                    var ow = x1a - x01a;
                    var dr = r1a - r1b;
                    sum += dr * (2 * ow + dr + 1) / 2;
                }

                // Determine intersection of L0 and L1b.
                var x01b = (BigInteger)(r1b - r0);
                var y01b = (BigInteger)(r0 - m0 * x01b);
                Debug.Assert(r0 - m0 * x01b == r1b - m1 * x01b);

                // Calculate width and height of parallelogram counting only lattice points.
                var w = (long)((y0 - y01b) + m1 * (x0 - x01b));
                var h = (long)((y1b - y01b) + m0 * (x1b - x01b));

                // Process the hyperbolic region bounded by L1b and L0.
                sum += ProcessRegion(w, h, m1, 1, m0, 1, x01b, y01b);

                // Advance to the next region.
                m0 = m1;
                x0 = x1a;
                y0 = y1a;
                r0 = r1a;
            }

            // Process values one up to xmin.
            for (var x = (BigInteger)1; x < xmin; x++)
                sum += n / x;

            // Account for sqrt(n) < x <= n using the Dirichlet hyperbola method.
            sum = 2 * sum - xmax * xmax;

            return sum;
        }

        private BigInteger ProcessRegion(long  w, long h, long m1n, long m1d, long m0n, long m0d, BigInteger x01, BigInteger y01)
        {
            // The hyperbola is defined by H(x, y): x*y = n.
            // Line L0 has slope m0 = -m0n/m0d.
            // Line L1 has slope m1 = -m1n/m1d.
            // Both lines pass through P01 = (x01, y01).
            // The region is a parallelogram with the left side bounded L1,
            // the bottom bounded by L0, with width w (along L0) and height h
            // (along L1).  The lower-left corner is P01 (the intersection of
            // L0 and L1) and represents (u, v) = (0, 0).
            // Both w and h are counted in terms of lattice points, not length.

            // For the purposes of counting, the lattice points on lines L0 and L1
            // have already been counted.

            // Note that m0d*m1n - m0n*m1d = 1 because
            // m0 and m1 are Farey neighbors, e.g. 1 & 2 or 3/2 & 2 or 8/5 & 5/3

            // The equations that define (u, v) in terms of (x, y) are:
            // u = m1d*(y-y01)+m1n*(x-x01)
            // v = m0d*(y-y01)+m0n*(x-x01)
 
            // And therefore the equations that define (x, y) in terms of (u, v) are:
            // x = x01-m1d*v+m0d*u
            // y = y01+m1n*v-m0n*u

            // Since all parameters are integers and m0d*m1n - m0n*m1d = 1,
            // every lattice point in (x, y) is a lattice point in (u, v)
            // and vice-versa.

            // Geometrically, the UV coordinate system is the composition
            // of a translation and two shear mappings.  The UV-based hyperbola
            // is essentially a "mini" hyperbola that resembles the full
            // hyperbola in that:
            // - The equation is still a hyperbola (although it is a quadratic in two variables)
            // - The endpoints of the curve are roughly tangent to the axes

            // We process the region by "lopping off" the maximal isosceles
            // right triangle in the lower-left corner and then processing
            // the two remaining "slivers" in the upper-left and lower-right,
            // which creates two smaller "micro" hyperbolas, which we process
            // recursively.

            // A line with -slope = 1 in UV-space has -slope = (m0n+m1n)/(m0d+m1d)
            // in XY-space.  We call this m2 and the line defining the third side
            // of the triangle as L2 contain point P2 tangent to the hyperbola.

            // This is all slightly complicated by the fact that diagonal that
            // defines the region that we "lop off" may be broken and shifted
            // up or down near the tangent point.  As a result we actually have
            // P2a and P2b and L2a and L2b.

            // Because we take a bite out of the middle, the sum of the sizes
            // of the two smaller regions will be less than the size of 
            // region we started with.

            // When we are in the region of the original hyperbola where
            // the curvature is roughly constant, the deformed hyperbola
            // will in fact resemble a circular arc.  For a true circular
            // arc with r=w=h, the new sub-regions will will be
            // (sqrt(2)-1)*r in size or about 41% of r.

            // As a result, each iteration reduces the problem space by about
            // a factor of two resulting in 1 + 2 + 4 + ... + sqrt(r) steps or O(sqrt(r)).
            // Since the sum of the sizes of the top-level regions is O(sqrt(n)),
            // This gives a O(n^(1/4)) algorithm for nearly constant curvature.

            // However, since the hyperbola is increasing non-circular for small
            // values of x, the subdivision is not nearly as beneficial (and
            // also not symmetric) so it is only worthwhile to use region
            // subdivision on regions where cubrt(n) < n < sqrt(n).

            // The sqrt(n) bound comes from symmetry and the Dirichlet
            // hyperbola method, which we also use.  The cubrt(n)
            // bound comes from the fact that the second deriviative H''(x)
            // exceeds one at (2n)^(1/3) ~= 1.26*cbrt(n).  Since we process
            // regions with adjacent integral slopes at the top level, by the
            // time we get to cbrt(n), the size of the region is at most
            // one, so we might as well process those values using the
            // naive approach of summing y = n/x.

            // Finally, at some point the region becomes small enough and we
            // can just count points under the hyperbola using whichever axis
            // is shorter.  This is quite a bit harder than computing y = n/x
            // because the transformations we are using result in a general
            // quadratic in two variables.  Nevertheless, with some
            // preliminary calculations, each value can be calculated with
            // a few additions, a square root and a division.

            // Sum the lattice points.
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
                    // Note that there are two solutions, one negative and one positive.
                    // We take the positive solution.

                    // We use the identities (a >= 0, b >= 0, c > 0; a, b, c elements of Z):
                    // floor(b*sqrt(a)/c) = floor(floor(sqrt(b^2*a))/c)
                    // floor(b*sqrt(a*c)/c) = floor(sqrt(b^2*a/c))
                    // to enable using integer arithmetic.

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
                    Debug.Assert(IntegerMath.Abs(v12a - v12b) >= 0 && IntegerMath.Abs(v12a - v12b) <= 1);

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

                // Any more regions to process?
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

            // Return the sum of lattice points in this region.
            return sum;
        }

        private long CountPoints(bool horizontal, long max, long m0n, long m0d, long m1n, long m1d, BigInteger x01, BigInteger y01)
        {
            // Count points under the hyperbola:
            // (x01 - m1d*v + m0d*u)*(y01 + m1n*v - m0n*u) = n
            // Horizontal: For u = 1 to max calculate v in terms of u.
            // vertical: For v = 1 to max calculate u in terms of v.
            // Note that there are two positive solutions and we
            // take the smaller of the two, the one nearest the axis.
            // By being frugal we can re-use most of the calculation
            // from the previous point.

            // We use the identity (a >= 0, b >= 0, c > 0; a, b, c elements of Z):
            // floor((b-sqrt(a)/c) = floor((b-ceiling(sqrt(a)))/c)
            // to enable using integer arithmetic.

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
