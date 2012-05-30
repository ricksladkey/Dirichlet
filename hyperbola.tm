<TeXmacs|1.0.7.15>

<style|article>

<\body>
  <doc-data|<doc-title|A Successive Approximation Algorithm for Computing the
  Divisor Summatory Function (draft)>|<\doc-author-data|<author-name|Richard
  Sladkey>>
    \;
  </doc-author-data>>

  <\abstract>
    An algorithm is presented to compute isolated values of the divisor
    summatory function in <math|O<around*|(|n<rsup|1/3>|)>>time and
    <math|O<around*|(|log n|)>> space. The algorithm is elementary and uses a
    geometric approach of successive approximation combined with coordinate
    transformation.
  </abstract>

  <section|Introduction>

  Consider the hyperbola from Dirichlet's divisor problem in an <math|x y>
  coordinate system:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|x,y|)>>|<cell|=>|<cell|x*<space|0.25spc>y>>|<row|<cell|>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  The number of lattice points under the hyperbola can be thought of as the
  number of combinations of positive integers <math|x> and <math|y> such that
  their product is less than or equal to <math|n>:

  <\equation>
    T<around*|(|n|)>=<big|sum><rsub|x,y:x*y\<leq\>n>1
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

  which gives an <math|O<around*|(|n|)>> algorithm. By using the symmetry of
  the hyperbola (and taking care to avoid double counting) we can do this
  even more efficiently:

  <\equation>
    T<around*|(|n|)>=2<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation>

  \ which gives an <math|O<around*|(|n<rsup|1/2>|)>> algorithm and is in fact
  the usual method by which the divisor summatory function is computed. Our
  goal is to break this square-root barrier.

  In 1903, Voronoï in [<reference|bib:Vor03>] made the first significant
  advance since Dirichlet on the bound on error term for the divisor problem
  by decomposing the hyperbola into a series of non-overlapping triangles
  corresponding to tangent lines whose slopes are extended Farey neighbors.
  We will use a similar approach but where Voronoï produced an exact
  expression for the error term and estimated its magnitude, we will produce
  an algorithm to determine a precise lattice count for an isolated value of
  <math|n> instead.

  <section|Preliminaries>

  It will be convenient to parameterize the sum in <math|T<around*|(|n|)>>
  as:

  <\equation>
    S<around*|(|n,x<rsub|1,>x<rsub|2>|)>=<big|sum><rsup|x<rsub|2>><rsub|x=x<rsub|1>><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation>

  so that:

  <\equation>
    T<around*|(|n|)>=S<around*|(|n,1,n|)>=2*S<around*|(|n,1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation>

  We will also need to count lattice points in triangles. Consider an
  isosceles right triangle <math|<around*|(|0,0|)>\<nocomma\>,<around*|(|i,i|)>,<around*|(|i,0|)>>,
  <math|i> an integer, excluding points on the bottom gives
  <math|1+2+\<ldots\>+i> or:

  <\equation*>
    \<Delta\><around*|(|i|)>=<frac|i*<around*|(|i+1|)>|2>
  </equation*>

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|<graphics||<line|<point|0|0>|<point|2.5|2.5>|<point|2.5|0.0>|<point|0.0|0.0>>|<point|0.5|0.5>|<point|1|0.5>|<point|1.5|0.5>|<point|2|0.5>|<point|2.5|0.5>|<point|2.5|1>|<point|2|1>|<point|1.5|1>|<point|1|1>|<point|1.5|1.5>|<point|2|1.5>|<point|2.5|1.5>|<point|2|2>|<point|2.5|2>|<point|2.5|2.5>|<math-at|<around*|(|i,i|)>|<point|2.5|3>>>>

  This formula is also applicable to triangles of the form
  <math|<around*|(|0,0|)>,<around*|(|i,a*i|)>,<around*|(|i,<around*|(|a-1|)>*i|)>>,
  <math|a> a positive integer. If we desire to to omit the lattice points on
  two sides, we can use <math|\<Delta\><around*|(|i-1|)>> instead of
  <math|\<Delta\><around*|(|i|)>>.

  <section|Region Processing>

  Instead of addressing all of the lattice points, let us for the moment
  consider the sub-task of counting the lattice points in a curvilinear
  triangular region bounded by two tangent lines and a segment of the
  hyperbola. If we can approximate the hyperbola by a series of tangent
  lines, then the area below the lines is a simple polygon and can be
  calculated directly by decomposing the area into triangles. On the other
  hand, the region above the two lines can be handled by chopping off another
  triangle with a third tangent line which creates two smaller curvilinear
  triangular regions.

  We will now go about counting the lattice points in such region. We will do
  this by first transforming the region into a new coordinate system. This is
  very simple conceptually but there are a number of details to take care of
  in order to count lattice points accurately and efficiently. First, the
  tangent lines are not true tangent lines but are actually shifted to pass
  through the nearest lattice points. Because of this, tangent lines need to
  be ``broken'' on either side of the true tangent point in order to keep
  them under but close to the hyperbola. Second, the coordinate
  transformation turns our simple <math|x*y=n> hyperbola into a general
  quadratic in two variables. Nevertheless, the recipe at a high level is
  simply ``tangent, tangent, chop, recurse.''

  This figure depicts a typical region in the <math|x y> coordinate system:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|-100|-100>|2>|gr-grid-old|<tuple|cartesian|<point|-100|-100>|2>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|-100|-100>|2>|gr-edit-grid-old|<tuple|cartesian|<point|-100|-100>|2>|gr-auto-crop|true|<graphics||<line|<point|-2|0>|<point|2.0|-4.0>>|<line|<point|-2|0>|<point|-4.0|4.0>>|<spline|<point|2.28704699048648|-4.19999999999999>|<point|-0.200000000000003|-1.40000000000001>|<point|-3.0|2.40000000000001>|<point|-4.2|4.59999999999999>>|<math-at|P<rsub|0>|<point|-2.59999999999999|-0.400000000000006>>|<math-at|P<rsub|1>|<point|-4.4|3.4>>|<point|-4|4>|<point|-2|0>|<point|2|-4>|<math-at|P<rsub|2>|<point|1.2|-4>>|<math-at|H<around*|(|x,y|)>=n|<point|-1.4|0.8>>|<math-at|L<rsub|2>|<point|-1.2|-1.4>>|<math-at|L<rsub|1>|<point|-4|2.6>>|<math-at|-m<rsub|1>=<frac|a<rsub|1>|b<rsub|1>>|<point|-4.40000000000001|1.2>>|<math-at|-m<rsub|2>=<frac|a<rsub|2>|b<rsub|2>>|<point|-1.40000000000001|-2.59999999999999>>>>

  Define two lines <math|L<rsub|1>> and <math|L<rsub|2>> whose slopes when
  negated have positive integral numerators <math|a<rsub|i>> and denominators
  <math|b<rsub|i>>:\ 

  <\eqnarray*>
    <tformat|<table|<row|<cell|-m<rsub|1>>|<cell|=>|<cell|<frac|a<rsub|1>|b<rsub|1>><eq-number>>>|<row|<cell|-m<rsub|2>>|<cell|=>|<cell|<frac|a<rsub|2>|b<rsub|2>><eq-number>>>>>
  </eqnarray*>

  The slopes are chosen to be Farey neighbors so that the determinant is
  unity:

  <\equation>
    <det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|b<rsub|1>>>|<row|<cell|a<rsub|2>>|<cell|b<rsub|2>>>>>>=a<rsub|1>*<space|0.25spc>b<rsub|2>-b<rsub|1>*<space|0.25spc>a<rsub|2>=1<label|eq:det>
  </equation>

  and the slopes are rational numbers which we require to be in lowest terms
  and so we can assume <math|gcd<around*|(|a<rsub|1>,b<rsub|1>|)> =
  gcd<around*|(|a<rsub|2>,b<rsub|2>|)> = 1>.

  Assume further that the lines intersect at the lattice point
  <math|P<rsub|0>>:

  <\equation>
    <around*|(|x<rsub|0>,y<rsub|0>|)><label|eq:xy0>
  </equation>

  with <math|x<rsub|0>> and <math|y<rsub|0>> positive integers.

  Then the equations for the lines <math|L<rsub|1>> and <math|L<rsub|2>> in
  point-slope form are:

  <\eqnarray*>
    <tformat|<table|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|1>|b<rsub|1>><eq-number><label|eq:ps1>>>|<row|<cell|<frac|y-y<rsub|0>|x-x<rsub|0>>>|<cell|=>|<cell|-<frac|a<rsub|2>|b<rsub|2>><eq-number><label|eq:ps2>>>>>
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

  Now observe that the <math|x y> lattice points form an alternate lattice
  relative to lines <math|L<rsub|1>> and <math|L<rsub|2>>:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|<graphics||<line|<point|-4|-2>|<point|-1.5|-4.5>>|<line|<point|-4.5|-1>|<point|-2.0|-3.5>>|<line|<point|-5|0>|<point|-2.5|-2.5>>|<line|<point|-6.5|3.0>|<point|-4.0|-2.0>>|<line|<point|-3.5|-2.5>|<point|-6.0|2.5>>|<line|<point|-3|-3>|<point|-5.5|2.0>>|<line|<point|-2.5|-3.5>|<point|-5.0|1.5>>|<line|<point|-2|-4>|<point|-4.5|1.0>>|<line|<point|-1.5|-4.5>|<point|-4.0|0.5>>|<line|<point|-6.5|3>|<point|-4.0|0.5>>|<point|-4|-2>|<point|-4.5|-1>|<point|-5|0>|<point|-5.5|1>|<point|-6|2>|<point|-6.5|3>|<point|-3.5|-2.5>|<point|-3|-3>|<point|-2.5|-3.5>|<point|-2|-4>|<point|-1.5|-4.5>|<point|-4|-1.5>|<point|-3.5|-2>|<point|-3|-2.5>|<point|-2.5|-3>|<point|-2|-3.5>|<point|-4.5|-0.5>|<point|-4|-1>|<point|-3.5|-1.5>|<point|-3|-2>|<point|-2.5|-2.5>|<line|<point|-5.5|1>|<point|-3.0|-1.5>>|<line|<point|-6|2>|<point|-3.5|-0.5>>|<point|-4.5|0.0>|<point|-5.0|0.5>|<point|-5.5|1.5>|<point|-6|2.5>|<point|-5.5|2>|<point|-5|1>|<point|-4.5|0.5>|<point|-4|0>|<point|-4|-0.5>|<point|-3.5|-1>|<point|-3.5|-0.5>|<point|-4|0.5>|<point|-4.5|1>|<point|-5|1.5>|<point|-3|-1.5>|<math-at|L<rsub|2>|<point|-3.65603525919218|-3.5>>|<math-at|L<rsub|1>|<point|-5.68321541904923|-0.5>>|<math-at|P<rsub|0>|<point|-4.5|-2.35754988898701>>>>

  Define a <math|u v> coordinate system with an origin of <math|P<rsub|0>>,
  <math|L<rsub|1>> as the <math|v> axis and <math|L<rsub|2>> as the <math|u>
  axis and <math|u> and <math|v> increasing by one for each lattice point in
  the direction of the hyperbola. Then the conversion from the <math|u v>
  coordinates to <math|x y> coordinates is given by:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|x<rsub|0>+b<rsub|2>*<space|0.25spc>u-b<rsub|1>*<space|0.25spc>v<eq-number><label|eq:uv2xy1>>>|<row|<cell|y>|<cell|=>|<cell|y<rsub|0>-a<rsub|2>*<space|0.25spc>u
    +a<rsub|1>*<space|0.25spc>v<eq-number><label|eq:uv2xy2>>>>>
  </eqnarray*>

  Substituting for <math|x<rsub|0>> and <math|y<rsub|0>> and rearranging
  gives:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)><eq-number><label|eq:uv2xy3>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)><eq-number><label|eq:uv2xy4>>>>>
  </eqnarray*>

  Solving these equations for <math|u> and <math|v> and substituting unity
  for the determinant provides the inverse conversion from <math|x y>
  coordinates to <math|u v> coordinates:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y-c<rsub|1><eq-number><label|eq:xy2uv1>>>|<row|<cell|v>|<cell|=>|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y-c<rsub|2><eq-number><label|eq:xy2uv2>>>>>
  </eqnarray*>

  Because all quantities are integers, equations (<reference|eq:uv2xy3>),
  (<reference|eq:uv2xy4>), (<reference|eq:xy2uv1>), (<reference|eq:xy2uv2>)
  mean that each <math|x y> lattice point corresponds to a <math|u v> lattice
  point and vice versa. As a result, we can choose to count lattice points in
  either <math|x y> coordinates or <math|u v> coordinates.

  Now we are ready to transform the hyperbola into the <math|u v> coordinate
  system by substituting for <math|x> and <math|y> in
  <math|H<around*|(|x,y|)>> which gives:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|u,v|)>>|<cell|=>|<cell|<around*|(|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>|)><around*|(|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>|)><eq-number>>>|<row|<cell|>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  Let us choose a point <math|P<rsub|1>> <math|<around*|(|0,h|)>> on the
  <math|v> axis and a point <math|P<rsub|2>> <math|<around*|(|w,0|)>> on the
  <math|u> axis such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|H<around*|(|u<rsub|h>,h|)>>|<cell|=>|<cell|n>>|<row|<cell|H<around*|(|w,v<rsub|w>|)>>|<cell|=>|<cell|n>>|<row|<cell|0\<leqslant\>u<rsub|h>>|<cell|\<less\>>|<cell|1>>|<row|<cell|0\<leqslant\>v<rsub|w>>|<cell|\<less\>>|<cell|1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|h>|)>>|<cell|\<geq\>>|<cell|0>>|<row|<cell|-d u/d
    v<around*|(|v<rsub|w>|)>>|<cell|\<geq\>>|<cell|0>>>>
  </eqnarray*>

  or equivalently that the hyperbola is less than one unit away from the
  nearest axis at <math|P<rsub|1>> and <math|P<rsub|2>> and that the distance
  to the hyperbola increases as you approach the origin.

  With these constraints, the hyperbolic segment has the same basic shape as
  the full hyperbola: roughly tangent to the axes at the endpoints and
  strictly decreasing relative to either axis.

  This figure depicts a region in the <math|u v> coordinate system:

  <with|gr-mode|<tuple|edit|point>|gr-frame|<tuple|scale|1.18926cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.733333par|center>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|magnify|1.18920711391541|gr-magnify|1.18920711391541|gr-color|#969696|gr-dash-style|10|<graphics||<point|0|0>|<point|0.0|3.5>|<point|3.5|0.0>|<math-at|P<rsub|2>|<point|3.5|-0.315875450190698>>|<with|color|#969696|dash-style|10|magnify|1.18920711391541|<line|<point|0|3.5>|<point|3.5|3.5>|<point|3.5|0.0>>>|<math-at|w|<point|1.67812|3.7323>>|<math-at|h|<point|3.71164|2>>|<math-at|u|<point|4.22327127104857|-0.250914248369648>>|<spline|<point|4.5|0.262528795121958>|<point|3.0|0.5>|<point|0.701313192297385|2.5>|<point|0.270375393271184|4.0>>|<math-at|H<around*|(|u,v|)>=n|<point|0.823796876955379|2.69183920352629>>|<math-at|v|<point|-0.411937664151288|4.0>>|<math-at|P<rsub|1>|<point|-0.5|3.34795412488904>>|<math-at|P<rsub|0>|<point|-0.5|-0.324619072766175>>|<with|color|#969696|magnify|1.18920711391541|<point|0.5|3.5>>|<with|color|#969696|magnify|1.18920711391541|<point|3.5|0.5>>>>

  We can now reformulate the number of lattice points in this region
  <math|R<rsub|> > as a function of the eight values that define it:

  <\equation>
    S<rsub|<rsub|R>>=S<rsub|R><around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>
  </equation>

  If <math|H<around*|(|w,1|)>\<leq\>n>, then <math|v<rsub|w>\<geq\>1> and we
  can remove the first lattice row:

  <\equation>
    S<rsub|R>=S<rsub|R><around*|(|w,h-1,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>+1|)>+w
  </equation>

  and if <math|H<around*|(|1,h|)>\<leq\>n>, then <math|u<rsub|h>\<geq\>1> and
  we can remove the first lattice column:

  <\equation>
    S<rsub|R>=S<rsub|R><around*|(|w-1,h,a<rsub|1>,b<rsub|1>,c<rsub|1>+1,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>+h
  </equation>

  so that the conditions are satisified.

  At this point we could count lattice points in the region bounded by the
  <math|u> and <math|v> axes and <math|u=w> and <math|v=h> using brute force:

  <\equation>
    S<rsub|R>=<big|sum><rsub|u,v:H<around*|(|u,v|)>\<leqslant\>n>1
  </equation>

  More efficiently, if we had a formulas for <math|u> and <math|v> in terms
  each other, we could sum columns of lattice points:

  <\eqnarray*>
    <tformat|<table|<row|<cell|>|<cell|S<rsub|W><around*|(|w|)>=<big|sum><rsup|w><rsub|u=1><around*|\<lfloor\>|V<around*|(|u|)>|\<rfloor\>>>|<cell|<eq-number>>>|<row|<cell|>|<cell|S<rsub|H><around*|(|h|)>=<big|sum><rsub|v=1><rsup|h><around*|\<lfloor\>|U<around*|(|v|)>|\<rfloor\>>>|<cell|<eq-number>>>>>
  </eqnarray*>

  using whichever axis has fewer points, keeping in mind that it could be
  assymmetric. (Note that these summations are certain not to overcount
  because by our conditions <math|V<around*|(|u|)>\<less\>h> for
  <math|0\<less\>u\<leq\>w> and <math|U<around*|(|v|)>\<less\>w> for
  <math|0\<less\>v\<leq\>h>.)

  And so:

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|R><around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|S<rsub|W><eq-number>>>|<row|<cell|>|<cell|=>|<cell|S<rsub|H>>>>>
  </eqnarray*>

  In fact we can derive formulas for <math|u> and <math|v> in terms of each
  other by solving <math|H<around*|(|u,v|)>=n> (which when expanded is a
  general quadratic in two variables) for <math|v> or <math|u>. The resulting
  explicit formulas for <math|v> in terms of <math|u> and <math|u> in terms
  of <math|v> are:

  <\eqnarray*>
    <tformat|<table|<row|<cell|V<around*|(|u|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-<sqrt|<around*|(|u+c<rsub|1>|)><rsup|2>-4*<space|0.25spc>a<rsub|1>*b<rsub|1>*n>|2*<space|0.25spc>a<rsub|1>*b<rsub|1>>-c<rsub|2><eq-number>>>|<row|<cell|U<around*|(|v|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-<sqrt|<around*|(|v+c<rsub|2>|)><rsup|2>-4*<space|0.25spc>a<rsub|2>*b<rsub|2>*n>|2*<space|0.25spc>a<rsub|2>*b<rsub|2>>-c<rsub|1><eq-number>>>>>
  </eqnarray*>

  (Note exchanging <math|u> for <math|v> results in the same formula with
  subscripts 1 and 2 exchanged.)

  As a result we can compute the number of lattice points within the region
  using a method similar to the method usually used for the hyperbola as a
  whole. Our goal, however, it to subdivide the region into two smaller
  regions and process them recursively, only using manual counting at our
  discretion. To do so we need to remove an isosceles right triangle in the
  lower-left corner and what will be left are two sub-regions in the
  upper-left and lower-right.

  This figure shows the right triangle and the two sub-regions:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.370012gw|0.270022gh>>|gr-geometry|<tuple|geometry|1par|100|center>|gr-grid|<tuple|cartesian|<point|0|0>|2>|gr-grid-old|<tuple|cartesian|<point|0|0>|2>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|2>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|2>|gr-auto-crop|true|gr-dash-style|10|<graphics||<with|color|#969696|dash-style|10|<line|<point|0|4.4>|<point|4.8|4.4>|<point|4.8|0.0>>>|<point|0|4.4>|<point|0|0>|<point|4.8|0>|<line|<point|0|2.6>|<point|2.4|0.0>>|<math-at|P<rsub|0>|<point|-0.6|-0.4>>|<math-at|P<rsub|1>|<point|-0.6|4.2>>|<math-at|P<rsub|2>|<point|4.6|-0.4>>|<math-at|u|<point|5.4|0.2>>|<math-at|v|<point|-0.337032014816775|4.9255853948935>>|<math-at|h|<point|5|2.4>>|<math-at|w|<point|2.4|4.6>>|<math-at|u<rprime|'>|<point|0.2|1.8>>|<math-at|u<rprime|''>|<point|3.4|-0.4>>|<math-at|v<rprime|''>|<point|1.4|0.4>>|<math-at|v<rprime|'>|<point|-0.4|3.4>>|<with|color|#969696|dash-style|10|<line|<point|1.29231|1.2>|<point|3.6|1.2>|<point|4.8|0.0>>>|<with|color|#969696|dash-style|10|<line|<point|1.2|1.3>|<point|1.2|3.4>|<point|0.0|4.4>>>|<math-at|R|<point|2.6|2.8>>|<math-at|R<rprime|''>|<point|3.0|0.6>>|<math-at|R<rprime|'>|<point|0.4|3.2>>|<spline|<point|0.2|4.6>|<point|0.6|2.6>|<point|2.88244146051065|0.48245468977378>|<point|5.0|0.2>>|<point|1.31142|1.4>|<math-at|P<rsub|tan>|<point|1.4|1.6>>>>

  A diagonal with slope -1 in the <math|u v> coordinate system has a slope in
  the <math|x y> coordinate system that is the mediant of the slopes of lines
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
  the point <math|P<rsub|tan>> on the hyperbola where the slope is equal to
  -1:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|tan>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|1><eq-number>>>|<row|<cell|v<rsub|tan>>|<cell|=>|<cell|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>|)>*<space|0.25spc><sqrt|<frac|n|a<rsub|3>*<space|0.25spc>b<rsub|3>>>-c<rsub|2><eq-number>>>>>
  </eqnarray*>

  The equation of a line through this intersection and tangent to the
  hyperbola is then <math|u+v=u<rsub|tan>+v<rsub|tan>> which simplifies to:

  <\equation>
    u+v=2*<space|0.25spc><sqrt|a<rsub|3>*<space|0.25spc>b<rsub|3>*<space|0.25spc>n>-c<rsub|1>-c<rsub|2>
  </equation>

  Next we need to find the pair of lattice points <math|P<rsub|4>>
  <math|<around*|(|u<rsub|4,>v<rsub|4>|)>> and <math|P<rsub|5>>
  <math|<around*|(|u<rsub|5>,v<rsub|5>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u<rsub|4>>|<cell|\<gtr\>>|<cell|0>>|<row|<cell|u<rsub|5>>|<cell|=>|<cell|u<rsub|4>+1>>|<row|<cell|-d
    v/d u<around*|(|u<rsub|4>|)>>|<cell|\<geq\>>|<cell|1>>|<row|<cell|-d v/d
    u<around*|(|u<rsub|5>|)>>|<cell|\<less\>>|<cell|1>>|<row|<cell|v<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|4>|)>|\<rfloor\>>>>|<row|<cell|v<rsub|5>>|<cell|=>|<cell|<around*|\<lfloor\>|V<around*|(|u<rsub|5>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  The derivative conditions ensure that the diagonal rays with slope
  <math|-1> pointing outward from <math|P<rsub|4>> and <math|P<rsub|5>> do
  not intersect the hyperbola. Setting <math|u<rsub|4> =
  <around*|\<lfloor\>|u<rsub|tan>|\<rfloor\>>> will satisfy the conditions as
  long as <math|u<rsub|4>\<neq\>0>.

  Let the point at which the ray from <math|P<rsub|4>> intersects the
  <math|v> axis be <math|P<rsub|6 ><around*|(|0,v<rsub|6>|)>> and the point
  at which the ray from <math|P<rsub|5>> intersects the <math|u> axis be
  <math|P<rsub|7 ><around*|(|u<rsub|7>,0|)>>. Then:

  <\eqnarray*>
    <tformat|<table|<row|<cell|v<rsub|6>>|<cell|=>|<cell|u<rsub|4>+v<rsub|4><eq-number>>>|<row|<cell|u<rsub|7>>|<cell|=>|<cell|u<rsub|5>+v<rsub|5><eq-number>>>>>
  </eqnarray*>

  A diagram of all the points defined so far:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1.18926cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.733333par|center>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|magnify|1.18920711391541|gr-dash-style|10|<graphics||<point|0|0>|<point|0.0|3.5>|<point|1.5|1.0>|<line|<point|1.5|1>|<point|2.5|0.0>>|<point|2.5|0>|<point|3.5|0.0>|<math-at|P<rsub|2>|<point|3.5|-0.315875450190698>>|<math-at|P<rsub|5>|<point|1.0|0.691569794964324>>|<with|color|#969696|dash-style|10|<line|<point|0|3.5>|<point|3.5|3.5>|<point|3.5|0.0>>>|<math-at|w|<point|1.67812|3.7323>>|<math-at|h|<point|3.71164|2>>|<math-at|u|<point|4.22327127104857|-0.250914248369648>>|<spline|<point|4.5|0.262528795121958>|<point|3.0|0.5>|<point|0.701313192297385|2.5>|<point|0.270375393271184|4.0>>|<line|<point|1.0|2.0>|<point|0.0|3.0>>|<point|0.0|3.0>|<point|1.25659730989343|1.69647908332071>|<math-at|P<rsub|6>|<point|-0.5|2.68372631475282>>|<point|1.0|2.0>|<math-at|P<rsub|4>|<point|0.5|1.69113478621578>>|<math-at|P<rsub|tan>|<point|1.5|1.76202136806692>>|<math-at|P<rsub|7>|<point|2.5|0.185032086664982>>|<math-at|H<around*|(|u,v|)>=n|<point|0.823796876955379|2.69183920352629>>|<math-at|v|<point|-0.411937664151288|4.0>>|<math-at|P<rsub|1>|<point|-0.5|3.34795412488904>>|<math-at|P<rsub|0>|<point|-0.5|-0.324619072766175>>|<with|dash-style|10|<line|<point|1|2>|<point|1.48426658509115|1.04990475131054>>>|<math-at|N|<point|0.342465|1>>>>

  Then the number of lattice points above the axes and inside the polygon
  <math|N> defined by points <math|P<rsub|0>,P<rsub|6>,P<rsub|4>,P<rsub|5>,P<rsub|7>>
  is

  <\equation*>
    S<rsub|N>=\<Delta\><around*|(|v<rsub|6>-1|)>-\<Delta\><around*|(|v<rsub|6>-u<rsub|5>|)>+\<Delta\><around*|(|u<rsub|7>-u<rsub|5>|)>
  </equation*>

  or

  <\equation>
    S<rsub|N>=<choice|<tformat|<table|<row|<cell|\<Delta\><around*|(|v<rsub|6>-1|)>+u<rsub|4>>|<cell|if
    v<rsub|6>\<less\>u<rsub|7>>>|<row|<cell|\<Delta\><around*|(|v<rsub|6>-1|)>>|<cell|if
    v<rsub|6> = u<rsub|7>>>|<row|<cell|\<Delta\><around*|(|u<rsub|7>-1|)>+v<rsub|5>>|<cell|if
    v<rsub|6>\<gtr\>u<rsub|7>>>>>>
  </equation>

  because counting on reverse lattice diagonals starting at the origin we sum
  <math|1+2+\<ldots\>+<around*|(|min<around*|(|v<rsub|6>,u<rsub|7>|)>-1|)>>
  plus a partial diagonal if the polygon is not a triangle.

  Using the properties of Farey fractions observe that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|b<rsub|1>>>|<row|<cell|a<rsub|3>>|<cell|b<rsub|3>>>>>>>|<cell|=>|<cell|a<rsub|1>*<around*|(|b<rsub|1>+b<rsub|2>|)><rsub|>-b<rsub|1>*<around*|(|a<rsub|1>+a<rsub|2>|)>=a<rsub|1>*b<rsub|2>-b<rsub|1>*a<rsub|2>=<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|b<rsub|1>>>|<row|<cell|a<rsub|2>>|<cell|b<rsub|2>>>>>>=1>>|<row|<cell|<det|<tformat|<table|<row|<cell|a<rsub|3>>|<cell|b<rsub|3>>>|<row|<cell|a<rsub|2>>|<cell|b<rsub|2>>>>>>>|<cell|=>|<cell|*<around*|(|a<rsub|1>+a<rsub|2>|)><rsub|>*b<rsub|2>-*<around*|(|b<rsub|1>+b<rsub|2>|)>*a<rsub|2>=a<rsub|1>*b<rsub|2>-b<rsub|1>*a<rsub|2>=<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|b<rsub|1>>>|<row|<cell|a<rsub|2>>|<cell|b<rsub|2>>>>>>=1>>>>
  </eqnarray*>

  so that <math|m<rsub|1>> and <math|m<rsub|3>> are also Farey neighbors and
  likewise for <math|m<rsub|3>> and <math|m<rsub|2>>.

  So we can define region <math|R<rprime|'>> to be the sub-region with
  <math|P<rprime|'><rsub|1>=P<rsub|1>,P<rprime|'><rsub|0>=P<rsub|6>,P<rprime|'><rsub|2>=P<rsub|4>>
  and the region <math|R<rprime|''>> to be the sub-region with
  <math|P<rprime|''><rsub|1>=P<rsub|5>,P<rprime|''><rsub|0>=P<rsub|7>,P<rsub|2><rprime|''>=P<rsub|2>>
  and then the number of lattice points in the entire region is
  <math|S<rsub|R>=S<rsub|N>+S<rsub|R<rprime|'>>+S<rsub|R<rprime|''>>> or

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|R><around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|S<rsub|N><eq-number>>>|<row|<cell|>|<cell|+>|<cell|S<rsub|R><around*|(|u<rsub|4>,h-v<rsub|6>,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+v<rsub|6>|)>>>|<row|<cell|>|<cell|+>|<cell|S<rsub|R><around*|(|w-u<rsub|7>,v<rsub|5>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+u<rsub|7>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>.>>>>
  </eqnarray*>

  This recursive formula for the sum of the lattice points in a region in
  terms of the lattice points in its sub-regions allows us to use a divide
  and conquer approach to counting lattice points under the hyperbola.

  <section|Top Level Processing>

  Now let us return to the hyperbola as a whole. It should be clear that it
  is easy in <math|x y> coordinates to calculate <math|y> in terms of
  <math|x> by solving <math|H<around*|(|x,y|)>=n> for <math|y>:

  <\equation>
    Y<around*|(|x|)>=<frac|n|x>
  </equation>

  We know that we only need to sum lattice points under the hyperbola up to
  <math|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>. The point <math|<sqrt|n>>
  is in fact at the <math|x=y> axis of symmetry and so the slope at that
  point is exactly <math|-1>. The next integral slope occurs at <math|-2>, so
  our first (and largest) region occurs between slopes <math|-m<rsub|1>=2>
  and <math|-m<rsub|2>=1>. By processing adjacent integral slopes we will
  start in the middle and work our way back towards the origin.

  However, we cannot use the region method for the whole hyperbola because
  regions become smaller and smaller and eventually a region has a size
  <math|w+h \<leq\>1>. We can find the point where this occurs by taking the
  second derivative of <math|Y<around*|(|x|)>> with respect to <math|x> and
  setting it to unity. In other words, the point on the hyperbola where the
  rate of change in the slope exceeds one per lattice column, which is:

  <\equation>
    x=<sqrt|2*n|3>= 2<rsup|1/3>*n<rsup|1/3>\<approx\>1.26*n<rsup|1/3>
  </equation>

  As a result there is no benefit in region processing the first
  <math|O<around*|(|n<rsup|1/3>|)>> lattice columns so we resort to the
  simple method to sum the lattice columns less than <math|x<rsub|min>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|min>>|<cell|=>|<cell|C<rsub|1>**<around*|\<lceil\>|<sqrt|2*n|3>|\<rceil\>><eq-number>>>|<row|<cell|x<rsub|max>>|<cell|=>|<cell|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>>|<row|<cell|y<rsub|min>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|max>|)>|\<rfloor\>>>>|<row|<cell|S<rsub|1>>|<cell|=>|<cell|S<around*|(|n,1,x<rsub|min>-1|)>>>>>
  </eqnarray*>

  where <math|C<rsub|1>\<geq\>1> is a constant to be chosen later.

  Next we need to account for the all the points on or below the first line
  which is a rectangle and a triangle:

  <\equation*>
    S<rsub|2>=<around*|(|x<rsub|max>-x<rsub|min>+1|)>*y<rsub|min>+\<Delta\><around*|(|x<rsub|max>-x<rsub|min>|)>
  </equation*>

  Because all slopes in this section of the algorithm are whole integers, we
  have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|i>>|<cell|=>|<cell|-m<rsub|i>>>|<row|<cell|b<rsub|i>>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  Assume that we have point <math|P<rsub|2>\<nocomma\>> and value
  <math|a<rsub|2>> from the previous iteration. For the first iteration we
  will have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|2>>|<cell|=>|<cell|x<rsub|max>>>|<row|<cell|y<rsub|2>>|<cell|=>|<cell|y<rsub|min>>>|<row|<cell|a<rsub|2>>|<cell|=>|<cell|1>>>>
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
    x<rsub|tan>=<sqrt|<frac|n|a<rsub|1>>>
  </equation>

  Similar to processing a region (but now in <math|x y> coordinates), we now
  need two lattice points <math|P<rsub|4> <around*|(|x<rsub|4,>y<rsub|4>|)>>
  and <math|P<rsub|5> <around*|(|x<rsub|5>,y<rsub|5>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|4>>|<cell|\<gtr\>>|<cell|x<rsub|min>>>|<row|<cell|x<rsub|5>>|<cell|=>|<cell|x<rsub|4>+1>>|<row|<cell|-d
    y/d x<around*|(|x<rsub|4>|)>>|<cell|\<geq\>>|<cell|a<rsub|1>>>|<row|<cell|-d
    y /d x<around*|(|x<rsub|5>|)>>|<cell|\<less\>>|<cell|a<rsub|1>>>|<row|<cell|y<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|4>|)>|\<rfloor\>>>>|<row|<cell|y<rsub|5>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|5>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  To meet these conditions we can set <math|x<rsub|4>=<around*|\<lfloor\>|x<rsub|tan>|\<rfloor\>>>
  unless <math|x<rsub|4>\<leq\>x<rsub|min>> in which case we can manually
  count the lattice columns between <math|x<rsub|min>> and <math|x<rsub|2>>
  and cease iterating. If so, the remaining columns can be computed as:

  <\equation*>
    S<rsub|3>=<big|sum><rsup|x<rsub|2-1>><rsub|x=x<rsub|min>><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|(|a<rsub|2>*<around*|(|x<rsub|2>-x|)>+y<rsub|2>|)>
  </equation*>

  which is the number of lattice points below the hyperbola and above line
  <math|L<rsub|2>> over the interval <math|<around*|[|x<rsub|min>,x<rsub|2>|)>>.

  Now take line <math|L<rsub|2>> with slope <math|-a<rsub|2>> passing through
  <math|P<rsub|2>>, lines <math|L<rsub|4>> and <math|L<rsub|5>> with slopes
  <math|-a<rsub|1>> and passing through <math|P<rsub|4>> and <math|P<rsub|5>>
  and then find the point <math|P<rsub|6>> where <math|L<rsub|4>> intersects
  <math|x=x<rsub|min>> and the point <math|P<rsub|0>> where <math|L<rsub|5>>
  intersects <math|L<rsub|2>> and the point <math|P<rsub|7>> where
  <math|L<rsub|2>> intersects <math|x=x<rsub|min>> and denote by
  <math|c<rsub|i>> the <math|y> intercept of line <math|L<rsub|i>>.

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|gr-dash-style|10|<graphics||<line|<point|-3.0|-1.0>|<point|-4.0|1.0>>|<point|-4|1>|<point|-3|-1>|<line|<point|-6.0|2.0>|<point|-1.0|-3.0>>|<line|<point|-4.5|1.5>|<point|-6.0|4.5>>|<point|-4.5|1.5>|<spline|<point|-5.5|4.33826814463599>|<point|-4.5|2.0>|<point|-2.5|-1.0>|<point|-1.0|-2.73817405755366>>|<math-at|P<rsub|0>|<point|-3.5|-1.25312>>|<math-at|L<rsub|2>|<point|-2.76973250390689|-2.0>>|<point|-1|-3>|<math-at|P<rsub|2>|<point|-1.5|-3.22179084540838>>|<with|color|dark
  grey|<line|<point|-6|4.5>|<point|-6.0|-0.5>>>|<math-at|x<rsub|min>|<point|-6.23638|-1>>|<math-at|R|<point|-2.30258|-0.5>>|<math-at|H<around*|(|x,y|)>=n|<point|-4.5|3>>|<math-at|P<rsub|5>|<point|-4.5|0.682911411193787>>|<math-at|P<rsub|4>|<point|-5.09461430341988|1.5>>|<math-at|P<rsub|tan>|<point|-4.0|1.5>>|<point|-6|4.5>|<point|-6|2>|<math-at|P<rsub|6>|<point|-6.5|4.0>>|<math-at|P<rsub|7>|<point|-6.5|2>>|<with|dash-style|10|<line|<point|-4.5|1.5>|<point|-4.01534594523085|1.04152334964942>>>|<math-at|M|<point|-5.69093125733293|2.5>>|<point|-4.11755853948935|1.33183952903823>>>

  \;

  Now add up the lattice points in the polygon <math|M> defined by the points
  <math|P<rsub|0>,P<rsub|7>,P<rsub|6>,P<rsub|4>,P<rsub|5>> but above
  <math|L<rsub|2>> by adding the whole triangle corresponding to
  <math|L<rsub|4>>, subtracting the portion of it to the right of
  <math|P<rsub|4>>, and then adding back the triangle corresponding to
  <math|L<rsub|5>> stating at <math|P<rsub|5>>:

  <\equation>
    S<rsub|M>=\<Delta\><around*|(|c<rsub|4>-c<rsub|2>-x<rsub|min>|)>-\<Delta\><around*|(|c<rsub|4>-c<rsub|2>-x<rsub|5>|)>+\<Delta\><around*|(|c<rsub|5>-c<rsub|2>-x<rsub|5>|)>
  </equation>

  where if <math|L<rsub|4>> is coincident with <math|L<rsub|5>>, the second
  two terms cancel each other out.

  Then choosing <math|P<rsub|1>=P<rsub|5>> (together with <math|P<rsub|0>>
  and <math|P<rsub|2>>) and calculating the necessary quantities we have a
  region <math|R> and can now count lattice points using region processing:\ 

  <\equation>
    S<rsub|R>=S<rsub|R><around*|(|a<rsub|1>*x<rsub|2>+y<rsub|2>-c<rsub|5>,a<rsub|2>*x<rsub|5>+y<rsub|5>-c<rsub|2>,a<rsub|1>,1,c<rsub|5>,a<rsub|2>,1,c<rsub|2>|)>
  </equation>

  so the total sum for this iteration is:

  <\equation*>
    S<rsub|A><around*|(|a<rsub|1>|)>=S<rsub|M>+S<rsub|R>
  </equation*>

  Then we may advance to the next region by setting:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rprime|'><rsub|2>>|<cell|=>|<cell|x<rsub|4>>>|<row|<cell|y<rprime|'><rsub|2>>|<cell|=>|<cell|y<rsub|4>>>|<row|<cell|a<rprime|'><rsub|2>>|<cell|=>|<cell|a<rsub|1>>>>>
  </eqnarray*>

  Summing all interations gives

  <\equation*>
    S<rsub|4>=<big|sum><rsup|a<rsub|max>><rsub|a=2>S<rsub|A><around*|(|a|)>.
  </equation*>

  Finally, the total number of lattice points under the hyperbola from
  <math|1> to <math|x<rsub|max>> is

  <\equation>
    S<rsub|T>=S<around*|(|1,x<rsub|max>|)>=S<rsub|1>+S<rsub|2>+S<rsub|3>+S<rsub|4>
  </equation>

  and therefore the final computation of the divisor summatory function is
  given by

  <\equation>
    T<around*|(|n|)>=2*S<rsub|T>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>.<rsup|2>
  </equation>

  <section|Division-Free Counting>

  Since we calculate <math|S<rsub|1>> using the traditional method and since
  the computation will consist entirely of <math|S<rsub|1>> when
  <math|n\<less\>4*C<rsub|1><rsup|6>>, it is beneficial to have a faster
  method of performing this step, albeit by a constant factor. Denote by
  <math|l=<around*|\<lceil\>|log<rsub|2><around*|(|n|)>|\<rceil\>>> the
  number of bits needed to represent <math|n>. We can avoid an <math|l>-bit
  division in most iterations by using a Bresenham-style calculation (see
  [<reference|bib:Bres77>]) and working backwards while computing an estimate
  of the result of the division based on the previous iteration.

  Define <math|\<beta\><around*|(|x|)>=<around*|\<lfloor\>|Y<around*|(|x|)>|\<rfloor\>>>,
  the finite difference <math|\<delta\><rsub|1><rsub|><around*|(|x|)>
  =\<beta\><around*|(|x|)>-\<beta\><around*|(|x+1|)>>, and the second-order
  finite difference <math|\<delta\><rsub|2><around*|(|x|)>=\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|1><around*|(|x+1|)>>.
  To check whether the value is correct we can also keep track of the error.
  So defining the error <math|\<varepsilon\><around*|(|x|)>=n-x*\<beta\><around*|(|x|)>=n-x*<around*|\<lfloor\>|n/x|\<rfloor\>>=n
  mod x> gives

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<varepsilon\><around*|(|x|)>-\<varepsilon\><around*|(|x+1|)>>|<cell|=>|<cell|<around*|(|x+a|)>\<beta\><around*|(|x+1|)>-x*\<beta\><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|x+1|)>*\<beta\><around*|(|x+1|)>-x*<around*|(|\<beta\><around*|(|x+1|)>+\<delta\><rsub|1><around*|(|x+1|)>+\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|\<beta\><around*|(|x+1|)>-x*\<delta\><rsub|1><around*|(|x+1|)>-x*\<delta\><rsub|2><around*|(|x|)>>>>>
  </eqnarray*>

  Introducing the intermediate quantity <math|\<gamma\><around*|(|x|)>=\<beta\><around*|(|x|)>-<around*|(|x-1|)>*\<delta\><rsub|1><around*|(|x|)>>
  and <math|<wide|\<varepsilon\>|^><around*|(|x|)>> as the estimate of the
  error assuming <math|\<delta\><rsub|2><around*|(|x|)>>=0 then

  <\eqnarray*>
    <tformat|<table|<row|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>>|<cell|=>|<cell|\<varepsilon\><around*|(|x+1|)>+\<gamma\><around*|(|x+1|)>>>|<row|<cell|\<delta\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|<wide|\<varepsilon\>|^><around*|(|x|)>|x>|\<rfloor\>>>>|<row|<cell|\<delta\><rsub|1><around*|(|x|)>>|<cell|=>|<cell|\<delta\><rsub|1><around*|(|x+1|)>+\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>>|<cell|=>|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x|)>>|<cell|=>|<cell|\<gamma\><around*|(|x+1|)>+2*\<delta\><rsub|1><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<beta\><around*|(|x|)>>|<cell|=>|<cell|\<beta\><around*|(|x+1|)>+\<delta\><rsub|1><around*|(|x|)>.>>>>
  </eqnarray*>

  Over the range <math|x<rsub|1>\<leq\>x\<leq\>x<rsub|2>> these integer
  quantites are bounded in size by <math|x,\<varepsilon\><around*|(|x|)>,<around*|\||<wide|\<varepsilon\>|^><around*|(|x|)>|\|>\<leq\>x<rsub|2>,<around*|\||\<gamma\><around*|(|x|)>|\|>\<leq\>max<around*|(|2*n/<around*|(|x<rsub|1><rsup|2>+x<rsub|1>|)>,x<rsub|2>|)>>,
  <math|\<beta\><around*|(|x|)>\<leq\>n/x<rsub|1>>,
  <math|\<delta\><rsub|1><around*|(|x|)>\<leq\>n/x<rsub|1><rsup|2>>+1,<math|<around*|\||\<delta\><rsub|2><around*|(|x|)>|\|>\<leq\>2*n/x<rsub|1><rsup|3>+2>.

  For <math|<sqrt|2n|3>\<less\>x\<leq\><sqrt|n>>,
  <math|\<delta\><rsub|2><around*|(|x|)>\<in\><around*|{|-1,0,1,2|}>> and so

  <\equation*>
    <around*|\<lfloor\>|<frac|<wide|\<varepsilon\>|^><around*|(|x|)>|x>|\<rfloor\>>=<choice|<tformat|<cwith|1|-1|1|1|cell-halign|r>|<table|<row|<cell|2>|<cell|if
    <wide|\<varepsilon\>|^><around*|(|x|)>\<geq\>2*x;>>|<row|<cell|1>|<cell|if
    x\<leq\><wide|\<varepsilon\>|^><around*|(|x|)>\<less\>2*x;>>|<row|<cell|-1>|<cell|if
    <wide|\<varepsilon\>|^><around*|(|x|)>\<less\>0;>>|<row|<cell|0>|<cell|otherwise;>>>>>
  </equation*>

  and thus <math|\<beta\><around*|(|x|)>,\<gamma\><around*|(|x|)>,\<delta\><rsub|1><around*|(|x|)>,\<varepsilon\><around*|(|x|)>>
  can be computed from <math|\<beta\><around*|(|x+1|)>,\<gamma\><around*|(|x+1|)>,\<delta\><rsub|1><around*|(|x+1|)>,\<varepsilon\><around*|(|x+1|)>>
  using only addition and subtraction of <math|<frac|1|2>*l>-bit quantities
  except <math|\<beta\><around*|(|x|)>> which is <math|<frac|2|3>*l> bits.
  Note that <math|<wide|\<varepsilon\>|^><around*|(|x|)>\<geq\>2*x> is very
  rare over this range and if <math|<wide|\<varepsilon\>|^><around*|(|x|)>\<geq\>3*x>,
  it means that <math|x\<less\><sqrt|2n|3>.> For
  <math|n<rsup|1/6>\<leq\>x\<leq\><sqrt|2n|3>> we can add the modest division
  <math|<around*|\<lfloor\>|<wide|\<varepsilon\>|^><around*|(|x|)>/x|\<rfloor\>>>
  between two <math|<frac|1|3>*l>-bit values, <math|\<gamma\><around*|(|x|)>>
  and <math|\<delta\><rsub|1><around*|(|x|)>> grow to <math|<frac|2|3>*l>
  bits and <math|\<beta\><around*|(|x|)>> grows to <math|<frac|5|6>*l> bits.
  \ For <math|x\<less\>n<rsup|1/6>> we can sum using ordinary division.

  <section|Algorithms>

  In this section we present a series of algorithms based on the previous
  sections. \ The short-hand notation <math|F<around*|(|x|)>:expression>
  signifies a functional value that remains unevaluated until referenced.

  \;

  The first algorithm is a straightforward version of the basic successive
  approximation method. A literal implementation based on this description
  will offer many opportunities for optimization. Various formulas have been
  slightly modified so that the entire algorithm can be implemented using
  only unsigned multi-precision integer arithmetic. The operations required
  are addition, subtraction, multiplication, floor division, floor square
  root, ceiling square root, and ceiling cube root. If any of the root
  operations are not available, they may be implemented using Newton's
  method.

  <\algorithm>
    Inputs: <math|n\<geq\>0,C<rsub|1>\<approx\>10,C<rsub|2>\<approx\>10>

    \;

    <math|\<Delta\><around*|(|i|)>:i*<around*|(|i+1|)>/2>

    <math|S<rsub|1><around*|(||)>:<big|sum><rsub|x=1><rsup|x\<less\>x<rsub|min>><around*|\<lfloor\>|n/x|\<rfloor\>>>

    <math|S<rsub|2><around*|(||)>:<around*|(|x<rsub|max>-x<rsub|min>+1|)>*y<rsub|min>+\<Delta\><around*|(|x<rsub|max>-x<rsub|min>|)>>

    <math|S<rsub|3><around*|(||)>:<big|sum><rsub|x=x<rsub|min><rsup|>><rsup|x\<less\>x<rsub|2>><around*|\<lfloor\>|n/x|\<rfloor\>>-<around*|(|a<rsub|2>*<around*|(|x<rsub|2>-x|)>+y<rsub|2>|)>>

    <math|S<rsub|M><around*|(||)>:\<Delta\><around*|(|c<rsub|4>-c<rsub|2>-x<rsub|min>|)>-\<Delta\><around*|(|c<rsub|4>-c<rsub|2>-x<rsub|5>|)>+\<Delta\><around*|(|c<rsub|5>-c<rsub|2>-x<rsub|5>|)>>

    \;

    <\math>
      x<rsub|max>\<leftarrow\><around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>,y<rsub|min>\<leftarrow\><around*|\<lfloor\>|n/x<rsub|max>|\<rfloor\>>,x<rsub|min>\<leftarrow\>min<around*|(|*<around*|\<lceil\>|C<rsub|1>*<sqrt|2*n|3>|\<rceil\>>,x<rsub|max>|)><with|font-series|bold|>
    </math>

    <math|s\<leftarrow\>0,a<rsub|2>\<leftarrow\>1,x<rsub|2>\<leftarrow\>x<rsub|max>,y<rsub|2>\<leftarrow\>y<rsub|min>,c<rsub|2>\<leftarrow\>a<rsub|2>*x<rsub|2>+y<rsub|2>>

    <with|font-series|bold|loop>

    <\indent>
      <\math>
        a<rsub|1>\<leftarrow\>a<rsub|2>+1
      </math>

      <math| x<rsub|4>\<leftarrow\><around*|\<lfloor\>|<sqrt|<around*|\<lfloor\>|n/a<rsub|1>|\<rfloor\>>>|\<rfloor\>>,y<rsub|4>\<leftarrow\><around*|\<lfloor\>|n/x<rsub|4>|\<rfloor\>>,c<rsub|4>\<leftarrow\>a<rsub|1>*x<rsub|4>+y<rsub|4>>

      <math|x<rsub|5>\<leftarrow\>x<rsub|4>+1,y<rsub|5>\<leftarrow\><around*|\<lfloor\>|n/x<rsub|5>|\<rfloor\>>,c<rsub|5>\<leftarrow\>a<rsub|1>*x<rsub|5>+y<rsub|5>>

      <with|font-series|bold|if> <math|x<rsub|4>\<less\>x<rsub|min>>
      <with|font-series|bold|then> <with|font-series|bold|exit>
      <with|font-series|bold|loop> <with|font-series|bold|end>
      <with|font-series|bold|if>

      <math|s\<leftarrow\>s+S<rsub|M><around*|(||)>+S<rsub|R><around*|(|a<rsub|1>*x<rsub|2>+y<rsub|2>-c<rsub|5>,a<rsub|2>*x<rsub|5>+y<rsub|5>-c<rsub|2>,a<rsub|1>,1,c<rsub|5>,a<rsub|2>,1,c<rsub|2>|)>>

      <math|a<rsub|2>\<leftarrow\>a<rsub|1>,x<rsub|2>\<leftarrow\>x<rsub|4>,y<rsub|2>\<leftarrow\>y<rsub|4>,c<rsub|2>\<leftarrow\>c<rsub|4>>
    </indent>

    <with|font-series|bold|end> <with|font-series|bold|loop>

    <math|s\<leftarrow\>s+S<rsub|1><around*|(||)>+S<rsub|2><around*|(||)>+S<rsub|3><around*|(||)>>

    <with|font-series|bold|return> <math|2*s-x<rsub|max><rsup|2>>

    \;

    <with|font-series|bold|function> <math|S<rsub|R><around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>

    <\indent>
      <math|\<Delta\><around*|(|i|)>:i*<around*|(|i+1|)>/2>

      <math|H<around*|(|u,v|)>:<around*|(|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>|)>*<around*|(|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>|)>>

      <math|U<rsub|tan><around*|(||)>:<around*|\<lfloor\>|<sqrt|<around*|\<lfloor\>|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>+2*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>|)><rsup|2>*n/<around*|(|a<rsub|3>*<space|0.25spc>b<rsub|3>|)>|\<rfloor\>>>|\<rfloor\>>-c<rsub|1>>

      <math|V<rsub|floor><around*|(|u|)>:<around*|\<lfloor\>|<around*|(|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-<around*|\<lceil\>|<sqrt|<around*|(|u+c<rsub|1>|)><rsup|2>-4*<space|0.25spc>a<rsub|1>*<space|0.25spc>b<rsub|1>*<space|0.25spc>n>|\<rceil\>>|)>/<around*|(|2*a<rsub|1*>*b<rsub|1>|)>|\<rfloor\>>-c<rsub|2>>

      <math|U<rsub|floor><around*|(|v|)>:<around*|\<lfloor\>|<around*|(|<around*|(|a<rsub|1>*<space|0.25spc>b<rsub|2>+b<rsub|1>*<space|0.25spc>a<rsub|2>|)>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-<around*|\<lceil\>|<sqrt|<around*|(|v+c<rsub|2>|)><rsup|2>-4*<space|0.25spc>a<rsub|2>*<space|0.25spc>b<rsub|2>*<space|0.25spc>n>|\<rceil\>>|)>/<around*|(|2*a<rsub|2*>*b<rsub|2>|)>|\<rfloor\>>-c<rsub|2>>

      <math|S<rsub|W><around*|(||)>:<big|sum><rsub|u=1><rsup|u\<less\>w>V<rsub|floor><around*|(|u|)>>

      <math|S<rsub|H><around*|(||)>:<big|sum><rsub|v=1><rsup|v\<less\>h>U<rsub|floor><around*|(|v|)>>

      <math|S<rsub|N><around*|(||)>:\<Delta\><around*|(|v<rsub|6>-1|)>-\<Delta\><around*|(|v<rsub|6>-u<rsub|5>|)>+\<Delta\><around*|(|u<rsub|7>-u<rsub|5>|)>>

      \;

      <math|s\<leftarrow\>0,a<rsub|3>\<leftarrow\>a<rsub|1>+a<rsub|2>,b<rsub|3>\<leftarrow\>b<rsub|1>+b<rsub|2>>

      <with|font-series|bold|if> <math|h\<gtr\>0\<wedge\>H<around*|(|w,1|)>\<leq\>n>
      <with|font-series|bold|then> <math|s\<leftarrow\>s+w,c<rsub|2>\<leftarrow\>c<rsub|2>+1,h\<leftarrow\>h-1>
      <with|font-series|bold|end> i<with|font-series|bold|f>

      <with|font-series|bold|if> <math|w\<gtr\>0\<wedge\>H<around*|(|1,h|)>\<leq\>n>
      <with|font-series|bold|then> <math|s\<leftarrow\>s+h,c<rsub|1>\<leftarrow\>c<rsub|1>+1,w\<leftarrow\>w-1>
      <with|font-series|bold|end> i<with|font-series|bold|f>

      <with|font-series|bold|if> <math|w\<leq\>C<rsub|2>>
      <with|font-series|bold|then> <with|font-series|bold|return>
      <math|s+S<rsub|W><around*|(||)>> <with|font-series|bold|end>
      <with|font-series|bold|if>

      <with|font-series|bold|if> <math|h\<leq\>C<rsub|2>>
      <with|font-series|bold|then> <with|font-series|bold|return>
      <math|s+S<rsub|H><around*|(||)>> <with|font-series|bold|end>
      <with|font-series|bold|if>

      <math|u<rsub|4>\<leftarrow\>U<rsub|tan><around*|(||)>,v<rsub|4>\<leftarrow\>V<rsub|floor><around*|(|u<rsub|4>|)>,u<rsub|5>\<leftarrow\>u<rsub|4>+1,v<rsub|5>\<leftarrow\>V<rsub|floor><around*|(|u<rsub|5>|)>>

      <\math>
        v<rsub|6>\<leftarrow\>u<rsub|4>+v<rsub|4>,u<rsub|7>\<leftarrow\>u<rsub|6>+v<rsub|6>
      </math>

      <math|s\<leftarrow\>s+S<rsub|N><around*|(||)>>

      <math|s\<leftarrow\>s+><math|S<rsub|R><around*|(|u<rsub|4>,h-v<rsub|6>,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+v<rsub|6>|)>>

      <math|s\<leftarrow\>s+><math|S<rsub|R><around*|(|w-u<rsub|7>,v<rsub|5>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+u<rsub|7>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>

      <with|font-series|bold|return> s
    </indent>

    <with|font-series|bold|end> <with|font-series|bold|function>
  </algorithm>

  <label|algorithm1>

  The next algorithm gives a flavor for the optimizations that are available.
  \ It computes the manual summation of a small region over <math|u> or
  <math|v> using a handful of additions, one square root and one division per
  lattice column. A similar technique can be used to compute
  <math|V<rsub|floor>> for the adjacent values <math|u<rsub|4>> and
  <math|u<rsub|5>>. Making this portion of the computation faster favors
  larger values of <math|C<rsub|2>>, the cutoff for small regions. An analogy
  is that this step is faster for small regions in the same way that an
  insertion sort is faster than a quicksort for small arrays and the break
  even point can be determined experimentally.

  <\algorithm>
    <\math>
      S<rsub|W><around*|(||)>:S<rsub|I><around*|(|w,c<rsub|1>,c<rsub|2>,a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>,2*a<rsub|1>*b<rsub|1>|)>

      S<rsub|H><around*|(||)>:S<rsub|I><around*|(|h,c<rsub|2>,c<rsub|1>,a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2,>2*a<rsub|2>*b<rsub|2>|)>
    </math>

    \;

    <with|font-series|bold|function> <math|S<rsub|I><around*|(|i<rsub|max>,p<rsub|1>,p<rsub|2>,q,r|)>>

    <\indent>
      <math|s\<leftarrow\>0,A\<leftarrow\>p<rsub|1><rsup|2>-2*r*n,B\<leftarrow\>p<rsub|1>*q,C\<leftarrow\>2*p<rsub|1>-1>

      <with|font-series|bold|for> <math|i=1,\<ldots\>,i<rsub|max>-1>
      <with|font-series|bold|do>

      <\indent>
        <math|C\<leftarrow\>C+2,A\<leftarrow\>A+C,B\<leftarrow\>B+q>
      </indent>

      <\indent>
        <math|s\<leftarrow\>s+<around*|\<lfloor\>|<around*|(|B-<around*|\<lceil\>|<sqrt|A>|\<rceil\>>|)>/r|\<rfloor\>>>
      </indent>

      <with|font-series|bold|end> <with|font-series|bold|for>

      <with|font-series|bold|return> <math|s-<around*|(|i<rsub|max>-1|)>*p<rsub|2>>
    </indent>

    <with|font-series|bold|end> <with|font-series|bold|function>
  </algorithm>

  The next algorithm formalizes the steps of the division-free counting
  method which can be used for the summation <math|S<rsub|1>>. \ Whether this
  is actually faster depends on many things but for example if
  <math|n\<less\>2<rsup|94>>, then <math|\<beta\>,\<delta\>,<around*|\||\<gamma\>|\|>,<around*|\||\<varepsilon\>|\|>\<less\>2<rsup|63>>
  for <math|2<rsup|32>\<less\>x\<less\>2<rsup|47>> and if signed 64-bit
  addition is a single-cycle operation, then a computation of <math|\<beta\>>
  using this method is about ten cycles vs. s a hundred \ cycles for a single
  multi-precision division.

  <\algorithm>
    <math|S<rsub|1><around*|(||)>:S<rsub|Q><around*|(|1,x<rsub|min>-1|)>>

    \;

    <with|font-series|bold|function> <math|S<rsub|Q><rsub|><around*|(|x<rsub|1>,x<rsub|2>|)>>

    <\indent>
      <math|s\<leftarrow\>0,x\<leftarrow\>x<rsub|2>,\<beta\>\<leftarrow\><around*|\<lfloor\>|n/<around*|(|x<rsub|>+1|)>|\<rfloor\>>,\<varepsilon\>\<leftarrow\>n
      <with|font-series|bold|mod <around*|(|x<rsub|>+1|)>>,\<delta\>\<leftarrow\><around*|\<lfloor\>|n/x<rsub|>|\<rfloor\>>-\<beta\>,\<gamma\>\<leftarrow\>\<beta\>-x<rsub|>*\<delta\>>

      <with|font-series|bold|while> <math|x\<geq\>x<rsub|1>>
      <with|font-series|bold|do>

      <\indent>
        <\math>
          \<varepsilon\>\<leftarrow\>\<varepsilon\>+\<gamma\>
        </math>

        <with|font-series|bold|if> <math|\<varepsilon\>\<geq\>x>
        <with|font-series|bold|then>

        <\indent>
          <math|\<delta\>\<leftarrow\>\<delta\>+1,\<gamma\>\<leftarrow\>\<gamma\>-x,\<varepsilon\>\<leftarrow\>\<varepsilon\>-x>

          <with|font-series|bold|if> <math|\<varepsilon\>\<geq\>x>
          <with|font-series|bold|then>

          <\indent>
            <math|\<delta\>\<leftarrow\>\<delta\>+1,\<gamma\>\<leftarrow\>\<gamma\>-x,\<varepsilon\>\<leftarrow\>\<varepsilon\>-x>
          </indent>

          <\indent>
            <with|font-series|bold|if> <math|\<varepsilon\>\<geq\>x>
            <with|font-series|bold|then> <with|font-series|bold|exit>
            <with|font-series|bold|while> <with|font-series|bold|end>
            <with|font-series|bold|if>
          </indent>

          <with|font-series|bold|end> <with|font-series|bold|if>
        </indent>

        <with|font-series|bold|else> <with|font-series|bold|if>
        <math|\<varepsilon\>\<less\>0> <with|font-series|bold|then>

        <\indent>
          <\math>
            \<delta\>\<leftarrow\>\<delta\>-1,\<gamma\>\<leftarrow\>\<gamma\>+x,\<varepsilon\>\<leftarrow\>\<varepsilon\>+x
          </math>
        </indent>

        <with|font-series|bold|end> <with|font-series|bold|if>

        <math|\<gamma\>\<leftarrow\>\<gamma\>+2*\<delta\>,\<beta\>\<leftarrow\>\<beta\>+\<delta\>,s\<leftarrow\>s+\<beta\>,x\<leftarrow\>x-1>
      </indent>

      <with|font-series|bold|end> <with|font-series|bold|while>

      <\math>
        \<varepsilon\>\<leftarrow\>n mod <with|font-series|bold|<around*|(|x<rsub|>+1|)>*>,\<delta\>\<leftarrow\><around*|\<lfloor\>|n/x<rsub|>|\<rfloor\>>-\<beta\>,\<gamma\>\<leftarrow\>\<beta\>-x<rsub|>*\<delta\>
      </math>

      <with|font-series|bold|while> <math|x\<geq\>x<rsub|1>>
      <with|font-series|bold|do>

      <\indent>
        <\math>
          \<varepsilon\>\<leftarrow\>\<varepsilon\>+\<gamma\>,\<delta\><rsub|2>\<leftarrow\><around*|\<lfloor\>|\<varepsilon\>/x|\<rfloor\>>,\<delta\>\<leftarrow\>\<delta\>+\<delta\><rsub|2>,\<varepsilon\>\<leftarrow\>\<varepsilon\>-x*\<delta\><rsub|2>
        </math>

        <math|\<gamma\>\<leftarrow\>\<gamma\>+2*\<delta\>-x*\<delta\><rsub|2>,\<beta\>\<leftarrow\>\<beta\>+\<delta\>,s\<leftarrow\>s+\<beta\>,x\<leftarrow\>x-1>
      </indent>

      <with|font-series|bold|end> <with|font-series|bold|while>

      <with|font-series|bold|while> <math|x\<geq\>x<rsub|1>>
      <with|font-series|bold|do>

      <\indent>
        <math|s\<leftarrow\>s+<around*|\<lfloor\>|n/x|\<rfloor\>>,x\<leftarrow\>x-1>
      </indent>

      <with|font-series|bold|end> <with|font-series|bold|while>

      <with|font-series|bold|return> s
    </indent>

    <with|font-series|bold|end> <with|font-series|bold|function>
  </algorithm>

  <section|Time and Space Complexity>

  Now we present an analysis of the runtime behavior of algorithm.

  <\theorem>
    The time complexity of algorithm [<reference|algorithm1>] when computing
    <math|T<around*|(|n|)>> is <math|O<around*|(|n<rsup|1/3>|)>> and the
    space complexity is <math|O<around*|(|log n|)>>.
  </theorem>

  Before we start, we realize that because
  <math|x<rsub|min>=O<around*|(|n<rsup|1/3>|)>> and we handle the values of
  <math|1\<leq\>x\<less\>x<rsub|min>> manually, the algorithm is at best
  <math|O<around*|(|n<rsup|1/3>|)>>. In this section we desire to show that
  the rest of the computation is at worst <math|O<around*|(|n<rsup|1/3>|)>>
  so that this lower bound holds for the entire computation.

  Our first task is to count and size all the top-level regions. We process
  one top level region for each integral slope <math|-a> from <math|-1 > to
  the slope at <math|x<rsub|min>>. The value for <math|a> at each value of
  <math|x> is given by:

  <\equation>
    a<rsub|>=-<frac|d |d x>Y<around*|(|x<rsub|>|)>=<frac|n|x<rsup|2>>
  </equation>

  and:

  <\equation>
    X<around*|(|a|)>=<sqrt|<frac|n|a>>
  </equation>

  \ Choosing <math|C<rsub|1>=1> so that <math|x<rsub|min>=<sqrt|2*n|3>>, then
  the highest value of <math|a> processed is:

  <\equation>
    a<rsub|max>=<frac|n|x<rsub|min><rsup|2>>=<frac|n<rsup|1/3>|2<rsup|2/3>>
  </equation>

  so there are <math|O<around*|(|n<rsup|1/3>|)>> top level regions.

  How big is each top level region? The change in <math|x> per unit change in
  <math|a> is <math|d x/d a> and so:

  <\equation>
    A=-<frac|d|d a>X<around*|(|a|)>=<frac|n<rsup|1/2>|2*a<rsup|3/2>>
  </equation>

  Assume for the moment that the number of total regions visited while
  processing a region of size <math|A> is:

  <\equation*>
    N<around*|(|A|)>=O<around*|(|A<rsup|G>|)>
  </equation*>

  noting that the cost of processing a region (excluding the cost of
  processing its sub-regions) is <math|O<around*|(|1|)>> and so the total
  number of regions is representative of the total cost.

  Now we sum the number of sub-regions processed across all top level region:

  <\eqnarray*>
    <tformat|<table|<row|<cell|N<rsub|total>=<big|sum><rsup|a<rsub|max>><rsub|a=2>N<around*|(|A|)>>|<cell|=>|<cell|O<around*|(|<big|int><rsup|a<rsub|max>><rsub|1>N<around*|(|A|)>
    d a|)>>>|<row|<cell|>|<cell|=>|<cell|O<around*|(|<big|int><rsub|1><rsup|a<rsub|max>><around*|(|<frac|n<rsup|1/2>|2*a<rsup|3/2>>|)><rsup|G>d
    a|)><eq-number>>>>>
  </eqnarray*>

  We can classify three cases depending on the value of <math|G> because the
  outcome of the integration depends on the final exponent of <math|a>:

  <\equation*>
    N<rsub|total>=<choice|<tformat|<table|<row|<cell|O<around*|(|n<rsup|1/3>|)>>|<cell|if
    G \<less\>2/3;>>|<row|<cell|O<around*|(|n<rsup|1/3>*log n|)>>|<cell|if
    G=2/3;>>|<row|<cell|O<around*|(|n<rsup|G/2>|)>>|<cell|if G \<gtr\>
    2/3.>>>>>
  </equation*>

  (Note that we cannot get below <math|O<around*|(|n<rsup|1/3>|)>> even if
  <math|G=0> because we have at least <math|a<rsub|max>=O<around*|(|n<rsup|1/3>|)>>
  top level regions.)

  Now let us analyze the exponent in <math|N<around*|(|A|)>>. In order to
  determine the number of regions encountered in the course of processing a
  region of size <math|A>, we need to analyze the recursion depth. The
  recursion will terminate when <math|w> or <math|h> is unity because by our
  conditions it is then impossible for the region to contain any more lattice
  points. Our next task is to measure the size of such a region and so we
  need to know how many <math|x> lattice columns that terminal region
  represents.

  We can use the transformation between <math|u v> and <math|x y> coordinates
  given by (<reference|eq:uv2xy1>) to compute the difference between the
  <math|x> coordinates of <math|P<rsub|2>> at <math|<around*|(|1,0|)>> and
  <math|P<rsub|1>> at <math|<around*|(|0,1|)>>, assuming the smallest case
  with <math|w=h=1>:

  <\equation>
    \<Delta\>x=x<rsub|2>-x<rsub|1>+1\<geq\><around*|(|x<rsub|0>+1\<cdot\>b<rsub|2>-0\<cdot\>b<rsub|1>|)>-<around*|(|x<rsub|0>+0\<cdot\>b<rsub|2>-1\<cdot\>b<rsub|1>|)>+1=b<rsub|1>+b<rsub|2>+1\<gtr\>b<rsub|1>+b<rsub|2>
  </equation>

  so the size of a terminal region is greater than the sum of the
  denominators of the slopes of the two lines that define it.

  Each time we recurse into two new regions we add a new extended Farey
  fraction that is the mediant of the two slopes for the outer region. As a
  result, we perform a partial traversal of a Stern-Brocot tree, doubling the
  number of nodes at each level. However, for our current purposes we can
  ignore the numerators because we are interested in the sum of denominators.
  \ Because regions cannot overlap, this means that the sum of the
  denominators at the deepest level of the tree cannot exceed the size of the
  first region and that only denominators affect the recursion depth.

  Next we need to derive a formula for the sum of the denominators of a
  partial Stern-Brocot tree of depth <math|D>. For example, if the first node
  <math|<around*|(|a<rsub|1>/b<rsub|1>,a<rsub|2>/b<rsub|2>|)>> is
  <math|<around*|(|2/1,1/1|)>>, the next two nodes are
  <math|<around*|(|2/1,3/2|)>> and <math|<around*|(|3/2,1/1|)>>. Continuing
  and ignoring numerators we have the following
  <math|<around*|(|b<rsub|1>,b<rsub|2>|)>> tree:

  <with|gr-mode|<tuple|edit|line>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|empty>|gr-grid-old|<tuple|cartesian|<point|0|0>|1>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|empty>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|1>|gr-auto-crop|true|<graphics||<math-at|<around*|(|2,3|)>|<point|-3.0|1.0>>|<math-at|<around*|(|3,1|)>|<point|-2.0|1.0>>|<math-at|<around*|(|3,2|)>|<point|-4.0|1.0>>|<math-at|<around*|(|1,3|)>|<point|-5.0|1.0>>|<math-at|<around*|(|2,1|)>|<point|-2.4|2.0>>|<math-at|<around*|(|1,2|)>|<point|-4.3|2.0>>|<line|<point|-3.2|2.79425>|<point|-3.6|2.4>>|<line|<point|-2.90807|2.79425>|<point|-2.2|2.38212726551131>>|<line|<point|-4.2|1.79425>|<point|-4.60806654319354|1.38212726551131>>|<line|<point|-3.90807|1.79425>|<point|-3.60806654319354|1.38212726551131>>|<line|<point|-1.8|1.79425>|<point|-1.60806654319354|1.38212726551131>>|<math-at|<around*|(|1,1|)>|<point|-3.4|3.0>>|<line|<point|-2.2|1.79425188517>|<point|-2.60806654319354|1.38212726551131>>|<line|<point|-4.84481|0.794252>|<point|-5.33303016271994|0.122502976584204>>|<line|<point|-4|0.2>|<point|-3.60806654319354|0.794251885169996>>|<line|<point|-3.4|0.2>|<point|-3.60806654319354|0.794251885169996>>|<line|<point|-3|0.3>|<point|-2.60806654319354|0.794251885169996>>|<line|<point|-2.4|0.2>|<point|-2.60806654319354|0.794251885169996>>|<line|<point|-2|0.3>|<point|-1.60806654319354|0.794251885169996>>|<line|<point|-1.3|0.3>|<point|-1.60806654319354|0.794251885169996>>|<line|<point|-4.5|0.2>|<point|-4.84481008345725|0.794251885169996>>>>

  At each new level we have twice as many nodes and half of the numbers are
  duplicated from the previous level and the other half of the numbers are
  the sum of numbers of their parent node. Since each parent's sum
  contributes to exactly two numbers in the children, the sum of the
  denominators at each level is triple the sum of the previous level. So
  staring with <math|1+1=2> leads to the sequence <math|2,6,18,54,\<ldots\>>,
  and denoting by <math|\<Omega\>> the set of terminal regions, the sum at
  depth <math|D> is therefore

  <\equation*>
    A\<gtr\><big|sum><rsub|R:R\<in\>\<Omega\>><rsub|>b<rsub|1>+b<rsub|2>=2*3<rsup|D>.
  </equation*>

  Because the number of terminal regions is
  <math|<around*|\||\<Omega\>|\|>=2<rsup|D>>, we can now place a bound on
  <math|<around*|\||\<Omega\>|\|>> in terms of <math|A>:

  <\equation*>
    <around*|\||\<Omega\>|\|>\<less\><around*|(|<frac|A|2>|)><rsup|1/log<rsub|2>
    3>.
  </equation*>

  Finally, since the total number of regions is
  <math|1+2+4+\<ldots\>+<around*|\||\<Omega\>|\|>=<big|sum><rsup|D><rsub|i=1>2<rsup|i>>,
  the number of regions as a function of the size <math|A> is

  <\equation>
    N<around*|(|A|)>=2*<around*|\||\<Omega\>|\|><rsub|>-1=O<around*|(|A<rsup|1/log<rsub|2>
    3>|)>
  </equation>

  and therefore <math|G=1/log<rsub|2> 3>.

  Since <math|1/log<rsub|2> 3\<approx\>0.63>, this means that
  <math|G\<less\>2/3> and the proof that the overall time complexity of the
  algorithm is <math|O<around*|(|n<rsup|1/3>|)>> is complete.

  The space complexity is simply our recursion depth which can be at most
  <math|O<around*|(|log n|)>>.

  <section|Higher-Order Divisor Sums>

  The two-dimensional hyperbola and the functions
  <math|\<tau\><around*|(|n|)>> and <math|T<around*|(|n|)>> can be
  generalized to higher dimensions. The divisor sum
  <math|T<rsub|3><around*|(|n|)>>, the summatory function for
  <math|\<tau\><rsub|3><around*|(|x|)>=<big|sum><rsub|a*b<around*|\||x|\<nobracket\>>>1>,
  can be computed by summing under the three-dimensional hyperbola.

  <\equation*>
    T<rsub|3><around*|(|n|)>=<big|sum><rsub|x,y,z:x*y*z\<leq\>n>1=<big|sum><rsub|z=1><rsup|n><big|sum><rsub|x=1><rsup|n><around*|\<lfloor\>|<frac|n|x*z>|\<rfloor\>>=<big|sum><rsub|z=1><rsup|n>T<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>.
  </equation*>

  Again using the symmetry of this hyperbola we can restrict the outer
  summation to <math|<sqrt|n|3>> by counting nested ``shells'', and avoiding
  double and triple counting, we get

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|[|3*<around*|(|2*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>><rsub|x=z+1><around*|(|<around*|\<lfloor\>|<frac|n/z|x>|\<rfloor\>>-z|)>-<around*|(|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>-z|)><rsup|2>+<around*|(|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>-z|)>|)>+1|]>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|[|3*<around*|(|2*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>><rsub|x=z+1><around*|\<lfloor\>|<frac|n/z|x>|\<rfloor\>>-2*z*<around*|(|<sqrt|<frac|n|z>>-z|)>-<around*|(|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>-2*z*<sqrt|<frac|n|z>>+z<rsup|2>|)>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>-z|)>+1|]>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|[|3*<around*|(|2*S<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+1,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+z<rsup|2>-z|)>+1|]>>>|<row|<cell|>|<cell|=>|<cell|3*<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|(|2*S<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+1,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>|)>+<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>><rsup|3>>>>>
  </eqnarray*>

  where in the last step we use the identity
  <math|<big|sum><rsub|z=1><rsup|k>3*<around*|(|z<rsup|2>-z|)>+1=3*<around*|(|k*<around*|(|k+1|)>*<around*|(|2*k+1|)>/6-k*<around*|(|k+1|)>/2|)>+k=k<rsup|3>>.
  Since <math|S<around*|(|n,x<rsub|1>,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>>
  is a partial result in the calculation of <math|T<around*|(|n|)>>, it is
  also has <math|O<around*|(|n<rsup|1/3>|)>> time complexity when using
  Algorithm [<reference|algorithm1>]. \ As a result, we can calculate
  <math|T<rsub|3><around*|(|n|)>> in

  <\equation*>
    <big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><rsub|z=1>O<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>><rsup|1/3>|)>=O<around*|(|<big|int><rsub|1><rsup|n<rsup|1/3>><frac|n<rsup|1/3>|z<rsup|1/3>><rsup|>
    d z|)>=O<around*|(|n<rsup|5/9>|)>,
  </equation*>

  a modest improvement over <math|O<around*|(|n<rsup|2/3>|)>> using a direct
  double summation. \ Similar derivations give
  <math|O<around*|(|n<rsup|2/3>|)>> for <math|T<rsub|4><around*|(|n|)>> and
  <math|O<around*|(|n<rsup|11/15>|)>> for <math|T<rsub|5><around*|(|n|)>> or
  <math|O<around*|(|n<rsup|1-4/<around*|(|3*k|)>>|)>> for
  <math|T<rsub|k><around*|(|n|)>> in general.

  <section|Remarks>

  It would be possible to simplify the algorithm somewhat by removing the
  distinction between top level regions and region processing itself by
  starting with the region defined by <math|<around*|(|1/0,1/1|)>>. The
  reason for the current assymetry is to capitalize on the simpler <math|x y>
  coordinate system where possible.

  The two major sections of the algorithm, <math|S<rsub|1>> and
  <math|S<rsub|4>>, are easily parallelizable. The section <math|S<rsub|1>>
  can divide summation batches to different processors. The section
  <math|S<rsub|4>> can be revised to use a work queue of regions instead of
  recursion. During region processing, one region can be enqueued and the
  other processed iteratively. Available processors can dequeue regions that
  need to be processed.

  <section|Related Work>

  In [<reference|bib:Gal00>], Galway presents an improved sieving algorithm
  that also features region decomposition based on extended Farey fractions
  as well as coordinate transformation. In [<reference|bib:Tao11>],
  applications for the divisor summatory are presented including computing
  the parity of <math|\<pi\><around*|(|x|)>>, the prime counting function, as
  well as a sketch for a different <math|O<around*|(|n<rsup|1/3>|)>>
  algorithm. In [<reference|bib:Sil12>], the parity of the prime counting
  function is studied more closely and several related algorithms are
  developed.

  <\bibliography|bib|tm-plain|hyperbola.bib>
    \;

    <\bib-list|>
      <bibitem|Vor03><label|bib:Vor03>Georges Voronoï,
      <with|font-shape|italic|Sur un problème du calcul des fonctions
      asymptotiques>, J. Reine Angew. Math. <with|font-series|bold|126>
      (1903), 241-282.

      <bibitem|Bres77><label|bib:Bres77>Jack Bresenham,
      <with|font-shape|italic|A linear algorithm for incremental digital
      display of circular arcs>, Communications of the ACM
      <with|font-series|bold|20> (1977), 100-106.

      <bibitem|Gal00><label|bib:Gal00>William F. Galway,
      <with|font-shape|italic|Dissecting a Sieve to Cut Its Need for Space>,
      In Proceedings of ANTS. (2000), 297-312.

      <bibitem|Tao11><label|bib:Tao11>Terence Tao, Ernest Croot III, and
      Harald Helfgott. <with|font-shape|italic|Deterministic methods to find
      primes>. Mathematics of Computation, 2011. Published electronically on
      August 23, 2011.

      <bibitem|Sil12><label|bib:Sil12>Tomás Oliveira e Silva,
      <with|font-shape|italic|Efficient Computation of the Parity of the
      Prime Counting Function>, in preparation.
    </bib-list>
  </bibliography>
</body>

<\references>
  <\collection>
    <associate|abc|<tuple|6|?>>
    <associate|algorithm1|<tuple|1|11>>
    <associate|auto-1|<tuple|1|1>>
    <associate|auto-10|<tuple|10|14>>
    <associate|auto-11|<tuple|10|?>>
    <associate|auto-2|<tuple|2|1>>
    <associate|auto-3|<tuple|3|2>>
    <associate|auto-4|<tuple|4|7>>
    <associate|auto-5|<tuple|5|9>>
    <associate|auto-6|<tuple|6|10>>
    <associate|auto-7|<tuple|7|12>>
    <associate|auto-8|<tuple|8|14>>
    <associate|auto-9|<tuple|9|14>>
    <associate|bib-|<tuple||?>>
    <associate|bib-A|<tuple|A|?>>
    <associate|bib-B|<tuple|B|?>>
    <associate|bib-Br|<tuple|Br|?>>
    <associate|bib-Bre|<tuple|Bre|?>>
    <associate|bib-Bres|<tuple|Bres|?>>
    <associate|bib-Bres7|<tuple|Bres7|?>>
    <associate|bib-Bres77|<tuple|Bres77|14>>
    <associate|bib-G|<tuple|G|?>>
    <associate|bib-Ga|<tuple|Ga|?>>
    <associate|bib-Gal|<tuple|Gal|?>>
    <associate|bib-Gal0|<tuple|Gal0|?>>
    <associate|bib-Gal00|<tuple|Gal00|14>>
    <associate|bib-S|<tuple|S|?>>
    <associate|bib-Si|<tuple|Si|?>>
    <associate|bib-Sil|<tuple|Sil|?>>
    <associate|bib-Sil1|<tuple|Sil1|?>>
    <associate|bib-Sil12|<tuple|Sil12|14>>
    <associate|bib-T|<tuple|T|?>>
    <associate|bib-Ta|<tuple|Ta|?>>
    <associate|bib-Tao|<tuple|Tao|?>>
    <associate|bib-Tao1|<tuple|Tao1|?>>
    <associate|bib-Tao11|<tuple|Tao11|14>>
    <associate|bib-V|<tuple|V|?>>
    <associate|bib-Vo|<tuple|Vo|?>>
    <associate|bib-Vor|<tuple|Vor|?>>
    <associate|bib-Vor0|<tuple|Vor0|?>>
    <associate|bib-Vor03|<tuple|Vor03|14>>
    <associate|bib-[|<tuple|[|?>>
    <associate|bib:Bres77|<tuple|Bres77|14>>
    <associate|bib:Gal00|<tuple|Gal00|14>>
    <associate|bib:Sil12|<tuple|Sil12|14>>
    <associate|bib:Tao11|<tuple|Tao11|14>>
    <associate|bib:Vor03|<tuple|Vor03|14>>
    <associate|eq:det|<tuple|9|3>>
    <associate|eq:gcd1|<tuple|10|?>>
    <associate|eq:ps1|<tuple|11|3>>
    <associate|eq:ps2|<tuple|12|3>>
    <associate|eq:uv2xy1|<tuple|20|4>>
    <associate|eq:uv2xy2|<tuple|21|4>>
    <associate|eq:uv2xy3|<tuple|22|4>>
    <associate|eq:uv2xy4|<tuple|23|4>>
    <associate|eq:xy0|<tuple|10|3>>
    <associate|eq:xy2uv1|<tuple|24|4>>
    <associate|eq:xy2uv2|<tuple|25|4>>
    <associate|uv2xy1|<tuple|22|?>>
    <associate|uv2xy2|<tuple|23|?>>
    <associate|uv2xy3|<tuple|24|?>>
    <associate|uv2xy4|<tuple|25|?>>
  </collection>
</references>

<\auxiliary>
  <\collection>
    <\associate|toc>
      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|1<space|2spc>Introduction>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-1><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|2<space|2spc>Preliminaries>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-2><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|3<space|2spc>Region
      Processing> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-3><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|4<space|2spc>Top
      Level Processing> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-4><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|5<space|2spc>Division-Free
      Counting> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-5><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|6<space|2spc>Algorithms>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-6><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|7<space|2spc>Time
      and Space Complexity> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-7><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|8<space|2spc>Remarks>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-8><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|9<space|2spc>Related
      Work> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-9><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Bibliography>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-10><vspace|0.5fn>
    </associate>
  </collection>
</auxiliary>