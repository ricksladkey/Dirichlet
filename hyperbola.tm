<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  <doc-data|<doc-title|A Successive Approximation Algorithm for Computing the
  Divisor Summatory Function>|<\doc-author-data|<author-name|Richard
  Sladkey>>
    \;
  </doc-author-data>>

  <\abstract>
    An algorithm is presented to compute the divisor summatory function in
    <math|O<around*|(|n<rsup|1/3+\<epsilon\>>|)>>time and
    <math|O<around*|(|<math-up|log >n|)>> space. \ The algorithm is
    elementary and uses a geometric approach of succesive approximation
    combined with coordinate transformation.
  </abstract>

  <section|Introduction>

  Consider the hyperbola from Dirichlet's divisor problem in an <math|x y>
  coordinate system:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|x,y|)>>|<cell|=>|<cell|x*<space|0.25spc>y<eq-number>>>|<row|<cell|>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  The number of lattice points under the hyperbola can be thought of as the
  number of combinations of positive integers <math|x> and <math|y> such that
  their product is less than or equal to <math|n>:

  <\equation>
    T<around*|(|n|)>=<big|sum><rsub|x,y:xy\<leqslant\>n>1
  </equation>

  \;

  As such, the hyperbola also represents the divisor summatory function, or
  the sum of the number of divisors of all numbers less than or equal to
  <math|n>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||x|\<nobracket\>>>1<eq-number>>>|<row|<cell|T<around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x=1><rsup|n>\<tau\><around*|(|x|)>>>>>
  </eqnarray*>

  One geometric algorithm is to sum columns of lattice points by choosing an
  axis and solving for the variable of the other axis:

  <\equation>
    T<around*|(|n|)>=<big|sum><rsup|n><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation>

  By using the symmetry of the hyperbola (and taking care to avoid double
  counting) we can do this even more efficiently:

  <\equation>
    T<around*|(|n|)>=2<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation>

  \ It will be convenient to parameterize this sum as:

  <\equation>
    S<around*|(|i,j|)>=<big|sum><rsup|j><rsub|x=i+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation>

  so that:

  <\equation>
    T<around*|(|n|)>=S<around*|(|0,n|)>=2*S<around*|(|0,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation>

  <section|Region Processing>

  Instead of considering all of the lattice points, let us for the moment
  focus on the subtask of counting the lattice points in a region bounded by
  two tangent lines and a segment of the hyperbola. If we can approximate the
  hyperbola by a series of tangent lines then the area below the lines is a
  simple polygon and can conceptually be calculated directly by decomposing
  the area into triangles. \ The region itself is crescent shaped with a
  corner, similar to the six pieces left over when subtracting an inscribed
  circle from a hexagon. \ We will now go about counting the lattice points
  in such a region.

  Define two lines whose slopes when negated have positive integral
  numerators and denominators:\ 

  <\eqnarray*>
    <tformat|<table|<row|<cell|-m<rsub|1>>|<cell|=>|<cell|<frac|a<rsub|1>|b<rsub|1>><eq-number>>>|<row|<cell|-m<rsub|2>>|<cell|=>|<cell|<frac|a<rsub|2>|b<rsub|2>><eq-number>>>>>
  </eqnarray*>

  The slopes are chosen to be Farey neighbors so that the determinant is
  unity:

  <\equation>
    <det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|a<rsub|2>>>|<row|<cell|b<rsub|1>>|<cell|b<rsub|2>>>>>>=a<rsub|1>*<space|0.25spc>b<rsub|2>-b<rsub|1>*<space|0.25spc>a<rsub|2>=1
  </equation>

  and the slopes are rational numbers which we require to be in lowest terms
  and so we can assume:

  <\eqnarray*>
    <tformat|<table|<row|<cell|gcd<around*|(|a<rsub|1>,b<rsub|1>|)>>|<cell|=>|<cell|1<eq-number>>>|<row|<cell|gcd<around*|(|a<rsub|2>,b<rsub|2>|)>>|<cell|=>|<cell|1<eq-number>>>>>
  </eqnarray*>

  Assume further that the lines intersect at point <math|P<rsub|0>>:

  <\equation>
    <around*|(|x<rsub|0>,y<rsub|0>|)>
  </equation>

  with <math|x<rsub|0>> and <math|y<rsub|0>> positive integers.

  Then lines <math|L<rsub|1>> and <math|L<rsub|2>> in point-slope form are:

  <\eqnarray*>
    <tformat|<table|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|1>|b<rsub|1>><eq-number>>>|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|2>|b<rsub|2>><eq-number>>>>>
  </eqnarray*>

  and converting to standard form:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y>|<cell|=>|<cell|x<rsub|0>*<space|0.25spc>a<rsub|1>+y<rsub|0>*<space|0.25spc>b<rsub|1><eq-number>>>|<row|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y>|<cell|=>|<cell|x<rsub|0>*<space|0.25spc>a<rsub|2>+y<rsub|0>*<space|0.25spc>b<rsub|2><eq-number>>>>>
  </eqnarray*>

  and defining:

  <\equation>
    c<rsub|i>=x<rsub|0>*<space|0.25spc>a<rsub|i>+y<rsub|0>*<space|0.25spc>b<rsub|i>
  </equation>

  we have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y>|<cell|=>|<cell|c<rsub|1><eq-number>>>|<row|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y>|<cell|=>|<cell|c<rsub|2><eq-number>>>>>
  </eqnarray*>

  Solving the definitions of <math|c<rsub|1>> and <math|c<rsub|2>> for
  <math|x<rsub|0>> and <math|y<rsub|0>> give:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|0>>|<cell|=>|<cell|c<rsub|1>*<space|0.25spc>b<rsub|2>-b<rsub|1>*<space|0.25spc>c<rsub|2><eq-number>>>|<row|<cell|y<rsub|0>>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>c<rsub|2>-c<rsub|1>*<space|0.25spc>a<rsub|2><eq-number>>>>>
  </eqnarray*>

  Define a <math|u v> coordinate system with an origin of <math|P<rsub|0>>,
  <math|L<rsub|1>> as the <math|v> axis and <math|L<rsub|2>> as the <math|u>
  axis and <math|u> and <math|v> increasing towards the hyperbola. Then the
  conversion from the <math|u v> coordinates to <math|x y> coordinates is
  given by:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|-b<rsub|1>*<space|0.25spc>v+b<rsub|2>*<space|0.25spc>u+x<rsub|0><eq-number>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>v-a<rsub|2>*<space|0.25spc>u+y<rsub|0><eq-number>>>>>
  </eqnarray*>

  Substituting for <math|x<rsub|0>> and <math|y<rsub|0>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)><eq-number>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)><eq-number>>>>>
  </eqnarray*>

  Solving these equations for <math|u> and <math|v> and substituting the
  determinant provides the conversion from <math|x y> coordinates to <math|u
  v> coordinates:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y-c<rsub|1><eq-number>>>|<row|<cell|v>|<cell|=>|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y-c<rsub|2><eq-number>>>>>
  </eqnarray*>

  Because all quantities are integers, the equations above mean that each
  <math|x y> lattice point corresponds to a <math|u v> lattice point and vice
  versa. \ As a result, we can choose to count lattice points in either
  <math|x y> coordinates or <math|u v> coordinates.

  Now we are ready to transform the hyperbola into the <math|u v> coordinate
  system by substituting for <math|x> and <math|y> which gives:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|u,v|)>>|<cell|=>|<cell|<around*|(|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>|)><around*|(|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>|)><eq-number>>>|<row|<cell|>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  Let us choose a point <math|P<rsub|1>> <math|<around*|(|0,h|)>> on the
  <math|v> axis and a point <math|P<rsub|2>> <math|<around*|(|w,0|)>> on the
  <math|u> axis such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|u<rsub|h>,h|)>>|<cell|=>|<cell|n>>|<row|<cell|H<around*|(|w,v<rsub|w>|)>>|<cell|=>|<cell|n>>|<row|<cell|0\<leqslant\>u<rsub|h>>|<cell|\<less\>>|<cell|1>>|<row|<cell|0\<leqslant\>v<rsub|w>>|<cell|\<less\>>|<cell|1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|h>|)>>|<cell|\<geqslant\>>|<cell|0>>|<row|<cell|-d
    u/d v<around*|(|v<rsub|w>|)>>|<cell|\<geqslant\>>|<cell|0>>>>
  </eqnarray*>

  or equivalently that the hyperbola is less than one unit away from the
  nearest axis at <math|P<rsub|1>> and <math|P<rsub|2>> and that the distance
  to the hyperbola increases as you approach the origin. \ There may be many
  points that satisfy these criteria.

  If <math|u<rsub|h> or v<rsub|w>> are one or greater, we can remove a
  rectangle from the left and/or bottom so that the conditions are
  satisified.

  With these constraints, the hyperbolic segment has the same basic shape as
  the full hyperbola: roughly tangent to the axes at the endpoints and
  monotonic in-between.

  We can now reformulate the number of lattice points in this region
  <math|R<rsub|> >as a function of the eight values that define it:

  <\equation>
    T<rsub|<rsub|R>>=T<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>
  </equation>

  For example, we could count lattice points in the region bounded by the
  <math|u> and <math|v> axes and <math|u=w> and <math|v=h> using brute force:

  <\equation>
    T<rsub|R>=<big|sum><rsub|u,v:H<around*|(|u,v|)>\<leqslant\>n>1
  </equation>

  More efficiently, if we had a formulas for <math|u> and <math|v> in terms
  each other, we could sum columns of lattice points:

  <\eqnarray*>
    <tformat|<table|<row|<cell|>|<cell|T<rsub|w>=<big|sum><rsup|w><rsub|u=1><around*|\<lfloor\>|V<around*|(|u|)>|\<rfloor\>>>|<cell|<eq-number>>>|<row|<cell|>|<cell|T<rsub|h>=<big|sum><rsub|v=1><rsup|h><around*|\<lfloor\>|U<around*|(|v|)>|\<rfloor\>>>|<cell|<eq-number>>>>>
  </eqnarray*>

  using whichever axis has fewer points (keeping in mind that it could be
  assymmetric). \ These summations are certain not to overcount because by
  our conditions <math|V<around*|(|u|)>\<less\>h> for
  <math|0\<less\>u\<leqslant\>w> and <math|U<around*|(|v|)>\<less\>w> for
  <math|0\<less\>v\<leqslant\>h>.

  And so:

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|T<rsub|w><eq-number>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|h>>>>>
  </eqnarray*>

  In fact we can derive formulas for <math|u> and <math|v> in terms of each
  other by solving <math|H<around*|(|u,v|)>=n> (which when expanded is a
  general quadratic in two variables) for <math|v> or <math|u>. The resulting
  explicit formulas for <math|v> in terms of <math|u> and <math|u> in terms
  of <math|v> are:

  <\eqnarray*>
    <tformat|<table|<row|<cell|V<around*|(|u|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-<sqrt|<around*|(|u+c<rsub|1>|)><rsup|2>-4*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>*<space|0.25spc>n>|2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>>-c<rsub|2><eq-number>>>|<row|<cell|U<around*|(|v|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-<sqrt|<around*|(|v+c<rsub|2>|)><rsup|2>-4*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>*<space|0.25spc>n>|2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>>-c<rsub|1><eq-number>>>>>
  </eqnarray*>

  (Note exchanging <math|u> for <math|v> results in the same formula with
  subscripts 1 and 2 exchanged.)

  As a result we can compute the number of lattice points within the region
  using a method similar to the method usually used for the hyperbola as a
  whole. \ Our goal, however, it to subdivide the region into two smaller
  regions and process them recursively, only using manual counting when the
  regions are small enough. \ To do so we need to remove an isosceles right
  triangle in the lower-left corner and what will be left are two sub-regions
  in the upper-left and lower-right.

  A diagonal with slope -1 in the UV coordinate system has a slope in the XY
  coordinate system that is the mediant of the slopes of lines
  <math|L<rsub|1>> and <math|L<rsub|2>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|-m<rsub|3>>|<cell|=>|<cell|<frac|a<rsub|1>+a<rsub|2>|b<rsub|1>+b<rsub|2>><eq-number>>>>>
  </eqnarray*>

  So let us define:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|3>>|<cell|=>|<cell|a<rsub|1>+a<rsub|2><eq-number>>>|<row|<cell|b<rsub|3>>|<cell|=>|<cell|b<rsub|1>+b<rsub|2><eq-number>>>>>
  </eqnarray*>

  Then differentiating <math|H<around*|(|u,v|)>=n> with respect to <math|u>
  and setting <math|d v/d u=-1> gives:

  <\equation>
    <around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>=<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>
  </equation>

  and the intersection of this line with <math|H<around*|(|u,v|)>=n> gives
  the point on the hyperbola where the slope is equal to -1:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|3>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|1><eq-number>>>|<row|<cell|v<rsub|3>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|2><eq-number>>>>>
  </eqnarray*>

  The equation of a line through this intersection and tangent to the
  hyperbola is then <math|u+v=u<rsub|T>+v<rsub|T>> which simplifies to:

  <\equation>
    u+v=2*<space|0.25spc><sqrt|a<rsub|3>*<space|0.25spc>b<rsub|3>*<space|0.25spc>n>-c<rsub|1>-c<rsub|2>
  </equation>

  Next we need to find the pair of lattice points <math|P<rsub|4>>
  <math|<around*|(|u<rsub|4,>v<rsub|4>|)>> and <math|P<rsub|5>>
  <math|<around*|(|u<rsub|5>,v<rsub|5>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|4>>|<cell|\<gtr\>>|<cell|0>>|<row|<cell|u<rsub|5>>|<cell|=>|<cell|u<rsub|4>+1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|4>|)>>|<cell|\<geqslant\>>|<cell|1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|5>|)>>|<cell|\<less\>>|<cell|1>>|<row|<cell|v<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|4>|)>|\<rfloor\>>>>|<row|<cell|v<rsub|5>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|5>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  The derivative conditions ensure that the diagonal rays with slope
  <math|-1> pointing outward from <math|P<rsub|4>> and <math|P<rsub|5>> do
  not intersect the hyperbola. \ Setting <math|u<rsub|4> =
  <around*|\<lfloor\>|u<rsub|3>|\<rfloor\>>> will satisfy the conditions as
  long as <math|u<rsub|4>\<neq\>0>.

  Let the point at which the ray from <math|P<rsub|4>> intersects the
  <math|v> axis be <math|P<rsub|6 ><around*|(|0,v<rsub|6>|)>> and the point
  at which the ray from <math|P<rsub|5>> intersects the <math|u> axis be
  <math|P<rsub|7 ><around*|(|u<rsub|7>,0|)>>. \ Then:

  <\eqnarray*>
    <tformat|<table|<row|<cell|v<rsub|6>>|<cell|=>|<cell|u<rsub|4>+v<rsub|4><eq-number>>>|<row|<cell|u<rsub|7>>|<cell|=>|<cell|u<rsub|5>+v<rsub|5><eq-number>>>>>
  </eqnarray*>

  Then the number of lattice points above the axes and inside the polygon
  defined by <math|P<rsub|0>,P<rsub|6>,P<rsub|4>,P<rsub|5>,P<rsub|7>> is:

  <\equation>
    \<Delta\><rsub|R>=<choice|<tformat|<table|<row|<cell|v<rsub|6>*<around*|(|v<rsub|6>-1|)>/2+u<rsub|4>>|<cell|v<rsub|6>\<less\>u<rsub|7>>>|<row|<cell|v<rsub|6>*<around*|(|v<rsub|6>-1|)>/2>|<cell|v<rsub|6>
    = u<rsub|7>>>|<row|<cell|u<rsub|7>*<around*|(|u<rsub|7>-1|)>/2+v<rsub|5>>|<cell|v<rsub|6>\<gtr\>u<rsub|7>>>>>>
  </equation>

  because counting on lattice diagonals starting at the origin we sum
  <math|1+2+\<ldots\>.+<around*|(|<math-up|min><around*|(|v<rsub|6>,u<rsub|7>|)>-1|)>>
  plus a partial diagonal if the polygon is not a triangle.

  Define region <math|R<rprime|'>> to be the sub-region with:

  <\eqnarray*>
    <tformat|<table|<row|<cell|P<rprime|'><rsub|1>>|<cell|=>|<cell|P<rsub|1>>>|<row|<cell|P<rprime|'><rsub|0>>|<cell|=>|<cell|P<rsub|4>>>|<row|<cell|P<rprime|'><rsub|2>>|<cell|=>|<cell|P<rsub|6>>>>>
  </eqnarray*>

  and the region <math|R<rprime|''>> to be the sub-region with:

  <\eqnarray*>
    <tformat|<table|<row|<cell|P<rprime|''><rsub|1>>|<cell|=>|<cell|P<rsub|5>>>|<row|<cell|P<rprime|''><rsub|0>>|<cell|=>|<cell|P<rsub|7>>>|<row|<cell|P<rprime|''><rsub|2>>|<cell|=>|<cell|P<rsub|2>>>>>
  </eqnarray*>

  Then the number of lattice points in the entire region is:

  <\equation>
    T<rsub|R>=\<Delta\><rsub|R>+T<rsub|R<rprime|'>>+T<rsub|R<rprime|''>>
  </equation>

  or:

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|\<Delta\><rsub|R><eq-number>>>|<row|<cell|>|<cell|+>|<cell|T<around*|(|u<rsub|4>,h-v<rsub|6>,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+v<rsub|6>|)>>>|<row|<cell|>|<cell|+>|<cell|T<around*|(|w-u<rsub|7>,v<rsub|5>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+u<rsub|7>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>>>>
  </eqnarray*>

  This recursive formula for the sum of the lattice points in a region in
  terms of the lattice points in its sub-regions allows us to use a divide
  and conquer approach to counting lattice points under the hyperbola.

  <section|Top Level Region Processing>

  Now let us return to the hyperbola as a whole. \ It should be clear that it
  is easy in <math|x y> coordinates to calculate <math|y> in terms of
  <math|x> by solving <math|H<around*|(|x,y|)>=n> for <math|y>:

  <\equation>
    Y<around*|(|x|)>=<frac|n|x>
  </equation>

  We know that we only need to sum lattice points under the hyperbola up to
  <math|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>. \ The point
  <math|<sqrt|n>> is in fact at the <math|x=y> axis of symmetry and so the
  slope at that point is exactly <math|-1>. \ The next integral slope occurs
  at <math|-2>, so our first (and largest) region occurs between slopes
  <math|-m<rsub|1>=2> and <math|-m<rsub|2>=1>. By processing adjacent
  integral slopes we will start in the middle and work our way back towards
  the origin.

  However, we cannot use the region method for the whole hyperbola because
  regions become smaller and smaller and eventually a region has a size
  <math|w+h \<leqslant\>1>. \ We can find the point where this occurs by
  taking the second derivative of <math|Y<around*|(|x|)>> with respect to
  <math|x> and setting it to unity. \ In other words, the point on the
  hyperbola where the rate of change in the slope exceeds one per lattice
  column, which is:

  <\equation>
    x=<sqrt|2*n|3>= 2<rsup|1/3>*n<rsup|1/3>\<approx\>1.26*n<rsup|1/3>
  </equation>

  As a result there is no value in region processing the first
  <math|O<around*|(|n<rsup|1/3>|)>> lattice columns so we ought to resort to
  the simple method:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|min>>|<cell|=>|<cell|<around*|\<lfloor\>|C*<sqrt|2*n|3>|\<rfloor\>><eq-number>>>|<row|<cell|T<rsub|A>>|<cell|=>|<cell|S<around*|(|0,x<rsub|min>|)>>>>>
  </eqnarray*>

  where <math|C\<geqslant\>1> is a constant to be chosen later.

  Because all slopes in this section of the algorithm are whole integers, we
  have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|i>>|<cell|=>|<cell|-m<rsub|i>>>|<row|<cell|b<rsub|i>>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  Assume that we have point <math|P<rsub|2>\<nocomma\>> and value
  <math|a<rsub|2>> from the previous iteration. \ For the first iteration we
  have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|2>>|<cell|=>|<cell|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>>|<row|<cell|y<rsub|2>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|2>|)>|\<rfloor\>>>>|<row|<cell|a<rsub|2>>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  For all iterations:

  <\equation*>
    a<rsub|1>=a<rsub|2>+1
  </equation*>

  The <math|x> coordinate of the point on the hyperbola where the slope is
  equal to <math|m<rsub|1>> can be found by taking the derivative of
  <math|Y<around*|(|x|)>> with respect to <math|x>, setting <math|d y/d
  x=m<rsub|1>>, and then solving for <math|x>:

  <\equation>
    x<rsub|3>=<sqrt|<frac|n|a<rsub|1>>>
  </equation>

  Similar to processing a region (but in <math|x y> coordinates), we now need
  two lattice points <math|P<rsub|4> <around*|(|x<rsub|4,>y<rsub|4>|)>> and
  <math|P<rsub|5> <around*|(|x<rsub|5>.y<rsub|5>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|4>>|<cell|\<gtr\>>|<cell|x<rsub|min>>>|<row|<cell|x<rsub|5>>|<cell|=>|<cell|x<rsub|4>+1>>|<row|<cell|-d
    y/d x<around*|(|x<rsub|4>|)>>|<cell|\<geqslant\>>|<cell|a<rsub|1>>>|<row|<cell|-d
    y /d x<around*|(|x<rsub|5>|)>>|<cell|\<less\>>|<cell|a<rsub|1>>>|<row|<cell|y<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|4>|)>|\<rfloor\>>>>|<row|<cell|y<rsub|5>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|5>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  To meet these conditions we can set <math|x<rsub|4>=<around*|\<lfloor\>|x<rsub|3>|\<rfloor\>>>
  unless <math|x<rsub|4>\<leqslant\>x<rsub|min>> in which case we can
  manually count the lattice columns between <math|x<rsub|min>> and
  <math|x<rsub|2>> and cease iterating.

  Then choosing <math|P<rsub|1>=P<rsub|5>> and calculating the necessary
  quantities:

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<rsub|1>>|<cell|=>|<cell|a<rsub|1*>x<rsub|5>+y<rsub|5>>>|<row|<cell|c<rsub|2>>|<cell|=>|<cell|a<rsub|2>*x<rsub|2>+y<rsub|2>>>|<row|<cell|w>|<cell|=>|<cell|a<rsub|1>*x<rsub|2>+y<rsub|2>-c<rsub|1>>>|<row|<cell|h>|<cell|=>|<cell|a<rsub|2>*x<rsub|5>+y<rsub|5>-c<rsub|2>>>>>
  </eqnarray*>

  we can now count lattice points using region processing:\ 

  <\equation>
    T<rsub|R>=T<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>
  </equation>

  Then we may advance to the next region by setting:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rprime|'><rsub|2>>|<cell|=>|<cell|x<rsub|4>>>|<row|<cell|y<rprime|'><rsub|2>>|<cell|=>|<cell|y<rsub|4>>>|<row|<cell|a<rprime|'><rsub|2>>|<cell|=>|<cell|a<rsub|1>>>>>
  </eqnarray*>

  <section|Algorithm>

  Introducing intermediate quantities and functions in support of integer
  arithmetic:

  <\eqnarray*>
    <tformat|<table|<row|<cell|d<rsub|i>>|<cell|=>|<cell|a<rsub|i>*<space|0.25spc>b<rsub|i><eq-number>>>|<row|<cell|f<rsub|i,j>>|<cell|=>|<cell|a<rsub|i>*<space|0.25spc>b<rsub|j>+b<rsub|i>*<space|0.25spc>a<rsub|j><eq-number>>>|<row|<cell|g<rsub|i,j><around*|(|w|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|f<rsub|i,j>*<space|0.25spc><around*|(|w+c<rsub|i>|)>-<around*|\<lceil\>|<sqrt|<around*|(|w+c<rsub|i>|)><rsup|2>-4*<space|0.25spc>d<rsub|i>*<space|0.25spc>n>|\<rceil\>>|2*<space|0.25spc>d<rsub|i>>|\<rfloor\>>-c<rsub|j><eq-number>>>>>
  </eqnarray*>

  \;

  Formulas for horizontal and vertical summation:

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|w>>|<cell|=>|<cell|<big|sum><rsub|u=1><rsup|w>g<rsub|1,2><around*|(|u<rsub|>|)><eq-number>>>|<row|<cell|S<rsub|h>>|<cell|=>|<cell|<big|sum><rsub|v=1><rsup|h>g<rsub|2,1><around*|(|v<rsub|>|)><eq-number>>>>>
  </eqnarray*>

  Formulas for <math|P<rsub|4>> and <math|P<rsub|5>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|<space|0.25spc><sqrt|<around*|\<lfloor\>|<frac|<around*|(|f<rsub|1,2>+2*<space|0.25spc>d<rsub|1>|)><rsup|2>n|d<rsub|3>>|\<rfloor\>>>|\<rfloor\>>-c<rsub|1><eq-number>>>|<row|<cell|v<rsub|4>>|<cell|=>|<cell|g<rsub|1,2><around*|(|u<rsub|4>|)><eq-number>>>|<row|<cell|u<rsub|5>>|<cell|=>|<cell|u<rsub|4>+1<eq-number>>>|<row|<cell|v<rsub|5>>|<cell|=>|<cell|g<rsub|1,2><around*|(|u<rsub|5>|)><eq-number>>>>>
  </eqnarray*>

  <section|Time and Space Complexity>

  Now we present a heuristic argument for the runtime behavior of the
  algorithm. \ Because we can use an explicit summation, the number of
  operations needed to count the number of lattice points in region <math|R>
  is at most:

  <\equation>
    A=<math-up|min><around*|(|w,h|)>
  </equation>

  Likewise, the number of operations needed to count the number of lattice
  points in region <math|R<rprime|'>> is at most:

  <\equation>
    B=<math-up|min><around*|(|u<rsub|4>,h-v<rsub|6>|)>
  </equation>

  If <math|w\<approx\>h> and we can approximate the hyperbolic segment by a
  circular arc, then:

  <\eqnarray*>
    <tformat|<table|<row|<cell|A>|<cell|\<approx\>>|<cell|w>>|<row|<cell|u<rsub|4>>|<cell|\<approx\>>|<cell|w*<around*|(|1-<frac|1|<sqrt|2>>|)>>>|<row|<cell|h-v<rsub|6>>|<cell|\<approx\>>|<cell|w*<around*|(|<sqrt|2>-1|)>>>|<row|<cell|B>|<cell|\<approx\>>|<cell|w*<around*|(|1-<frac|1|<sqrt|2>>|)>>>|<row|<cell|A/B>|<cell|\<approx\>>|<cell|2+<sqrt|2><eq-number>>>>>
  </eqnarray*>

  which would result in a recursion depth of approximately:

  <\equation>
    E=<frac|<math-up|log><rsub|2> w|<math-up|log><rsub|2><around*|(|2+<sqrt|2>|)>>
  </equation>

  levels or approximately:

  <\equation>
    N<rsub|R>=2<rsup|E>=O<around*|(|w<rsup|1/log<rsub|2><around*|(|2+<sqrt|2>|)>>|)>
  </equation>

  regions total in the course of processing region <math|R> with width
  <math|w>. \ Because the sum of widths of all regions is of size
  <math|O<around*|(|<sqrt|n>|)>>, the total number of regions for the whole
  hyperbola is:

  \;

  <\equation>
    N<rsub|>=O<around*|(|<sqrt|n><rsup|1/log<rsub|2><around*|(|2+<sqrt|2>|)>>|)>\<approx\>O<around*|(|n<rsup|0.282>|)>\<ll\>O<around*|(|n<rsup|<rsup|1/3>>|)>
  </equation>

  \;
</body>

<\references>
  <\collection>
    <associate|abc|<tuple|6|?>>
    <associate|auto-1|<tuple|1|?>>
    <associate|auto-2|<tuple|2|?>>
    <associate|auto-3|<tuple|3|?>>
    <associate|auto-4|<tuple|4|?>>
    <associate|auto-5|<tuple|5|?>>
  </collection>
</references>