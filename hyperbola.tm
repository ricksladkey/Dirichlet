<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  Hyperbola in XY coordinate system:

  <\equation*>
    H<around*|(|x,y|)>=x*<space|0.25spc>y=n
  </equation*>

  The number of lattice points under the hyperbola can be thought of as the
  number of combinations of positive integers <math|x> and <math|y> such that
  there product is less than or equal to <math|n>:

  <\equation*>
    S<around*|(|n|)>=<big|sum><rsub|x,y:xy\<leqslant\>n>1
  </equation*>

  We can also sum columns of lattice points by choosing an axis and solving
  for that variable:

  <\equation*>
    S<around*|(|n|)>=<big|sum><rsup|n><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation*>

  By using the symmetry of the hyperbola (and taking care to avoid double
  counting) we can do this even more efficiently:

  <\equation*>
    S<around*|(|n|)>=2<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation*>

  \ It will be convenient to parameterize this sum as:

  <\equation*>
    S<rprime|'><around*|(|i,j|)>=<big|sum><rsup|j><rsub|x=i+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation*>

  so that:

  <\equation*>
    S<around*|(|n|)>=S<rprime|'><around*|(|0,n|)>=2*S<rprime|'><around*|(|0,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation*>

  Instead of focusing on the whole calculation, let us for the moment focus
  on the subtask of counting the lattice points in a region bounded by two
  tangent lines and a segment of the hyperbola. If we can approximate the
  hyperbola by a series of tangent lines then the area below the lines is a
  simple polygon and can be calculated directly by decomposing the area into
  triangles. \ The region itself is crescent shaped with a corner, similar to
  the six pieces left over when subtracting an inscribed circle from a
  hexagon. \ We will now go about counting the lattice points in such a
  region.

  Define two lines whose slopes when negated have positive integral
  numerators and denominators:\ 

  <\eqnarray*>
    <tformat|<table|<row|<cell|-m<rsub|1>>|<cell|=>|<cell|<frac|a<rsub|1>|b<rsub|1>>>>|<row|<cell|-m<rsub|2>>|<cell|=>|<cell|<frac|a<rsub|2>|b<rsub|2>>>>>>
  </eqnarray*>

  The slopes are chosen to be Farey neighbors so the determinant is unity:

  <\equation*>
    a<rsub|1>*<space|0.25spc>b<rsub|2>-b<rsub|1>*<space|0.25spc>a<rsub|2>=1
  </equation*>

  Assume the lines intersect at point P0:

  <\equation*>
    <around*|(|x<rsub|0>,y<rsub|0>|)>
  </equation*>

  with <math|x<rsub|0>> and <math|y<rsub|0>> positive integers.

  Lines L1 and L2 in point-slope form:

  <\eqnarray*>
    <tformat|<table|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|1>|b<rsub|1>>>>|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|2>|b<rsub|2>>>>>>
  </eqnarray*>

  Converting to standard form:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y>|<cell|=>|<cell|x<rsub|0>*<space|0.25spc>a<rsub|1>+y<rsub|0>*<space|0.25spc>b<rsub|1>>>|<row|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y>|<cell|=>|<cell|x<rsub|0>*<space|0.25spc>a<rsub|2>+y<rsub|0>*<space|0.25spc>b<rsub|2>>>>>
  </eqnarray*>

  and defining:

  <\equation*>
    c<rsub|i>=x<rsub|0>*<space|0.25spc>a<rsub|i>+y<rsub|0>*<space|0.25spc>b<rsub|i>
  </equation*>

  we have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y>|<cell|=>|<cell|c<rsub|1>>>|<row|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y>|<cell|=>|<cell|c<rsub|2>>>>>
  </eqnarray*>

  Solving the definitions of <math|c<rsub|1>> and <math|c<rsub|2>> for
  <math|x<rsub|0>> and <math|y<rsub|0>> give:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|0>>|<cell|=>|<cell|c<rsub|1>*<space|0.25spc>b<rsub|2>-b<rsub|1>*<space|0.25spc>c<rsub|2>>>|<row|<cell|y<rsub|0>>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>c<rsub|2>-c<rsub|1>*<space|0.25spc>a<rsub|2>>>>>
  </eqnarray*>

  Define a UV coordinate system with an origin of P0, L2 as the <math|u> axis
  and L1 as the <math|v> axis and <math|u> and <math|v> increasing towards
  the hyperbola. Then the conversion from the UV coordinates to XY
  coordinates is given by:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|-b<rsub|1>*<space|0.25spc>v+b<rsub|2>*<space|0.25spc>u+x<rsub|0>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>v-a<rsub|2>*<space|0.25spc>u+y<rsub|0>>>>>
  </eqnarray*>

  Substituting for <math|x<rsub|0>> and <math|y<rsub|0>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>>>>>
  </eqnarray*>

  Solving these equations for <math|u> and <math|v> and substituting the
  determinant provides the conversion from XY coordinates to UV coordinates:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y-c<rsub|1>>>|<row|<cell|v>|<cell|=>|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y-c<rsub|2>>>>>
  </eqnarray*>

  Now we are ready to transform the hyperbola into the UV coordinate system
  by substituting for <math|x> and <math|y> which gives:

  <\equation*>
    H<around*|(|u,v|)>=*<around*|(|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>|)><around*|(|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>|)>=n
  </equation*>

  Now choose a point P1 <math|<around*|(|0,h|)>> on the <math|v> axis and a
  point P2 <math|<around*|(|w,0|)>> on the <math|u> axis such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|u<rsub|h>,h|)>>|<cell|=>|<cell|n>>|<row|<cell|H<around*|(|w,v<rsub|w>|)>>|<cell|=>|<cell|n>>|<row|<cell|0\<leqslant\>u<rsub|h>>|<cell|\<less\>>|<cell|1>>|<row|<cell|0\<leqslant\>v<rsub|w>>|<cell|\<less\>>|<cell|1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|h>|)>>|<cell|\<geqslant\>>|<cell|0>>|<row|<cell|-d
    u/d v<around*|(|v<rsub|w>|)>>|<cell|\<geqslant\>>|<cell|0>>>>
  </eqnarray*>

  or equivalently that the hyperbola is less than one unit away from the
  nearest axis at P1 and P2 and that the distance to the hyperbola increases
  as you approach the origin. \ There may be many points that satisfy these
  criteria.

  With these constraints, the hyperbolic segment has the same basic shape as
  the full hyperbola: roughly tangent to the axes at the endpoints and curved
  inbetween.

  With this machinery in place, we could count lattice points in the region
  bounded by the <math|u> and <math|v> axes and <math|u=w> and <math|v=h>
  using brute force:

  <\equation*>
    S=<big|sum><rsub|u,v:H<around*|(|u,v|)>\<leqslant\>n>1
  </equation*>

  \ More efficiently, if we had a formulas for <math|u> and <math|v> in terms
  each other, we could sum columns of lattice points:

  <\eqnarray*>
    <tformat|<table|<row|<cell|>|<cell|S=<big|sum><rsup|w><rsub|u=1><around*|\<lfloor\>|v<around*|(|u|)>|\<rfloor\>>>|<cell|>>|<row|<cell|>|<cell|S=<big|sum><rsub|v=1><rsup|h><around*|\<lfloor\>|u<around*|(|v|)>|\<rfloor\>>>|<cell|>>>>
  </eqnarray*>

  using whichever axis has fewer points, keeping in mind that it could be
  assymetric.

  In fact we can derive such a formula by solving <math|H<around*|(|u,v|)>=n>
  (which is a quadratic in two variables) for <math|v> or <math|u>. Then
  explicit formulas for <math|v> in terms of <math|u> and <math|u> in terms
  of <math|v> are:

  <\eqnarray*>
    <tformat|<table|<row|<cell|V<around*|(|u|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-<sqrt|<around*|(|u+c<rsub|1>|)><rsup|2>-4*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>*<space|0.25spc>n>|2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>>-c<rsub|2>>>|<row|<cell|U<around*|(|v|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-<sqrt|<around*|(|v+c<rsub|2>|)><rsup|2>-4*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>*<space|0.25spc>n>|2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>>-c<rsub|1>>>>>
  </eqnarray*>

  (Note exchanging <math|u> for <math|v> results in the same formula with
  subscripts 1 and 2 exchanged.)

  As a result we can compute the number of lattice points within the region
  using a method similar to the regular method for the hyperbola as a whole.
  \ Our goal, however, it to subdivide the region into two smaller regions
  and process them recursively and only use manual counting when the regions
  are small enough. \ To do so we need to remove a triangle in the lower-left
  corner and what will be left are two sub-regions in the upper-left and
  lower-right.

  A diagonal with slope -1 in the UV coordinate system has a slope in the XY
  coordinate system that is the mediant of the slopes of lines L1 and L2:

  <\eqnarray*>
    <tformat|<table|<row|<cell|-m<rsub|3>>|<cell|=>|<cell|<frac|a<rsub|3>|b<rsub|3>>=<frac|a<rsub|1>+a<rsub|2>|b<rsub|1>+b<rsub|2>>>>>>
  </eqnarray*>

  So let us define:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|3>>|<cell|=>|<cell|a<rsub|1>+a<rsub|2>>>|<row|<cell|b<rsub|3>>|<cell|=>|<cell|b<rsub|1>+b<rsub|2>>>>>
  </eqnarray*>

  Then differentiating <math|H<around*|(|u,v|)>=n> with respect to <math|u>
  and setting <math|d v/d u=-1> gives:

  <\equation*>
    <around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>=<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>
  </equation*>

  and the intersection of this line with <math|H<around*|(|u,v|)>=n> gives
  the point on the hyperbola where the slope is equal to -1:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|T>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|1>>>|<row|<cell|v<rsub|T>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|2>>>>>
  </eqnarray*>

  The equation of a line through the intersection and tangent to the
  hyperbola is then <math|u+v=u<rsub|T>+v<rsub|T>> which simplifies to:

  <\equation*>
    u+v=2*<space|0.25spc><sqrt|a<rsub|3>*<space|0.25spc>b<rsub|3>*<space|0.25spc>n>-c<rsub|1>-c<rsub|2>
  </equation*>

  Next we need to find the pair of lattice points P3
  <math|<around*|(|u<rsub|3,>v<rsub|3>|)>> and P4
  <math|<around*|(|u<rsub|4>,v<rsub|4>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|4>>|<cell|=>|<cell|u<rsub|3>+1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|3>|)>>|<cell|\<geqslant\>>|<cell|1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|4>|)>>|<cell|\<less\>>|<cell|1>>|<row|<cell|v<rsub|3>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|3>|)>|\<rfloor\>>>>|<row|<cell|v<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|4>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  These conditions ensure that diagonal rays pointing outward from P3 and P4
  do not intersect the hyperbola. \ Setting <math|u<rsub|3> =
  <around*|\<lfloor\>|u<rsub|T>|\<rfloor\>>> will satisfy the conditions as
  long as <math|u<rsub|3>\<neq\>0>.

  Introducing intermediate quantities and functions in support of integer
  arithmetic:

  <\eqnarray*>
    <tformat|<table|<row|<cell|d<rsub|i>>|<cell|=>|<cell|a<rsub|i>*<space|0.25spc>b<rsub|i>>>|<row|<cell|f<rsub|i,j>>|<cell|=>|<cell|a<rsub|i>*<space|0.25spc>b<rsub|j>+b<rsub|i>*<space|0.25spc>a<rsub|j>>>|<row|<cell|g<rsub|i,j><around*|(|w|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|f<rsub|i,j>*<space|0.25spc><around*|(|w+c<rsub|i>|)>-<around*|\<lceil\>|<sqrt|<around*|(|w+c<rsub|i>|)><rsup|2>-4*<space|0.25spc>d<rsub|i>*<space|0.25spc>n>|\<rceil\>>|2*<space|0.25spc>d<rsub|i>>|\<rfloor\>>-c<rsub|j>>>>>
  </eqnarray*>

  \;

  Formulas for <math|<around*|(|u<rsub|3,>v<rsub|3>|)>> and
  <math|<around*|(|u<rsub|4,>v<rsub|4>|)>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|3>>|<cell|=>|<cell|<around*|\<lfloor\>|<space|0.25spc><sqrt|<around*|\<lfloor\>|<frac|<around*|(|f<rsub|1,2>+2*<space|0.25spc>d<rsub|1>|)><rsup|2>n|d<rsub|3>>|\<rfloor\>>>|\<rfloor\>>-c<rsub|1>>>|<row|<cell|u<rsub|4>>|<cell|=>|<cell|u<rsub|3+1>>>|<row|<cell|v<rsub|3>>|<cell|=>|<cell|g<rsub|1,2><around*|(|u<rsub|3>|)>>>|<row|<cell|v<rsub|4>>|<cell|=>|<cell|g<rsub|1,2><around*|(|u<rsub|4>|)>>>>>
  </eqnarray*>

  Formulas for horizontal and vertical summation:

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|w>>|<cell|=>|<cell|<big|sum><rsub|u=1><rsup|w>g<rsub|1,2><around*|(|u<rsub|>|)>>>|<row|<cell|S<rsub|h>>|<cell|=>|<cell|<big|sum><rsub|v=1><rsup|h>g<rsub|2,1><around*|(|v<rsub|>|)>>>>>
  </eqnarray*>

  \;
</body>

<\references>
  <\collection>
    <associate|auto-1|<tuple|1|?>>
  </collection>
</references>