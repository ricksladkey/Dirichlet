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
    elementary and uses a geometric approach of successive approximation
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

  which gives an <math|O<around*|(|n|)>> algorithm. \ By using the symmetry
  of the hyperbola (and taking care to avoid double counting) we can do this
  even more efficiently:

  <\equation>
    T<around*|(|n|)>=2<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation>

  \ which gives an <math|O<around*|(|n<rsup|1/2>|)>> algorithm and is in fact
  the usual method by which the divisor summatory function is computed.

  <section|Preliminaries>

  It will be convenient to parameterize sum in <math|T<around*|(|n|)>> as:

  <\equation>
    S<around*|(|i,j|)>=<big|sum><rsup|j><rsub|x=i+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
  </equation>

  so that:

  <\equation>
    T<around*|(|n|)>=S<around*|(|0,n|)>=2*S<around*|(|0,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
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
  <math|<around*|(|0,0|)>,<around*|(|i,c*i|)>,<around*|(|i,<around*|(|c-1|)>*i|)>>,
  <math|c> an integer. \ If we desire to to omit the lattice points on two
  sides, we can use <math|\<Delta\><around*|(|i-1|)>> instead of
  <math|\<Delta\><around*|(|i|)>>.

  <section|Region Processing>

  Instead of considering all of the lattice points, let us for the moment
  focus on the subtask of counting the lattice points in a region bounded by
  two tangent lines and a segment of the hyperbola. If we can approximate the
  hyperbola by a series of tangent lines then the area below the lines is a
  simple polygon and can conceptually be calculated directly by decomposing
  the area into triangles.

  We will now go about counting the lattice points in such a region. We will
  do this by transforming the region into a new coordinate system.

  This figure depicts a region in the <math|x y> coordinate system:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|-100|-100>|2>|gr-grid-old|<tuple|cartesian|<point|-100|-100>|2>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|-100|-100>|2>|gr-edit-grid-old|<tuple|cartesian|<point|-100|-100>|2>|gr-auto-crop|true|<graphics||<line|<point|-2|0>|<point|2.0|-4.0>>|<line|<point|-2|0>|<point|-4.0|4.0>>|<spline|<point|2.28704699048648|-4.19999999999999>|<point|-0.200000000000003|-1.40000000000001>|<point|-3.0|2.40000000000001>|<point|-4.2|4.59999999999999>>|<math-at|P<rsub|0>|<point|-2.59999999999999|-0.400000000000006>>|<math-at|P<rsub|1>|<point|-4.4|3.4>>|<point|-4|4>|<point|-2|0>|<point|2|-4>|<math-at|P<rsub|2>|<point|1.2|-4>>|<math-at|H<around*|(|x,y|)>=n|<point|-1.4|0.8>>|<math-at|L<rsub|2>|<point|-1.2|-1.4>>|<math-at|L<rsub|1>|<point|-4|2.6>>|<math-at|-m<rsub|1>=<frac|a<rsub|1>|b<rsub|1>>|<point|-4.40000000000001|1.2>>|<math-at|-m<rsub|2>=<frac|a<rsub|2>|b<rsub|2>>|<point|-1.40000000000001|-2.59999999999999>>>>

  Define two lines <math|L<rsub|1>> and <math|L<rsub|2>> whose slopes when
  negated have positive integral numerators and denominators:\ 

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

  Notice the <math|x y> lattice points form another lattice relative to lines
  <math|L<rsub|1>> and <math|L<rsub|2>>:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|<graphics||<line|<point|-4|-2>|<point|-1.5|-4.5>>|<line|<point|-4.5|-1>|<point|-2.0|-3.5>>|<line|<point|-5|0>|<point|-2.5|-2.5>>|<line|<point|-6.5|3.0>|<point|-4.0|-2.0>>|<line|<point|-3.5|-2.5>|<point|-6.0|2.5>>|<line|<point|-3|-3>|<point|-5.5|2.0>>|<line|<point|-2.5|-3.5>|<point|-5.0|1.5>>|<line|<point|-2|-4>|<point|-4.5|1.0>>|<line|<point|-1.5|-4.5>|<point|-4.0|0.5>>|<line|<point|-6.5|3>|<point|-4.0|0.5>>|<point|-4|-2>|<point|-4.5|-1>|<point|-5|0>|<point|-5.5|1>|<point|-6|2>|<point|-6.5|3>|<point|-3.5|-2.5>|<point|-3|-3>|<point|-2.5|-3.5>|<point|-2|-4>|<point|-1.5|-4.5>|<point|-4|-1.5>|<point|-3.5|-2>|<point|-3|-2.5>|<point|-2.5|-3>|<point|-2|-3.5>|<point|-4.5|-0.5>|<point|-4|-1>|<point|-3.5|-1.5>|<point|-3|-2>|<point|-2.5|-2.5>|<line|<point|-5.5|1>|<point|-3.0|-1.5>>|<line|<point|-6|2>|<point|-3.5|-0.5>>|<point|-4.5|0.0>|<point|-5.0|0.5>|<point|-5.5|1.5>|<point|-6|2.5>|<point|-5.5|2>|<point|-5|1>|<point|-4.5|0.5>|<point|-4|0>|<point|-4|-0.5>|<point|-3.5|-1>|<point|-3.5|-0.5>|<point|-4|0.5>|<point|-4.5|1>|<point|-5|1.5>|<point|-3|-1.5>|<math-at|L<rsub|2>|<point|-3.65603525919218|-3.5>>|<math-at|L<rsub|1>|<point|-5.68321541904923|-0.5>>|<math-at|P<rsub|0>|<point|-4.5|-2.35754988898701>>>>

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
  axis and <math|u> and <math|v> increasing by one for each lattice point in
  the direction of the hyperbola. Then the conversion from the <math|u v>
  coordinates to <math|x y> coordinates is given by:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|x<rsub|0>+b<rsub|2>*<space|0.25spc>u-b<rsub|1>*<space|0.25spc>v<eq-number>>>|<row|<cell|y>|<cell|=>|<cell|y<rsub|0>-a<rsub|2>*<space|0.25spc>u
    +a<rsub|1>*<space|0.25spc>v<eq-number>>>>>
  </eqnarray*>

  Substituting for <math|x<rsub|0>> and <math|y<rsub|0>>:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x>|<cell|=>|<cell|b<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)>-b<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)><eq-number>>>|<row|<cell|y>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc><around*|(|v+c<rsub|2>|)>-a<rsub|2>*<space|0.25spc><around*|(|u+c<rsub|1>|)><eq-number>>>>>
  </eqnarray*>

  Solving these equations for <math|u> and <math|v> and substituting unity
  for the determinant provides the conversion from <math|x y> coordinates to
  <math|u v> coordinates:

  <\eqnarray*>
    <tformat|<table|<row|<cell|u>|<cell|=>|<cell|a<rsub|1>*<space|0.25spc>x+b<rsub|1>*<space|0.25spc>y-c<rsub|1><eq-number>>>|<row|<cell|v>|<cell|=>|<cell|a<rsub|2>*<space|0.25spc>x+b<rsub|2>*<space|0.25spc>y-c<rsub|2><eq-number>>>>>
  </eqnarray*>

  Because all quantities are integers, the equations above mean that each
  <math|x y> lattice point corresponds to a <math|u v> lattice point and vice
  versa. \ As a result, we can choose to count lattice points in either
  <math|x y> coordinates or <math|u v> coordinates.

  Now we are ready to transform the hyperbola into the <math|u v> coordinate
  system by substituting for <math|x> and <math|y> into
  <math|H<around*|(|x,y|)>> which gives:

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
  to the hyperbola increases as you approach the origin.

  With these constraints, the hyperbolic segment has the same basic shape as
  the full hyperbola: roughly tangent to the axes at the endpoints and
  monotonic in-between.

  This figure depicts a region in the <math|u v> coordinate system:

  <with|gr-mode|<tuple|edit|point>|gr-frame|<tuple|scale|1.18926cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.733333par|center>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|magnify|1.18920711391541|gr-magnify|1.18920711391541|<graphics||<point|0|0>|<point|0.0|3.5>|<point|3.5|0.0>|<math-at|P<rsub|2>|<point|3.5|-0.315875450190698>>|<with|color|dark
  grey|<line|<point|0|3.5>|<point|3.5|3.5>|<point|3.5|0.0>>>|<math-at|w|<point|1.67812|3.7323>>|<math-at|h|<point|3.71164|2>>|<math-at|u|<point|4.22327127104857|-0.250914248369648>>|<spline|<point|4.5|0.262528795121958>|<point|3.0|0.5>|<point|0.701313192297385|2.5>|<point|0.270375393271184|4.0>>|<math-at|H<around*|(|u,v|)>=n|<point|0.823796876955379|2.69183920352629>>|<math-at|v|<point|-0.411937664151288|4.0>>|<math-at|P<rsub|1>|<point|-0.5|3.34795412488904>>|<math-at|P<rsub|0>|<point|-0.5|-0.324619072766175>>>>

  We can now reformulate the number of lattice points in this region
  <math|R<rsub|> >as a function of the eight values that define it:

  <\equation>
    S<rsub|<rsub|R>>=S<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>
  </equation>

  If <math|H<around*|(|w,1|)>\<leqslant\>n>, then
  <math|v<rsub|w>\<geqslant\>1> and we can remove the first lattice row:

  <\equation>
    S<rsub|R>=S<around*|(|w,h-1,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>+1|)>+w
  </equation>

  and if <math|H<around*|(|1,h|)>\<leqslant\>n>, then
  <math|u<rsub|h>\<geqslant\>1> and we can remove the first lattice column:

  <\equation>
    S<rsub|R>=S<around*|(|w-1,h,a<rsub|1>,b<rsub|1>,c<rsub|1>+1,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>+h
  </equation>

  so that the conditions are satisified.

  Now we could count lattice points in the region bounded by the <math|u> and
  <math|v> axes and <math|u=w> and <math|v=h> using brute force:

  <\equation>
    S<rsub|R>=<big|sum><rsub|u,v:H<around*|(|u,v|)>\<leqslant\>n>1
  </equation>

  More efficiently, if we had a formulas for <math|u> and <math|v> in terms
  each other, we could sum columns of lattice points:

  <\eqnarray*>
    <tformat|<table|<row|<cell|>|<cell|S<rsub|w>=<big|sum><rsup|w><rsub|u=1><around*|\<lfloor\>|V<around*|(|u|)>|\<rfloor\>>>|<cell|<eq-number>>>|<row|<cell|>|<cell|S<rsub|h>=<big|sum><rsub|v=1><rsup|h><around*|\<lfloor\>|U<around*|(|v|)>|\<rfloor\>>>|<cell|<eq-number>>>>>
  </eqnarray*>

  using whichever axis has fewer points (keeping in mind that it could be
  assymmetric). \ These summations are certain not to overcount because by
  our conditions <math|V<around*|(|u|)>\<less\>h> for
  <math|0\<less\>u\<leqslant\>w> and <math|U<around*|(|v|)>\<less\>w> for
  <math|0\<less\>v\<leqslant\>h>.

  And so:

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|S<rsub|w><eq-number>>>|<row|<cell|>|<cell|=>|<cell|S<rsub|h>>>>>
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

  This figure shows the right triangle and the two sub-regions:

  <with|gr-mode|<tuple|edit|math-at>|gr-frame|<tuple|scale|1cm|<tuple|0.370012gw|0.270022gh>>|gr-geometry|<tuple|geometry|1par|100|center>|gr-grid|<tuple|cartesian|<point|0|0>|2>|gr-grid-old|<tuple|cartesian|<point|0|0>|2>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|2>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|2>|gr-auto-crop|true|<graphics||<with|color|dark
  grey|<line|<point|0|4.4>|<point|4.8|4.4>|<point|4.8|0.0>>>|<point|0|4.4>|<point|0|0>|<point|4.8|0>|<line|<point|0|2.6>|<point|2.4|0.0>>|<spline|<point|0.2|4.6>|<point|0.4|2.8>|<point|2.6|0.6>|<point|5.0|0.2>>|<math-at|R<rprime|''>|<point|2.4|0.2>>|<math-at|R<rprime|'>|<point|0.0|2.6>>|<math-at|P<rsub|0>|<point|-0.6|-0.4>>|<math-at|P<rsub|1>|<point|-0.6|4.2>>|<math-at|P<rsub|2>|<point|4.6|-0.4>>|<math-at|u|<point|5.4|0.2>>|<math-at|v|<point|-0.337032014816775|4.9255853948935>>|<math-at|h|<point|5|2.4>>|<math-at|w|<point|2.4|4.6>>|<math-at|u<rprime|'>|<point|0.2|1.8>>|<math-at|u<rprime|''>|<point|3.4|-0.4>>|<math-at|v<rprime|''>|<point|1.4|0.4>>|<math-at|v<rprime|'>|<point|-0.4|3.4>>|<math-at|R|<point|1.8|2.0>>>>

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

  (Note that <math|u<rsub|3>> and <math|v<rsub|3>> are real-valued.)

  The equation of a line through this intersection and tangent to the
  hyperbola is then <math|u+v=u<rsub|3>+v<rsub|3>> which simplifies to:

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

  A diagram of all the points defined so far:

  <with|gr-mode|<tuple|group-edit|props>|gr-frame|<tuple|scale|1.18926cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.733333par|center>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|magnify|1.18920711391541|gr-color|dark
  grey|<graphics||<point|0|0>|<point|0.0|3.5>|<point|1.5|1.0>|<line|<point|1.5|1>|<point|2.5|0.0>>|<point|2.5|0>|<point|3.5|0.0>|<math-at|P<rsub|2>|<point|3.5|-0.315875450190698>>|<math-at|P<rsub|5>|<point|1.0|0.691569794964324>>|<with|color|dark
  grey|<line|<point|0|3.5>|<point|3.5|3.5>|<point|3.5|0.0>>>|<math-at|w|<point|1.67812|3.7323>>|<math-at|h|<point|3.71164|2>>|<math-at|u|<point|4.22327127104857|-0.250914248369648>>|<spline|<point|4.5|0.262528795121958>|<point|3.0|0.5>|<point|0.701313192297385|2.5>|<point|0.270375393271184|4.0>>|<line|<point|1.0|2.0>|<point|0.0|3.0>>|<point|0.0|3.0>|<point|1.25659730989343|1.69647908332071>|<math-at|P<rsub|6>|<point|-0.5|2.68372631475282>>|<point|1.0|2.0>|<math-at|P<rsub|4>|<point|0.5|1.69113478621578>>|<math-at|P<rsub|3>|<point|1.5|1.76202136806692>>|<math-at|P<rsub|7>|<point|2.5|0.185032086664982>>|<math-at|H<around*|(|u,v|)>=n|<point|0.823796876955379|2.69183920352629>>|<math-at|v|<point|-0.411937664151288|4.0>>|<math-at|P<rsub|1>|<point|-0.5|3.34795412488904>>|<math-at|P<rsub|0>|<point|-0.5|-0.324619072766175>>|<math-at||<point|2|3.5>>>>

  Then the number of lattice points above the axes and inside the polygon
  defined by <math|P<rsub|0>,P<rsub|6>,P<rsub|4>,P<rsub|5>,P<rsub|7>> is:

  <\equation>
    S<rsub|T>=<choice|<tformat|<table|<row|<cell|\<Delta\><around*|(|v<rsub|6>-1|)>+u<rsub|4>>|<cell|if
    v<rsub|6>\<less\>u<rsub|7>>>|<row|<cell|\<Delta\><around*|(|v<rsub|6>-1|)>>|<cell|if
    v<rsub|6> = u<rsub|7>>>|<row|<cell|\<Delta\><around*|(|u<rsub|7>-1|)>+v<rsub|5>>|<cell|if
    v<rsub|6>\<gtr\>u<rsub|7>>>>>>
  </equation>

  because counting on lattice diagonals starting at the origin we sum
  <math|1+2+\<ldots\>+<around*|(|<math-up|min><around*|(|v<rsub|6>,u<rsub|7>|)>-1|)>>
  plus a partial diagonal if the polygon is not a triangle.

  Now observe that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|a<rsub|3>>>|<row|<cell|b<rsub|1>>|<cell|b<rsub|3>>>>>>>|<cell|=>|<cell|a<rsub|1>*<around*|(|b<rsub|1>+b<rsub|2>|)><rsub|>-b<rsub|1>*<around*|(|a<rsub|1>+a<rsub|2>|)>=a<rsub|1>*b<rsub|2>-b<rsub|1>*a<rsub|2>=<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|a<rsub|2>>>|<row|<cell|b<rsub|1>>|<cell|b<rsub|2>>>>>>=1>>|<row|<cell|<det|<tformat|<table|<row|<cell|a<rsub|3>>|<cell|a<rsub|2>>>|<row|<cell|b<rsub|3>>|<cell|b<rsub|2>>>>>>>|<cell|=>|<cell|*<around*|(|a<rsub|1>+a<rsub|2>|)><rsub|>*b<rsub|2>-*<around*|(|b<rsub|1>+b<rsub|2>|)>*a<rsub|2>=a<rsub|1>*b<rsub|2>-b<rsub|1>*a<rsub|2>=<det|<tformat|<table|<row|<cell|a<rsub|1>>|<cell|a<rsub|2>>>|<row|<cell|b<rsub|1>>|<cell|b<rsub|2>>>>>>=1>>>>
  </eqnarray*>

  so that <math|m<rsub|1>> and <math|m<rsub|3>> are also Farey neighbors and
  likewise for <math|m<rsub|3>> and <math|m<rsub|2>>.

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
    S<rsub|R>=S<rsub|T>+S<rsub|R<rprime|'>>+S<rsub|R<rprime|''>>
  </equation>

  or:

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>|<cell|=>|<cell|S<rsub|T><eq-number>>>|<row|<cell|>|<cell|+>|<cell|S<around*|(|u<rsub|4>,h-v<rsub|6>,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+v<rsub|6>|)>>>|<row|<cell|>|<cell|+>|<cell|S<around*|(|w-u<rsub|7>,v<rsub|5>,a<rsub|3>,b<rsub|3>,c<rsub|1>+c<rsub|2>+u<rsub|7>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>>>>>
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

  As a result there is no benefit in region processing the first
  <math|O<around*|(|n<rsup|1/3>|)>> lattice columns so we resort to the
  simple method:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|min>>|<cell|=>|<cell|<around*|\<lfloor\>|C*<sqrt|2*n|3>|\<rfloor\>><eq-number>>>|<row|<cell|x<rsub|max>>|<cell|=>|<cell|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>>|<row|<cell|y<rsub|min>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|max>|)>|\<rfloor\>>>>|<row|<cell|S<rsub|A>>|<cell|=>|<cell|S<around*|(|0,x<rsub|min>|)>>>>>
  </eqnarray*>

  where <math|C\<geqslant\>1> is a constant to be chosen later.

  Next we need to account for the all the points on or below the first line
  which is:

  <\equation*>
    S<rsub|B>=<around*|(|x<rsub|max>-x<rsub|min>+1|)>*y<rsub|min>+\<Delta\><around*|(|x<rsub|max>-x<rsub|min>|)>
  </equation*>

  Because all slopes in this section of the algorithm are whole integers, we
  have:

  <\eqnarray*>
    <tformat|<table|<row|<cell|a<rsub|i>>|<cell|=>|<cell|-m<rsub|i>>>|<row|<cell|b<rsub|i>>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  Assume that we have point <math|P<rsub|2>\<nocomma\>> and value
  <math|a<rsub|2>> from the previous iteration. \ For the first iteration we
  have:

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
    x<rsub|3>=<sqrt|<frac|n|a<rsub|1>>>
  </equation>

  (Note that <math|x<rsub|3>> is real-valued.)

  Similar to processing a region (but now in <math|x y> coordinates), we now
  need two lattice points <math|P<rsub|4> <around*|(|x<rsub|4,>y<rsub|4>|)>>
  and <math|P<rsub|5> <around*|(|x<rsub|5>.y<rsub|5>|)>> such that:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsub|4>>|<cell|\<gtr\>>|<cell|x<rsub|min>>>|<row|<cell|x<rsub|5>>|<cell|=>|<cell|x<rsub|4>+1>>|<row|<cell|-d
    y/d x<around*|(|x<rsub|4>|)>>|<cell|\<geqslant\>>|<cell|a<rsub|1>>>|<row|<cell|-d
    y /d x<around*|(|x<rsub|5>|)>>|<cell|\<less\>>|<cell|a<rsub|1>>>|<row|<cell|y<rsub|4>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|4>|)>|\<rfloor\>>>>|<row|<cell|y<rsub|5>>|<cell|=>|<cell|<around*|\<lfloor\>|Y<around*|(|x<rsub|5>|)>|\<rfloor\>>>>>>
  </eqnarray*>

  To meet these conditions we can set <math|x<rsub|4>=<around*|\<lfloor\>|x<rsub|3>|\<rfloor\>>>
  unless <math|x<rsub|4>\<leqslant\>x<rsub|min>> in which case we can
  manually count the lattice columns between <math|x<rsub|min>> and
  <math|x<rsub|2>> and cease iterating. \ If so, the remaining columns can be
  computed as:

  <\equation*>
    S<rsub|C>=<big|sum><rsup|x<rsub|2-1>><rsub|x=x<rsub|min>><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|(|a<rsub|2>*<around*|(|x<rsub|2>-x|)>+y<rsub|2>|)>
  </equation*>

  which is the number of lattice points below the hyperbola and above line
  <math|L<rsub|2>> over the interval <math|<around*|[|x<rsub|min>,x<rsub|2>|)>>.

  Now take lines <math|L<rsub|4>> and <math|L<rsub|5>> with slopes
  <math|-a<rsub|1>> and passing through <math|P<rsub|4>> and <math|P<rsub|5>>
  and then find the point <math|P<rsub|6>> where <math|L<rsub|4>> intersects
  <math|x=x<rsub|min>> and the point <math|P<rsub|7>> where <math|L<rsub|5>>
  intersects <math|x=x<rsub|min>>.

  <with|gr-mode|<tuple|edit|arc>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|5>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-auto-crop|true|<graphics||<line|<point|-3.0|-1.0>|<point|-4.0|1.0>>|<point|-4|1>|<point|-3|-1>|<line|<point|-6.0|2.0>|<point|-1.0|-3.0>>|<line|<point|-4.5|1.5>|<point|-6.0|4.5>>|<point|-4.5|1.5>|<spline|<point|-5.5|4.33826814463599>|<point|-4.5|2.0>|<point|-2.5|-1.0>|<point|-1.0|-2.73817405755366>>|<math-at|P<rsub|0>|<point|-3.5|-1.25312>>|<math-at|L<rsub|2>|<point|-2.76973250390689|-2.0>>|<point|-1|-3>|<math-at|P<rsub|2>|<point|-1.5|-3.22179084540838>>|<with|color|dark
  grey|<line|<point|-6|4.5>|<point|-6.0|-0.5>>>|<math-at|x<rsub|min>|<point|-6.23638|-1>>|<math-at|R|<point|-2.30258|-0.5>>|<math-at|H<around*|(|x,y|)>=n|<point|-4.5|3>>|<point|-4.24680283911887|1.5>|<math-at|P<rsub|5>|<point|-4.5|0.682911411193787>>|<math-at|P<rsub|4>|<point|-5.09461430341988|1.5>>|<math-at|P<rsub|3>|<point|-4.0|1.5>>|<point|-6|4.5>|<point|-6|2>|<math-at|P<rsub|6>|<point|-6.5|4.0>>|<math-at|P<rsub|7>|<point|-6.5|2>>>>

  \;

  Next add up the lattice points in the polygon
  <math|P<rsub|0>,P<rsub|7>,P<rsub|6>,P<rsub|4>,P<rsub|5>> but above
  <math|L<rsub|2>>:

  <\equation>
    S<rsub|W>=\<Delta\><around*|(|y<rsub|6>-y<rsub|7>|)>-\<Delta\><around*|(|y<rsub|6>-y<rsub|7>-<around*|(|x<rsub|4>-x<rsub|min>|)>|)>+\<Delta\><around*|(|x<rsub|0>-x<rsub|5>|)>
  </equation>

  Then choosing <math|P<rsub|1>=P<rsub|5>> (together with <math|P<rsub|0>>
  and <math|P<rsub|2>>) and calculating the necessary quantities:

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<rsub|1>>|<cell|=>|<cell|a<rsub|1*>x<rsub|5>+y<rsub|5>>>|<row|<cell|c<rsub|2>>|<cell|=>|<cell|a<rsub|2>*x<rsub|2>+y<rsub|2>>>|<row|<cell|w>|<cell|=>|<cell|a<rsub|1>*x<rsub|2>+y<rsub|2>-c<rsub|1>>>|<row|<cell|h>|<cell|=>|<cell|a<rsub|2>*x<rsub|5>+y<rsub|5>-c<rsub|2>>>>>
  </eqnarray*>

  we can now count lattice points using region processing:\ 

  <\equation>
    S<rsub|R>=S<around*|(|w,h,a<rsub|1>,b<rsub|1>,c<rsub|1>,a<rsub|2>,b<rsub|2>,c<rsub|2>|)>
  </equation>

  so the total sum for this iteration is:

  <\equation*>
    S<around*|(|a<rsub|1>|)>=S<rsub|W>+S<rsub|R>
  </equation*>

  Then we may advance to the next region by setting:

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rprime|'><rsub|2>>|<cell|=>|<cell|x<rsub|4>>>|<row|<cell|y<rprime|'><rsub|2>>|<cell|=>|<cell|y<rsub|4>>>|<row|<cell|a<rprime|'><rsub|2>>|<cell|=>|<cell|a<rsub|1>>>>>
  </eqnarray*>

  Finally, the total number of lattice points under the hyperbola from
  <math|1> to <math|x<rsub|max>> is:

  <\equation*>
    S=S<rsub|A>+S<rsub|B>+S<rsub|C>+<big|sum><rsup|a<rsub|max>><rsub|a=2>S<around*|(|a|)>
  </equation*>

  and therefore the final computation of the divisor summatory function is
  given by:

  <\equation*>
    T<around*|(|n|)>=2*S-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation*>

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

  \;

  <section|Time and Space Complexity>

  Now we present a heuristic argument for the runtime behavior of the
  algorithm.

  First we realize that because <math|x<rsub|min>=O<around*|(|n<rsup|1/3>|)>>
  and we process the values of <math|1\<leqslant\>x\<less\>x<rsub|min>>
  manually, the algorithm is at best <math|O<around*|(|n<rsup|1/3>|)>>. \ In
  this section we desire to show that the rest of the computation is at worst
  <math|O<around*|(|n<rsup|1/3>|)>> so that this lower bound holds for the
  entire computation.

  In order to determine the number of regions encountered in the course of
  processing a region with width <math|w> and <math|h>, we need to analyze
  the recursion depth. \ We will define the size of the region to be the
  number of operations needed to count lattice points using a direct
  summation in <math|u v> coordinates. \ Since we can choose either axis,
  this will be the minimum off the two values:

  <\equation>
    A=<math-up|min><around*|(|w,h|)>
  </equation>

  Similarly, the size of sub-region <math|R<rprime|'>> is then:

  <\equation>
    B=<math-up|min><around*|(|u<rsub|4>,h-v<rsub|6>|)>
  </equation>

  Then the reduction factor is the ratio:

  <\equation*>
    F=<frac|A|B>
  </equation*>

  If we recurse into <math|R<rprime|'>>, we will reduce <math|B> by a factor
  of <math|F> again, and so forth, until the size is less than one and so:

  <\equation*>
    A=F<rsup|E>
  </equation*>

  and thus the recursion depth is:

  <\equation*>
    E=<frac|<math-up|log><rsub|2>A|<math-up|log><rsub|2>F>
  </equation*>

  and the number of regions is <math|1+2+4+\<ldots\>.+2<rsup|E>=2<rsup|E+1>-1*=O<around*|(|2<rsup|E>|)>>
  but:

  <\equation*>
    2<rsup|E>=A<rsup|1/<math-up|log><rsub|2>F>
  </equation*>

  so the number of regions is:

  <\equation>
    N<rsub|><around*|(|A|)>=O<around*|(|A<rsup|1/<math-up|log><rsub|2>F>|)>
  </equation>

  For example, if <math|F=2>, then <math|N<rsub|><around*|(|A|)>=O<around*|(|A|)>>
  and if <math|F=4>, then <math|N<rsub|><around*|(|A|)>=O<around*|(|A<rsup|1/2>|)>>.

  Now if <math|w\<approx\>h> and we can approximate the hyperbolic segment by
  a circular arc, then:

  <\eqnarray*>
    <tformat|<table|<row|<cell|A>|<cell|\<approx\>>|<cell|w>>|<row|<cell|u<rsub|4>>|<cell|\<approx\>>|<cell|w*<around*|(|1-<frac|1|<sqrt|2>>|)>>>|<row|<cell|h-v<rsub|6>>|<cell|\<approx\>>|<cell|w*<around*|(|<sqrt|2>-1|)>>>|<row|<cell|B>|<cell|\<approx\>>|<cell|w*<around*|(|1-<frac|1|<sqrt|2>>|)>>>|<row|<cell|F<rsub|est>>|<cell|\<approx\>>|<cell|2+<sqrt|2><eq-number>>>>>
  </eqnarray*>

  or approximately:

  <\equation>
    N<rsub|R>=O<around*|(|w<rsup|1/log<rsub|2><around*|(|2+<sqrt|2>|)>>|)>
  </equation>

  regions total while processing region <math|R>.

  Now we need to count and size all the top-level regions. \ We process one
  top level region for each integral slope <math|-a> from <math|-1 >to the
  slope at <math|x<rsub|min>>. \ The value for <math|a> at each value of
  <math|x> is given by:

  <\equation>
    a<rsub|>=-<frac|d |d x>Y<around*|(|x<rsub|>|)>=<frac|n|x<rsup|2>>
  </equation>

  and:

  <\equation>
    X<around*|(|a|)>=<sqrt|<frac|n|a>>
  </equation>

  \ Choosing <math|C=1> so that <math|x<rsub|min>=<sqrt|2*n|3>>, then the
  highest value of <math|a> processed is:

  <\equation>
    a<rsub|max>=<frac|n|x<rsub|min><rsup|2>>=<frac|n<rsup|1/3>|2<rsup|2/3>>
  </equation>

  so there are <math|O<around*|(|n<rsup|1/3>|)>> top level regions.

  How big is each top level region? The change in <math|x> per unit change in
  <math|a> is <math|d x/d a> and so:

  <\equation>
    \<Delta\> x=-<frac|d|d a>X<around*|(|a|)>=<frac|n<rsup|1/2>|2*a<rsup|3/2>>
  </equation>

  Now we integrate the number of sub-regions processed for each top level
  region:

  <\equation>
    N<rsub|total>=<big|int><rsup|a<rsub|max>><rsub|1>N<around*|(|\<Delta\>
    x|)> d a=O<around*|(|<big|int><rsub|1><rsup|a<rsub|max>><around*|(|<frac|n<rsup|1/2>|2*a<rsup|3/2>>|)><rsup|1/<math-up|log><rsub|2>F>d
    a|)>
  </equation>

  \;

  We can classify three cases depending on the value of:

  <\equation*>
    G=1/log<rsub|2>F
  </equation*>

  because the outcome of the integration depends on the final exponent of
  <math|a>:

  <\equation*>
    N<rsub|total>=<choice|<tformat|<table|<row|<cell|O<around*|(|n<rsup|1/3>|)>>|<cell|if
    G \<less\>2/3>>|<row|<cell|O<around*|(|n<rsup|1/3>*log n|)>>|<cell|if
    G=2/3>>|<row|<cell|O<around*|(|n<rsup|G/2>|)>>|<cell|if G \<gtr\>
    2/3>>>>>
  </equation*>

  (Note that we cannot get below <math|O<around*|(|n<rsup|1/3>|)>> even if
  <math|G=0> because we have at least <math|a<rsub|min>=O<around*|(|n<rsup|1/3>|)>>
  top level regions.)

  So if we can show that the value of <math|G\<leqslant\>2/3>, then the
  algorithm overall will have a time complexity of
  <math|O<around*|(|n<rsup|1/3+\<epsilon\>>|)>>.

  Since <math|G\<leqslant\>2/3> when:

  <\equation*>
    F\<geqslant\>2<rsup|3/2>
  </equation*>

  and since we estimated earlier that:

  <\equation*>
    F<rsub|est>=2+<sqrt|2>\<gtr\>2<rsup|3/2>
  </equation*>

  the evidence for our time complexity conjecture is complete.

  The space complexity is simply our recursion depth which can be at most
  <math|O<around*|(|log n|)>>.

  \;
</body>

<\references>
  <\collection>
    <associate|abc|<tuple|6|?>>
    <associate|auto-1|<tuple|1|1>>
    <associate|auto-2|<tuple|2|2>>
    <associate|auto-3|<tuple|3|6>>
    <associate|auto-4|<tuple|4|8>>
    <associate|auto-5|<tuple|5|8>>
    <associate|auto-6|<tuple|6|?>>
  </collection>
</references>

<\auxiliary>
  <\collection>
    <\associate|toc>
      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Introduction>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-1><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Preliminaries>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-2><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Region
      Processing> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-3><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Top
      Level Region Processing> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-4><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Algorithm>
      <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-5><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Time
      and Space Complexity> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-6><vspace|0.5fn>
    </associate>
  </collection>
</auxiliary>