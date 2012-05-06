using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Integer = System.Numerics.BigInteger;

namespace Decompose.Numerics
{
    public class DivisorSummatoryFunction2
    {
        private struct Region
        {
            public Region(Integer w, Integer h, Integer a1, Integer b1, Integer a2, Integer b2, Integer x0, Integer y0)
            {
                this.w = w; this.h = h; this.a1 = a1; this.b1 = b1; this.a2 = a2; this.b2 = b2; this.x0 = x0; this.y0 = y0;
            }
            public Integer w;
            public Integer h;
            public Integer a1;
            public Integer b1;
            public Integer a2;
            public Integer b2;
            public Integer x0;
            public Integer y0;
        }

        private readonly Integer smallRegionCutoff = 10;
        private readonly Integer minimumMultiplier = 10;

        private bool diag;
        private Integer n;
        private Integer xmin;
        private Integer xmax;

        private Stack<Region> stack;

        public DivisorSummatoryFunction2(bool diag)
        {
            this.diag = diag;
            stack = new Stack<Region>();
        }

        public Integer Evaluate(Integer n)
        {
            this.n = n;

            // Count lattice points under the hyperbola x*y = n.
            var sum = (Integer)0;

            // Compute the range of values over which we will apply the
            // geometric algorithm.
            xmax = IntegerMath.FloorRoot(n, 2);
            xmin = IntegerMath.Min(IntegerMath.FloorRoot(n, 3) * minimumMultiplier, xmax);

            // Calculate the line tangent to the hyperbola at the x = sqrt(n).
            var m2 = (Integer)1;
            var x0 = xmax;
            var y0 = n / x0;
            var r0 = y0 + m2 * x0;
            var width = x0 - xmin;
            Debug.Assert(r0 - m2 * x0 == y0);

            // Add the bottom rectangle.
            sum += (width + 1) * y0;

            // Add the isosceles right triangle corresponding to the initial
            // line L2 with -slope = 1.
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
                var m1 = m2 + 1;
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
                    // points above the last L2.
                    for (var x = xmin; x < x0; x++)
                        sum += n / x - (r0 - m2 * x);
                    break;
                }

                // Invariants:
                // The value before x1a along L1a is on or below the hyperbola.
                // The value after x1b along L1b is on or below the hyperbola.
                // The new slope is one greater than the old slope.
                Debug.Assert((x1a - 1) * (r1a - m1 * (x1a - 1)) <= n);
                Debug.Assert((x1b + 1) * (r1b - m1 * (x1b + 1)) <= n);
                Debug.Assert(m1 - m2 == 1);

                // Add the triangular wedge above the previous slope and below the new one
                // and bounded on the left by xmin.
                var x01a = r1a - r0;
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

                // Determine intersection of L2 and L1b.
                var x01b = r1b - r0;
                var y01b = r0 - m2 * x01b;
                Debug.Assert(r0 - m2 * x01b == r1b - m1 * x01b);

                // Calculate width and height of parallelogram counting only lattice points.
                var w = (y0 - y01b) + m1 * (x0 - x01b);
                var h = (y1b - y01b) + m2 * (x1b - x01b);

                // Process the hyperbolic region bounded by L1b and L2.
                sum += ProcessRegion(w, h, m1, 1, m2, 1, x01b, y01b);

                // Advance to the next region.
                m2 = m1;
                x0 = x1a;
                y0 = y1a;
                r0 = r1a;
            }

            // Process values one up to xmin.
            for (var x = (Integer)1; x < xmin; x++)
                sum += n / x;

            // Account for sqrt(n) < x <= n using the Dirichlet hyperbola method.
            sum = 2 * sum - xmax * xmax;

            return sum;
        }

        private Integer ProcessRegion(Integer w, Integer h, Integer a1, Integer b1, Integer a2, Integer b2, Integer x0, Integer y0)
        {
            // The hyperbola is defined by H(x, y): x*y = n.
            // Line L1 has -slope m1 = a1/b1.
            // Line L2 has -slope m2 = a2/b2.
            // Both lines pass through P0 = (x0, y0).
            // The region is a parallelogram with the left side bounded L1,
            // the bottom bounded by L2, with width w (along L2) and height h
            // (along L1).  The lower-left corner is P0 (the intersection of
            // L2 and L1) and represents (u, v) = (0, 0).
            // Both w and h are counted in terms of lattice points, not length.

            // For the purposes of counting, the lattice points on lines L1 and L2
            // have already been counted.

            // Note that a1*b2 - b1*a2 = 1 because
            // m2 and m1 are Farey neighbors, e.g. 1 & 2 or 3/2 & 2 or 8/5 & 5/3

            // The equations that define (u, v) in terms of (x, y) are:
            // u = b1*(y-y0)+a1*(x-x0)
            // v = b2*(y-y0)+a2*(x-x0)

            // And therefore the equations that define (x, y) in terms of (u, v) are:
            // x = x0-b1*v+b2*u
            // y = y0+a1*v-a2*u

            // Since all parameters are integers and a1*b2 - b1*a2 = 1,
            // every lattice point in (x, y) is a lattice point in (u, v)
            // and vice-versa.

            // Geometrically, the UV coordinate system is the composition
            // of a translation and two shear mappings.  The UV-based hyperbola
            // is essentially a "mini" hyperbola that resembles the full
            // hyperbola in that:
            // - The equation is still a hyperbola (although it is now a quadratic in two variables)
            // - The endpoints of the curve are roughly tangent to the axes

            // We process the region by "lopping off" the maximal isosceles
            // right triangle in the lower-left corner and then processing
            // the two remaining "slivers" in the upper-left and lower-right,
            // which creates two smaller "micro" hyperbolas, which we then
            // process recursively.

            // When we are in the region of the original hyperbola where
            // the curvature is roughly constant, the deformed hyperbola
            // will in fact resemble a circular arc.

            // A line with -slope = 1 in UV-space has -slope = (a1+a2)/(b1+b2)
            // in XY-space.  We call this m3 and the line defining the third side
            // of the triangle as L3 containing point P3 tangent to the hyperbola.

            // This is all slightly complicated by the fact that diagonal that
            // defines the region that we "lop off" may be broken and shifted
            // up or down near the tangent point.  As a result we actually have
            // P3 and P4 and L3 and L4.

            // We can measure work in units of X because it is the short
            // axis and it ranges from cbrt(n) to sqrt(n).  If we did one
            // unit of work for each X coordinate we would have an O(sqrt(n))
            // algorithm.  But because there is only one lattice point on a
            // line with slope m per the denominator of m in X and because
            // the denominator of m roughly doubles for each subdivision,
            // there will be less than one unit of work for each unit of X.

            // As a result, each iteration reduces the work by about
            // a factor of two resulting in 1 + 2 + 4 + ... + sqrt(r) steps
            // or O(sqrt(r)).  Since the sum of the sizes of the top-level
            // regions is O(sqrt(n)), this gives a O(n^(1/4)) algorithm for
            // nearly constant curvature.

            // However, since the hyperbola is increasingly non-circular for small
            // values of x, the subdivision is not nearly as beneficial (and
            // also not symmetric) so it is only worthwhile to use region
            // subdivision on regions where cubrt(n) < n < sqrt(n).

            // The sqrt(n) bound comes from symmetry and the Dirichlet
            // hyperbola method (which we also use).  The cubrt(n)
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
            var sum = (Integer)0;

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
                    if ((b2 * w - b1 + x0) * (a1 - a2 * w + y0) <= n)
                    {
                        // Remove the first row.
                        sum += w;
                        x0 -= b1;
                        y0 += a1;
                        --h;
                        if (h == 0)
                            break;
                    }

                    // Check whether the point at (1, h) is inside the hyperbola.
                    if ((b2 - b1 * h + x0) * (a1 * h - a2 + y0) <= n)
                    {
                        // Remove the first column.
                        sum += h;
                        x0 += b2;
                        y0 -= a2;
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
                    Debug.Assert((b2 - b1 * h + x0) * (+a1 * h - a2 + y0) > n);
                    Debug.Assert((b2 * w - b1 + x0) * (a1 - a2 * w + y0) > n);
                    Debug.Assert(b2 * a1 - a2 * b1 == 1);

                    // Find the pair of points (u3, v3) and (u4, v4) below H(u,v) where:
                    // -dv/du at u=u3 >= 1
                    // -dv/du at u=u4 <= 1
                    // u4 = u3 + 1
                    // Specifically, solve:
                    // (x0 - b1*v + b2*u)*(y0 + a1*v - a2*u) = n at dv/du = -1
                    // and solve for the line tan = u + v tangent passing through that point.
                    // Then u3 = floor(u) and u4 = u3 + 1.
                    // Finally compute v3 and v4 from u3 and u4 using the tangent line
                    // which may result in a value too small by at most one.
                    // Note that there are two solutions, one negative and one positive.
                    // We take the positive solution.

                    // We use the identities (a >= 0, b >= 0, c > 0; a, b, c elements of Z):
                    // floor(b*sqrt(a)/c) = floor(floor(sqrt(b^2*a))/c)
                    // floor(b*sqrt(a*c)/c) = floor(sqrt(b^2*a/c))
                    // to enable using integer arithmetic.

                    // Formulas:
                    // m2nd = m2d*m2n, mxy1 = b1*y0+a1*x0, mxy2 = m2d*y0+m2n*x0
                    // u = floor((2*b1*m2n+1)*sqrt(m2nd*n)/m2nd-mxy1)
                    // v = floor(-u+2*sqrt(m2nd*n)-mxy2)
                    var c1 = a1 * x0 + b1 * y0;
                    var c2 = a2 * x0 + b2 * y0;
                    var a3 = a1 + a2;
                    var b3 = b1 + b2;
                    var c3 = c1 + c2;
                    var ab3 = a3 * b3;
                    var sqrtcoef = 2 * b1 * a3 + 1;
                    var tan = IntegerMath.FloorSquareRoot(2 * 2 * ab3 * n) - c3;
                    var u3 = IntegerMath.FloorSquareRoot(sqrtcoef * sqrtcoef * n / ab3) - c1;
                    var v3 = u3 != 0 ? tan - u3 : h;
                    var u4 = u3 < w ? u3 + 1 : w;
                    var v4 = tan - u4;

                    // Check for under-estimate of v3 and/or v4.
                    if (u3 != 0)
                    {
                        var v3plus = v3 + 1;
                        if ((b2 * u3 - b1 * v3plus + x0) * (a1 * v3plus - a2 * u3 + y0) <= n)
                            ++v3;
                    }
                    var v4plus = v4 + 1;
                    if ((b2 * u4 - b1 * v4plus + x0) * (a1 * v4plus - a2 * u4 + y0) <= n)
                        ++v4;

                    // Compute the V intercept of L3 and L4.  Since the lines are diagonal the intercept
                    // is the same on both U and V axes and v13 = u03 and v14 = u04.
                    var r3 = u3 + v3;
                    var r4 = u4 + v4;
                    Debug.Assert(IntegerMath.Abs(r3 - r4) >= 0 && IntegerMath.Abs(r3 - r4) <= 1);

                    // Count points horizontally or vertically if one axis collapses (or is below our cutoff)
                    // or if the triangle exceeds the bounds of the rectangle.
                    if (u3 <= smallRegionCutoff || v4 <= smallRegionCutoff || r3 > w || r4 > h)
                    {
                        if (h > w)
                            sum += CountPoints(true, w, a1, b1, c1, a2, b2, c2);
                        else
                            sum += CountPoints(false, h, a2, b2, c2, a1, b1, c1);
                        break;
                    }

                    // Add the triangle defined L2, L1, and smaller of L3 and L4.
                    var size = IntegerMath.Min(r3, r4);
                    sum += size * (size - 1) / 2;

                    // Adjust for the difference (if any) between L3 and L4.
                    if (r3 != r4)
                        sum += r3 > r4 ? u3 : v4;

                    // Push left region onto the stack.
                    stack.Push(new Region(u3, h - r3, a1, b1, a3, b3, x0 - b1 * r3, y0 + a1 * r3));

                    // Process right region iteratively (no change to a2 and b2).
                    w -= r4;
                    h = v4;
                    a1 = a3;
                    b1 = b3;
                    x0 = x0 + b2 * r4;
                    y0 = y0 - a2 * r4;
                }

                // Any more regions to process?
                if (stack.Count == 0)
                    break;

                // Pop a region off the stack for processing.
                var region = stack.Pop();
                w = region.w;
                h = region.h;
                a1 = region.a1;
                b1 = region.b1;
                a2 = region.a2;
                b2 = region.b2;
                x0 = region.x0;
                y0 = region.y0;
            }

            // Return the sum of lattice points in this region.
            return sum;
        }

        private Integer CountPoints(bool horizontal, Integer max, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
        {
            // Count points under the hyperbola:
            // (x0 - b1*v + b2*u)*(y0 + a1*v - a2*u) = n
            // Horizontal: For u = 1 to max calculate v in terms of u.
            // vertical: For v = 1 to max calculate u in terms of v.
            // Note that there are two positive solutions and we
            // take the smaller of the two, the one nearest the axis.
            // By being frugal we can re-use most of the calculation
            // from the previous point.

            // We use the identity (a >= 0, b >= 0, c > 0; a, b, c elements of Z):
            // floor((b-sqrt(a)/c) = floor((b-ceiling(sqrt(a)))/c)
            // to enable using integer arithmetic.

            // Formulas:
            // m0nd = b2*a2, m1nd = b1*a1, 
            // m01s = b2*a1+a2*b1, mxy0d = b2*y0-a2*x0,
            // mxy1d = a1*x0-b1*y0,
            // mxy0 = b2*y0+a2*x0, mxy1 = b1*y0+a1*x0
            // v = floor((-sqrt((u+mxy1)^2-4*m1nd*n)+m01s*u+mxy1d)/(2*m1nd))
            // u = floor((-sqrt((v+mxy0)^2-4*m0nd*n)+m01s*v+mxy0d)/(2*m0nd))
            var sum = (Integer)0;
            var coef = a1 * b2 + b1 * a2;
            var denom = 2 * a1 * b1;
            var a = c1 * c1 - 2 * denom * n;
            var b = c1 * coef;
            var da = 2 * c1 - 1;
            for (var i = (Integer)1; i <= max; i++)
            {
                da += 2;
                a += da;
                b += coef;
                sum += (b - IntegerMath.CeilingSquareRoot(a)) / denom;
            }
            return sum - max * c2;
        }

    }
}
