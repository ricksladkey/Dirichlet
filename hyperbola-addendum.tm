<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  Showing that by using <math|T<rsub|2><around*|(|n|)>>,
  <math|T<rsub|3><around*|(|n|)>> starts its iterations with
  <math|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>>.

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|3*<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|(|2*S<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+1,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>|)>+<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>><rsup|3>>>|<row|<cell|>|<cell|=>|<cell|3*<around*|(|2*T<rsub|2><around*|(|n|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>-n|)>+3*<big|sum><rsub|z=2><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|(|2*S<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+1,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>|)>+<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>><rsup|3>>>>>
  </eqnarray*>

  <\equation*>
    T<rsub|3><around*|(|n|)>=<big|sum><rsup|n><rsub|z=1>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>=<big|sum><rsub|z=1><rsup|n><big|sum><rsup|n/z><rsub|x=1>\<tau\><rsub|2><around*|(|x|)>=<big|sum><rsup|n><rsub|z=1><big|sum><rsup|<around*|\<lfloor\>|n/z|\<rfloor\>>><rsub|y=1><big|sum><rsup|<around*|\<lfloor\>|n/<around*|(|y*z|)>|\<rfloor\>>><rsub|x=1>1=<big|sum><rsub|x,y,z:x*y*z\<leq\>n>1
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|n>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>+<big|sum><rsub|z=1><rsup|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1>z*<around*|(|T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>-T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z+1>|\<rfloor\>>|)>|)>>>|<row|<cell|>|<cell|>|<cell|>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>+<big|sum><rsub|z=1><rsup|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>-<around*|(|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1|)>*T<rsub|2><around*|(|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|n>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>=<big|sum><rsub|z=1><rsup|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>+<big|sum><rsub|z=<around*|\<lceil\>|<sqrt|n>|\<rceil\>>><rsup|n>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<around*|(|n|)>-T<around*|(|m|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsup|m><rsub|x=1><around*|\<lfloor\>|<frac|m|x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|x,y:m\<less\>x*y\<leq\>n>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|n><rsub|x=m+1>\<tau\><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|2*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>|)>-<around*|(|2*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|m|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>><rsup|2>|)>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|(|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsup|<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|m|x>|\<rfloor\>>|)>-<around*|(|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>-<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>><rsup|2>|)>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|(|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>><rsub|x=1><around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<frac|m|x>|\<rfloor\>>|)>+<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>|)>-<around*|(|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>-<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>><rsup|2>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|x=1>\<tau\><rsub|2><around*|(|x|)>=<big|sum><rsup|n><rsub|x=1>T<rsub|1><around*|(|<frac|n|x>|)>>>|<row|<cell|T<rsub|1><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|x=1>\<mu\><around*|(|x|)>*T<rsub|2><around*|(|<frac|n|x>|)>>>|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|x=1><big|sum><rsub|d<around*|\||x|\<nobracket\>>>\<tau\><rsub|2><around*|(|d|)>>>|<row|<cell|\<tau\><rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><rsub|2><around*|(|d|)>>>|<row|<cell|\<tau\><rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d>|)>>>|<row|<cell|T<rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|d=1>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|\<approx\>>|<cell|2*<big|sum><rsup|n><rsub|c=0><big|sum><rsup|x*<around*|(|x-c|)>\<leq\>n><rsub|x=c+1><around*|\<lfloor\>|<frac|n|x*<around*|(|x-c|)>>|\<rfloor\>>>>|<row|<cell|>|<cell|\<approx\>>|<cell|6*<big|sum><rsup|c<rsup|2>\<leq\>n><rsub|c=0><big|sum><rsup|x<rsup|2>*<around*|(|x-c|)>\<leq\>n><rsub|x=c+1><around*|\<lfloor\>|<frac|n|x*<around*|(|x-c|)>>|\<rfloor\>>>>|<row|<cell|>|<cell|\<approx\>>|<cell|6*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|c=0><big|sum><rsup|<around*|\<lfloor\>|n/<around*|(|c+<sqrt|n|3>|)><rsup|2>|\<rfloor\>>><rsub|x=c+1><around*|\<lfloor\>|<frac|n|x*<around*|(|x-c|)>>|\<rfloor\>>>>>>
  </eqnarray*>

  <with|gr-mode|<tuple|edit|line>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|empty>|gr-grid-old|<tuple|cartesian|<point|0|0>|5>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|empty>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|5>|<graphics||<line|<point|0|0>|<point|0.5|-0.5>|<point|0.0|-1.0>|<point|-0.5|-0.5>|<point|0.0|0.0>>|<line|<point|0|-1>|<point|0.0|-1.5>|<point|0.5|-1.0>|<point|0.5|-0.5>>|<line|<point|0|-1.5>|<point|-0.5|-1.0>|<point|-0.5|-0.5>>|<line|<point|-0.5|-0.5>|<point|-0.5|0.5>|<point|0.0|1.0>|<point|0.0|0.0>>|<line|<point|-0.5|0.5>|<point|-1.0|1.0>|<point|-0.5|1.5>|<point|0.0|1.0>|<point|0.0|3.0>|<point|-0.5|3.5>|<point|-0.5|1.5>>|<line|<point|-0.5|3.5>|<point|0.0|4.0>|<point|0.5|3.5>|<point|0.0|3.0>>|<line|<point|0.5|3.5>|<point|0.5|1.5>|<point|0.0|1.0>|<point|0.5|0.5>|<point|1.0|1.0>|<point|0.5|1.5>>|<line|<point|0.5|0.5>|<point|0.5|-0.5>>|<line|<point|1|1>|<point|1.0|0.0>|<point|2.0|-1.0>|<point|1.5|-1.5>|<point|0.5|-0.5>|<point|1.0|0.0>>|<line|<point|0|-1.5>|<point|1.0|-2.5>|<point|1.5|-2.0>|<point|0.5|-1.0>>|<line|<point|1.5|-1.5>|<point|1.5|-2.0>|<point|3.5|-4.0>|<point|3.5|-4.5>|<point|4.0|-4.0>|<point|4.0|-3.5>|<point|3.5|-4.0>>|<line|<point|4|-3.5>|<point|2.0|-1.5>|<point|2.0|-1.0>>|<line|<point|2|-1.5>|<point|1.5|-2.0>>|<line|<point|1.5|-2>|<point|1.5|-2.5>|<point|3.5|-4.5>>|<line|<point|1.5|-2.5>|<point|1.0|-3.0>|<point|1.0|-2.5>>|<line|<point|0|-2>|<point|-1.0|-3.0>|<point|-1.0|-2.5>>|<line|<point|-0.5|-0.5>|<point|-1.0|0.0>|<point|-1.0|1.0>>|<line|<point|-1|0>|<point|-2.0|-1.0>|<point|-1.5|-1.5>|<point|-0.5|-0.5>>|<line|<point|-2|-1>|<point|-2.0|-1.5>|<point|-1.5|-2.0>|<point|-1.5|-1.5>>|<line|<point|-2|-1.5>|<point|-4.0|-3.5>|<point|-3.5|-4.0>|<point|-1.5|-2.0>>|<line|<point|-1.5|-2>|<point|-0.5|-1.0>>|<line|<point|0|-1.5>|<point|-1.0|-2.5>|<point|-1.5|-2.0>|<point|-1.5|-2.5>|<point|-1.0|-3.0>>|<line|<point|-1.5|-2.5>|<point|-3.5|-4.5>|<point|-4.0|-4.0>|<point|-4.0|-3.5>>|<line|<point|-3.5|-4>|<point|-3.5|-4.5>>|<line|<point|0|-2>|<point|1.0|-3.0>>|<line|<point|0|-1.5>|<point|0.0|-2.0>>>>

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rsup|2>*z>|<cell|=>|<cell|n>>|<row|<cell|x<rsup|2>*<around*|(|x-c|)>>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  From Mark Lewko (``seem to be able to compute''):

  <\eqnarray*>
    <tformat|<table|<row|<cell|D<rsub|k><around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<tau\><rsub|k><around*|(|n|)>=<big|sum><rsub|a*b\<leq\>x>\<tau\><rsub|k-1><around*|(|a|)>=<big|sum><rsub|a\<leq\>x><big|sum><rsub|b\<leq\>x/a>\<tau\><rsub|k-1><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x<rsup|1/k>><big|sum><rsub|b\<leq\>x/a>\<tau\><rsub|k-1><around*|(|b|)>+<big|sum><rsub|x<rsup|1/k>\<less\>b\<leq\>x><big|sum><rsub|a\<leq\>x/b>\<tau\><rsub|k-1><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x<rsup|1/k>><big|sum><rsub|b\<leq\>x/a>\<tau\><rsub|k-1><around*|(|b|)>+<big|sum><rsub|a\<leq\>x<rsup|1-1/k>>\<tau\><rsub|k-1><around*|(|a|)>*<big|sum><rsub|x<rsup|1/k>\<less\>b\<leq\>x/a>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<less\>x<rsup|1/k>>D<rsub|k-1><around*|(|<around*|\<lfloor\>|<frac|x|a>|\<rfloor\>>|)>+<big|sum><rsub|a\<leq\>x<rsup|1-1/k>>\<tau\><rsub|k-1><around*|(|a|)>*<around*|(|<around*|\<lfloor\>|<frac|x|a>|\<rfloor\>>-x<rsup|1/k>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>t<rsub|j><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|j><rsub|k=0><around*|(|-1|)><rsup|j-k>*<binom|j|k>*<big|sum><rsub|n\<leq\>x>\<tau\><rsub|k><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|k=0><rsup|j><around*|(|-1|)><rsup|j-k>*<binom|j|k>*D<rsub|k><around*|(|x|)>>>>>
  </eqnarray*>

  <\equation*>
    <big|sum><rsub|x\<leq\>n>\<mu\><around*|(|n|)><rsup|2>=<big|sum><rsub|x\<leq\>n><big|sum><rsub|l<around*|\||x|\<nobracket\>>>\<mu\><around*|(|l<rsup|1/2>|)>=<big|sum><rsub|l<rsup|2*>*m\<leq\>n>\<mu\><around*|(|l|)>=<big|sum><rsub|l=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>\<mu\><around*|(|l|)><around*|\<lfloor\>|<frac|n|l<rsup|2>>|\<rfloor\>>
  </equation*>

  From Nathan McKenzie:

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|k><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|j=a+1>T<rsub|k-1><around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|j=1><rsup|a>\<tau\><rsub|k-1><around*|(|j|)>*T<rsub|1><around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|j=1><rsup|a><big|sum><rsub|s=a/j+1><rsup|n/j><big|sum><rsub|m=1><rsup|k-2>\<tau\><rsub|m><around*|(|j|)>*T<rsub|k-m-1><around*|(|<frac|n|j*s>|)>>>|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|j=a+1>T<rsub|2><around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|j=1><rsup|a>\<tau\><rsub|2><around*|(|j|)>*T<rsub|1><around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|j=1><rsup|a><big|sum><rsub|s=a/j+1><rsup|n/j>T<rsub|1><around*|(|<frac|n|j*s>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|D<rsub|k,s><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n<rsup|1/k>><rsub|m=s><big|sum><rsub|j=0><rsup|k-1><binom|k|j>*D<rsub|j,m+1><around*|(|<around*|\<lfloor\>|<frac|n|m<rsup|k-j>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\equation*>
    n=<big|prod><rsup|k><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>,d<rsub|i><around*|(|n|)>=<big|prod><rsup|k><rsub|i=1><binom|a<rsub|i>+i-1|a<rsub|i>>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|D<rsub|i><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|j=1>D<rsub|i-1><around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|a><rsub|j=1>D<rsub|i-1><around*|(|<frac|n|j>|)>+<big|sum><rsub|j=a+1><rsup|n>D<rsub|i-1><around*|(|<frac|n|j>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|a><rsub|j=1>D<rsub|i-1><around*|(|<frac|n|j>|)>>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a><big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=1>D<rsub|i-2><around*|(|<frac|n/j|k>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a><big|sum><rsup|<around*|\<lfloor\>|a/j|\<rfloor\>>><rsub|k=1>D<rsub|i-2><around*|(|<frac|n/j|k>|)>+<big|sum><rsub|j=1><rsup|a><big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>D<rsub|i-2><around*|(|<frac|n/j|k*>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*D<rsub|i-2><around*|(|<frac|n|j>|)>+<big|sum><rsub|j=1><rsup|a>d<rsub|1><around*|(|j|)>*<big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>D<rsub|i-2><around*|(|<frac|n/j|k>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*D<rsub|i-2><around*|(|<frac|n|j>|)>>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*<big|sum><rsup|n/j><rsub|k=1>D<rsub|i-3><around*|(|<frac|n/j|k>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*<big|sum><rsup|a/j><rsub|k=1>D<rsub|i-3><around*|(|<frac|n/j|k>|)>+<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*<big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>D<rsub|i-3><around*|(|<frac|n/j|k>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|a>d<rsub|3><around*|(|j|)>*D<rsub|i-3><around*|(|<frac|n|j>|)>+<big|sum><rsub|j=1><rsup|a>d<rsub|2><around*|(|j|)>*<big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>D<rsub|i-3><around*|(|<frac|n/j|k>|)>>>>>
  </eqnarray*>

  <\equation*>
    D<rsub|i><around*|(|n|)>=<big|sum><rsup|a><rsub|j=1>d<rsub|i-1><around*|(|j|)>*<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>+<big|sum><rsup|a><rsub|j=1><big|sum><rsup|i-2><rsub|l=1>d<rsub|l><around*|(|j|)><big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>D<rsub|i-l-1><around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|n/j|\<rfloor\>>|k>|\<rfloor\>>|)>+<big|sum><rsup|n><rsub|j=a+1>D<rsub|i-1><around*|(|<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>|)>
  </equation*>

  <\equation*>
    T<rsub|i><around*|(|n|)>=<big|sum><rsup|a><rsub|j=1>\<tau\><rsub|i-1><around*|(|j|)>*<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>+<big|sum><rsup|i-2><rsub|\<ell\>=1><big|sum><rsup|a><rsub|j=1>\<tau\><rsub|\<ell\>><around*|(|j|)><big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>T<rsub|i-\<ell\>-1><around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|n/j|\<rfloor\>>|k>|\<rfloor\>>|)>+<big|sum><rsup|n><rsub|j=a+1>T<rsub|i-1><around*|(|<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>|)>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|m><rsub|k=b+1>T<rsub|i><around*|(|<around*|\<lfloor\>|<frac|m|k>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsup|m><rsub|k=1>T<rsub|i><around*|(|<around*|\<lfloor\>|<frac|m|k>|\<rfloor\>>|)>-<big|sum><rsup|b><rsub|k=1>T<rsub|i><around*|(|<around*|\<lfloor\>|<frac|m|k>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|i+1><around*|(|m|)>-<big|sum><rsup|b><rsub|k=1>T<rsub|i><around*|(|<around*|\<lfloor\>|<frac|m|k>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|m><rsub|k=b+1><big|sum><rsup|<around*|\<lfloor\>|m/k|\<rfloor\>>><rsub|l=1>T<rsub|i-1><around*|(|<around*|\<lfloor\>|<frac|m/k|l>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|n/j|\<rfloor\>>|k>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|n/j|\<rfloor\>>><rsub|k=<around*|\<lfloor\>|a/j|\<rfloor\>>+1><big|sum><rsup|<around*|\<lfloor\>|n/<around*|(|j*k|)>|\<rfloor\>>><rsub|\<ell\>=1>T<rsub|1><around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|n/<around*|(|j*k|)>|\<rfloor\>>|\<ell\>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\equation*>
    D<rsub|k,s><around*|(|n|)>=<big|sum><rsup|n<rsup|1/k>><rsub|m=s><big|sum><rsup|k-1><rsub|j=0><binom|k|j>*D<rsub|j,m+1><around*|(|<around*|\<lfloor\>|<frac|n|m<rsup|k-j>>|\<rfloor\>>|)>
  </equation*>

  Why a wheel is used with Linnik's identity.

  <\equation*>
    <big|sum><rsup|><rsub|k><frac|<around*|(|-1|)><rsup|k+1>|k>*t<rsub|k><around*|(|n|)>=<choice|<tformat|<table|<row|<cell|1/a>|<cell|if
    p<rsup|a>>>|<row|<cell|0>|<cell|otherwise>>>>>
  </equation*>

  therefore in

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<Pi\><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|k><frac|<around*|(|-1|)><rsup|k+1>|k>*T<rsub|k><around*|(|n|)>>>>>
  </eqnarray*>

  the inclusion of any <math|t<rsub|k><around*|(|n|)>> contributions can be
  omitted for <math|n> not a prime power as long as it is omitted for all
  <math|T<rsub|k><around*|(|n|)>>.

  Other stuff:

  <\equation*>
    <binom|n|k>=<frac|n!|k!*<around*|(|n-k|)>!>
  </equation*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|i\<leq\><around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>>\<mu\><around*|(|i|)>*<around*|\<lfloor\>|<frac|<sqrt|n|3>|i>|\<rfloor\>><rsup|3>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><rsub|i=1>i<rsup|3>*<big|sum><rsup|<frac|<around*|\<lfloor\>|<frac|<sqrt|n|3>|i>|\<rfloor\>>|>><rsub|j=<around*|\<lfloor\>|<frac|<sqrt|n|3>|i+1>|\<rfloor\>>+1>\<mu\><around*|(|j|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><rsub|i=1>i<rsup|3>*<around*|[|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|i>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|i+1>|\<rfloor\>>|)>|]>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|1*<around*|[|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|1>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|2>|\<rfloor\>>|)>|]>+>>|<row|<cell|>|<cell|>|<cell|-1*<around*|[|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|2>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|3>|\<rfloor\>>|)>|]>+>>|<row|<cell|>|<cell|>|<cell|1*<around*|[|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|4>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|5>|\<rfloor\>>|)>|]>+>>|<row|<cell|>|<cell|>|<cell|\<ldots\>
    <around*|(|mod 9|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|1>|\<rfloor\>>|)>-2*M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|2>|\<rfloor\>>|)>+M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|3>|\<rfloor\>>|)>+>>|<row|<cell|>|<cell|>|<cell|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|4>|\<rfloor\>>|)>-2*M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|5>|\<rfloor\>>|)>+M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|6>|\<rfloor\>>|)>+>>|<row|<cell|>|<cell|>|<cell|\<ldots\>
    <around*|(|mod 9|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><rsub|i=1>M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|i>|\<rfloor\>>|)>-3*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/3|\<rfloor\>>><rsub|j=1>M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|3*j-1>|\<rfloor\>>|)>
    <around*|(|mod 9|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|1-3*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/3|\<rfloor\>>><rsub|j=1>M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|3*j-1>|\<rfloor\>>|)>
    <around*|(|mod 9|)>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/3|\<rfloor\>>><rsub|j=1>M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|3*j-1>|\<rfloor\>>|)><rsup|>>|<cell|=>|<cell|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|2>|\<rfloor\>>|)>+M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|5>|\<rfloor\>>|)>+M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|8>|\<rfloor\>>|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|2>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|5>|\<rfloor\>>|)>|)>+2*<around*|(|M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|5>|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|<frac|<sqrt|n|3>|8>|\<rfloor\>>|)>|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|i=<around*|\<lfloor\>|<sqrt|n|3>/5|\<rfloor\>>+1><rsup|<around*|\<lfloor\>|<sqrt|n|3>/2|\<rfloor\>>>\<mu\><around*|(|i|)>+2*<big|sum><rsub|i=<around*|\<lfloor\>|<sqrt|n|3>/8|\<rfloor\>>+1><rsup|<around*|\<lfloor\>|<sqrt|n|3>/5|\<rfloor\>>>\<mu\><around*|(|i|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>/3|\<rfloor\>>-1>j*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|3*j-1|)>|\<rfloor\>>><rsub|i=<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|3*j+2|)>|\<rfloor\>>>\<mu\><around*|(|i|)><rsup|>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|j=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>/9|\<rfloor\>>-1><around*|(|*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|9*j-1|)>|\<rfloor\>>><rsub|i=<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|9*j+2|)>|\<rfloor\>>>\<mu\><around*|(|i|)>-<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|9*j+2|)>|\<rfloor\>>><rsub|i=<around*|\<lfloor\>|<sqrt|n|3>/<around*|(|9*j+5|)>|\<rfloor\>>>\<mu\><around*|(|i|)>|)><rsup|>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|3><around*|(|x|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsup|<around*|\<lfloor\>|x<rsup|1/3>|\<rfloor\>>><rsub|j=1>\<mu\><around*|(|j|)>*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|x<rsup|>|j<rsup|3>>|\<rfloor\>>|)>-1|)>/3>>>>
  </eqnarray*>

  Attempt at proof without Linnik's identity.

  For <math|a> a square-free product of <math|\<omega\><around*|(|a|)>>
  distinct primes

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|a*<around*|(|-<binom|\<omega\><around*|(|a|)>|\<omega\><around*|(|a|)>>+<binom|\<omega\><around*|(|a|)>|\<omega\><around*|(|a|)>-1>-<binom|\<omega\><around*|(|a|)>|\<omega\><around*|(|a|)>-2>+\<ldots\>\<pm\><binom|\<omega\><around*|(|a|)>|1>|)>>>|<row|<cell|>|<cell|=>|<cell|a*<around*|(|-<big|sum><rsup|\<omega\><around*|(|a|)>><rsub|j=0><around*|(|-1|)><rsup|j>*<binom|\<omega\><around*|(|a|)>|\<omega\><around*|(|a|)>-j>+<around*|(|-1|)><rsup|\<omega\><around*|(|a|)>>*<binom|\<omega\><around*|(|a|)>|0>|)>>>|<row|<cell|>|<cell|=>|<cell|a*<around*|(|0+<around*|(|-1|)><rsup|\<omega\><around*|(|a|)>>\<cdot\>1|)>>>|<row|<cell|>|<cell|=>|<cell|a*<around*|(|-1|)><rsup|\<omega\><around*|(|a|)>>>>|<row|<cell|>|<cell|=>|<cell|a*\<mu\><around*|(|a|)>>>>>
  </eqnarray*>

  for <math|a=p<rsup|b>> a prime power

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|a*<around*|(|-<binom|b|b>+<binom|b+1|b>-<binom|b+2|b>|)>>>>>
  </eqnarray*>

  Recurrence approach to coefficients <math|c<around*|(|a|)>>

  Seeing that <math|c<around*|(|1|)>=1>, we can then express
  <math|c<around*|(|a|)>> for <math|a\<gtr\>1> as a recurrence relation

  <\equation*>
    c<around*|(|a|)>=-<big|sum><rsub|d\<gtr\>1,d<around*|\||a|\<nobracket\>>>d*c<around*|(|<frac|a|d>|)>
  </equation*>

  and substituting <math|c<around*|(|a|)>=1\<cdot\>c<around*|(|a/1|)>> and
  rearranging yields

  <\equation*>
    <big|sum><rsup|><rsub|d<around*|\||a|\<nobracket\>>>d*c<around*|(|<frac|a|d>|)>=0
  </equation*>

  Noting that the left hand side is equal to unity if <math|a=1>, we obtain

  <\equation*>
    <big|sum><rsub|d<around*|\||a|\<nobracket\>>>d*c<around*|(|<frac|a|d>|)>=<choice|<tformat|<table|<row|<cell|1>|<cell|if
    a=1>>|<row|<cell|0>|<cell|otherwise>>>>>=\<epsilon\><around*|(|a|)>
  </equation*>

  where <math|\<epsilon\><around*|(|a|)>> is the multiplicative identity.
  \ If we select

  <\equation*>
    c<around*|(|n|)>=n*\<mu\><around*|(|n|)>
  </equation*>

  then substituting gives

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<around*|\||a|\<nobracket\>>>d*c<around*|(|<frac|a|d>|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||a|\<nobracket\>>>d*<around*|(|<frac|a|d>*\<mu\><around*|(|<frac|a|d>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|a*<big|sum><rsub|d<around*|\||a|\<nobracket\>>>\<mu\><around*|(|<frac|a|d>|)>>>|<row|<cell|>|<cell|=>|<cell|a<big|sum><rsub|d<around*|\||a|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|a*\<epsilon\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|\<epsilon\><around*|(|a|)>>>>>
  </eqnarray*>

  and thus our choice satisfies the recurrence relation.

  <\equation*>
    <big|sum><rsub|m\<leq\>n,\<omega\><around*|(|m|)>=2>1=#<around*|{|m:m\<leq\>n,m=p<rsup|a>*q<rsup|b>|}>
  </equation*>

  \;

  <\equation*>
    \;
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|p<rsub|1>*p<rsub|2>\<leq\>n>1>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>n,\<omega\><around*|(|a|)>=2,a
    squarefree>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|p<rsub|1>\<leq\>n>\<pi\><around*|(|<around*|\<lfloor\>|<frac|n|p<rsub|1>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  \;

  <\equation*>
    f<rsub|2,3><around*|(|n|)>=<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|2><around*|(|<frac|n|d<rsup|3>>|)>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|2,3><around*|(|1|)>>|<cell|=>|<cell|1>>|<row|<cell|f<rsub|2,3><around*|(|p|)>>|<cell|=>|<cell|2>>|<row|<cell|f<rsub|2,3><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|3,a\<gtr\>1>>|<row|<cell|f<rsub|2,3><around*|(|n|)>>|<cell|=>|<cell|2<rsup|\<omega\><around*|(|n|)>>,n
    squarefree>>|<row|<cell|f<rsub|2,3><around*|(|n|)>>|<cell|=>|<cell|3*2<rsup|c<rsub|1>*>*3<rsup|c<rsub|2>>,n
    not squarefree>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n>f<rsub|2,3><around*|(|m|)>>|<cell|=>|<cell|1+3*<around*|(|n-S<around*|(|n|)>|)>*C<rsub|1>+2*\<pi\><around*|(|n|)>+4*C<rsub|2>>>>>
  </eqnarray*>

  <\equation*>
    \<pi\><around*|(|n|)>\<equiv\><around*|(|<big|sum><rsub|m\<leq\>n>f<rsub|2,3><around*|(|m|)>-1-3*<around*|(|n-S<around*|(|n|)>|)>*C<rsub|1>|)>/2
    <around*|(|mod 2|)>
  </equation*>

  <\equation*>
    f<rsub|2,4><around*|(|n|)>=<big|sum><rsub|d<rsup|4><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|2><around*|(|<frac|n|d<rsup|4>>|)>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|2,4><around*|(|1|)>>|<cell|=>|<cell|1>>|<row|<cell|f<rsub|2,4><around*|(|p|)>>|<cell|=>|<cell|2>>|<row|<cell|f<rsub|2,4><around*|(|p<rsup|2>|)>>|<cell|=>|<cell|3>>|<row|<cell|f<rsub|2,4><around*|(|p<rsup|3>|)>>|<cell|=>|<cell|4>>|<row|<cell|f<rsub|2,4><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|4,a\<gtr\>3>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2><around*|(|n|)>=<big|sum><rsup|n><rsub|i=1>\<tau\><rsub|2><around*|(|i|)>>|<cell|=>|<cell|2*\<pi\><around*|(|3|)>+<big|sum><rsup|n><rsub|i=5>\<tau\><rsub|1><around*|(|i|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><rsub|0><around*|(|n|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|0>|<cell|otherwise>>>>>=\<varepsilon\><around*|(|n|)>>>|<row|<cell|\<tau\><rsub|1><around*|(|n|)>>|<cell|=>|<cell|1=1<around*|(|n|)>>>|<row|<cell|\<tau\><rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|k=1><around*|(|a<rsub|i>+1|)>=d<around*|(|n|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><rsub|k><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d>\<tau\><rsub|k-1><around*|(|<frac|n|d>|)>=<around*|(|1
    \<ast\> \<tau\><rsub|k-1>|)><around*|(|n|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|i=1>\<tau\><rsub|2><around*|(|i|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|n><rsub|i=1>\<tau\><rsub|1><around*|(|i|)>*T<rsub|1><around*|(|<around*|\<lfloor\>|<frac|n|i>|\<rfloor\>>|)>>>|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|n><rsub|i=1>\<tau\><rsub|2><around*|(|i|)>*T<rsub|1><around*|(|<around*|\<lfloor\>|<frac|n|i>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  Try omitting factors all numbers divisible by two.

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|x:x\<leq\>n,2\<nmid\>x>\<tau\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|2*<big|sum><rsub|x:x\<leq\><sqrt|n>,2\<nmid\>x><around*|\<lceil\>|<frac|<around*|\<lfloor\>|n/x|\<rfloor\>>|2>|\<rceil\>>-<around*|(|<around*|\<lceil\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|2>|\<rceil\>>|)><rsup|2>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|\<lceil\>|<frac|<around*|\<lfloor\>|n/x|\<rfloor\>>|2>|\<rceil\>>>|<cell|=>|<cell|<frac|<around*|\<lfloor\>|n/x|\<rfloor\>>+1|2>>>|<row|<cell|>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|n+x|2*x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|n|2*x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2>>|<row|<cell|>|<cell|=>|<cell|<around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2|)>/2>>>>
  </eqnarray*>

  <\equation*>
    S<rsub|odd><around*|(|n;a,b|)>=<around*|(|<big|sum><rsub|a\<leq\>x\<leq\>b,x
    odd><around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2|)>|)>/2
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsup|><rsub|2,odd><around*|(|n|)>=<big|sum><rsub|x:x\<leq\>n,x
    odd>\<tau\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|2*S<rsub|odd><around*|(|n;1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>>>
  </eqnarray*>

  Note that:

  <\itemize-dot>
    <item>The value of <math|T<rsub|2,even><around*|(|n|)>> can be computed
    from <math|T<rsub|2><around*|(|n/2|)>> and
    <math|T<rsub|2><around*|(|n/4|)>> and so all terms smaller by a power of
    <math|2> can be calculated together in the same total time as for
    <math|T<rsub|2><around*|(|n|)>> alone.

    <item>The included points form a double-size latticed, offset by one,
    that would allow the successive approximation algorithm to be used for
    <math|T<rsub|2,odd><around*|(|n|)>> with a simple modification.
  </itemize-dot>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsup|><rsub|2><around*|(|n|)>>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+2*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>-T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+2*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>+3*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>-2*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|8>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+2*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>+3*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>+4*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|8>|\<rfloor\>>|)>-3*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|16>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|i=0><around*|(|i+1|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|i>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2><around*|(|n|)>-T<rsub|2><around*|(|<frac|n|2>|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|i=0><around*|(|i+1|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|i>>|\<rfloor\>>|)>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|i=1>i*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|i>>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|i=0>T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|i>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|m><rsub|k=0>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|m>>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsup|m><rsub|k=0><big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n/2<rsup|m>|\<rfloor\>>><rsub|i=0><around*|(|i+1|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n/2<rsup|m>|2<rsup|i>>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|m><rsub|k=0><big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n/2<rsup|m>|\<rfloor\>>><rsub|i=0><around*|(|i+1|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n|2<rsup|m-i>>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+2*T<rsub|2,odd><around*|(|<frac|n|2>|)>+3*T<rsub|2,odd><around*|(|<frac|n|4>|)>+4*T<rsub|2,odd><around*|(|<frac|n|8>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2,odd><around*|(|<frac|n|2>|)>+2*T<rsub|2,odd><around*|(|<frac|n|4>|)>+3*T<rsub|2,odd><around*|(|<frac|n|8>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2,odd><around*|(|<frac|n|4>|)>+2*T<rsub|2,odd><around*|(|<frac|n|8>|)>+3*T<rsub|2,odd><around*|(|<frac|n|16>|)>+\<ldots\>>>>>
  </eqnarray*>

  Reorganize <math|T<rsub|3><around*|(|n|)>> by increasing factor of <math|2>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|T<rsub|2><around*|(|<frac|n|1>|)>+T<rsub|2><around*|(|<frac|n|2>|)>+T<rsub|2><around*|(|<frac|n|3>|)>+\<ldots\>+T<rsub|2><around*|(|<frac|n|n>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|<frac|n|1>|)>+T<rsub|2><around*|(|<frac|n|2>|)>+T<rsub|2><around*|(|<frac|n|4>|)>+T<rsub|2><around*|(|<frac|n|8>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2><around*|(|<frac|n|3>|)>+T<rsub|2><around*|(|<frac|n|6>|)>+T<rsub|2><around*|(|<frac|n|12>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2><around*|(|<frac|n|5>|)>+T<rsub|2><around*|(|<frac|n|10>|)>+T<rsub|2><around*|(|<frac|n|20>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|T<rsub|2,odd><around*|(|<frac|n|1>|)>+T<rsub|2,odd><around*|(|<frac|n|3>|)>+T<rsub|2,odd><around*|(|<frac|n|5>|)>+T<rsub|2,odd><around*|(|<frac|n|7>|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|\<ldots\>.>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2,odd><around*|(|<frac|n|3>|)>+T<rsub|2,odd><around*|(|<frac|n|9>|)>+T<rsub|2,odd><around*|(|<frac|n|15>|)>+T<rsub|2,odd><around*|(|<frac|n|21>|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|T<rsub|2,odd><around*|(|<frac|n/3|1>|)>+T<rsub|2,odd><around*|(|<frac|n/3|3>|)>+T<rsub|2,odd><around*|(|<frac|n/3|5>|)>+T<rsub|2,odd><around*|(|<frac|n/3|7>|)>+\<ldots\>>>>>
  </eqnarray*>

  Characterize <math|T<rsub|k,odd><around*|(|n|)>>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|0,odd><around*|(|n|)>>|<cell|=>|<cell|1>>|<row|<cell|T<rsub|1,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|i
    \<leq\>n><rsub|i odd<rsup|>>T<rsub|0,odd><around*|(|<frac|n|i>|)>=<around*|\<lfloor\>|<frac|n+1|2>|\<rfloor\>>>>|<row|<cell|T<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|i
    \<leq\>n><rsub|i odd>T<rsub|1,odd><around*|(|<frac|n|i>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|6*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><big|sum><rsub|z\<less\>x\<leq\><sqrt|n/z>,z
    odd><around*|(|<frac|<around*|\<lfloor\>|<frac|n/z|x>|\<rfloor\>>+1|2>-<frac|z+1|2>|)>>>|<row|<cell|>|<cell|->|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>-<frac|z+1|2>|)><rsup|2>>>|<row|<cell|>|<cell|+>|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|<frac|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+1|2>-<frac|z+1|2>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|>1>>|<row|<cell|>|<cell|=>|<cell|6*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|>S<rsub|odd><around*|(|<frac|n|z>,z+2,<sqrt|<frac|n|z>>|)>>>|<row|<cell|>|<cell|->|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><around*|(|<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>|)><rsup|2>>>|<row|<cell|>|<cell|<rsub|+>>|<cell|6*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><frac|z+1|2>*<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>>>|<row|<cell|>|<cell|->|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|<frac|z-1|2>|)><rsup|2>*>>|<row|<cell|>|<cell|->|<cell|6*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><frac|z+1|2>*<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>>>|<row|<cell|>|<cell|+>|<cell|6*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|<frac|z+1|2>|)><rsup|2>*>>|<row|<cell|>|<cell|+>|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><frac|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+1|2>>>|<row|<cell|>|<cell|->|<cell|3**<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><frac|z+1|2>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|>1>>|<row|<cell|>|<cell|=>|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|2*S<rsub|odd><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+2,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|(|<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>|)><rsup|2>+<frac|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+1|2>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|3*<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>><around*|(|2*S<around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>,z+1,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>|)>+<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>><rsup|3>>>>>
  </eqnarray*>

  Sum of <math|T<rsub|2><around*|(|n|)>> correction term modulo 2.

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|n;a,b|)>>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>i\<leq\>b><around*|\<lfloor\>|<frac|n|i>|\<rfloor\>>>>>>
  </eqnarray*>

  <\equation*>
    T<rsub|2><around*|(|n|)>=2*<big|sum><rsub|i=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><around*|\<lfloor\>|<frac|n|i>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2><around*|(|n|)>>|<cell|=>|<cell|2*<big|sum><rsub|i=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><around*|\<lfloor\>|<frac|n|i>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>>>|<row|<cell|>|<cell|=>|<cell|2*S<around*|(|n;1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<rsup|>=1><rsup|m>\<mu\><around*|(|d|)><around*|\<lfloor\>|<frac|m|d>|\<rfloor\>><rsup|2>>|<cell|=>|<cell|<big|sum><rsup|m><rsub|i=1>i<rsup|2>*<big|sum><rsup|<around*|\<lfloor\>|m/i|\<rfloor\>>><rsub|j=<around*|\<lfloor\>|m/<around*|(|i+1|)>|\<rfloor\>>+1>\<mu\><around*|(|j|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|m><rsub|i=1>i<rsup|2>*<around*|(|M<around*|(|<around*|\<lfloor\>|m/i|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|m/<around*|(|i+1|)>|\<rfloor\>>|)>|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|i
    odd><around*|(|M<around*|(|<around*|\<lfloor\>|m/i|\<rfloor\>>|)>-M<around*|(|<around*|\<lfloor\>|m/<around*|(|i+1|)>|\<rfloor\>>|)>|)>
    <around*|(|mod 4|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|M<around*|(|m|)>-M<around*|(|<frac|m|2>|)>+M<around*|(|<frac|m|3>|)>-M<around*|(|<frac|m|4>|)>+\<ldots\>
    <around*|(|mod 4|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|i=1><rsup|m><around*|(|-1|)><rsup|i+1>*M<around*|(|<frac|m|i>|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|-1
    <around*|(|mod 4|)>,m\<geq\>2>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|2><around*|(|n|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n>>\<mu\><around*|(|d|)>*T<rsub|2><around*|(|<frac|n|d<rsup|2>>|)>-1|)>/2>>|<row|<cell|>|<cell|=>|<cell|<around*|[|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n><rsup|>>\<mu\><around*|(|d|)>*<around*|(|2*S<around*|(|<frac|n|d<rsup|2>>;1,<sqrt|<frac|n|d<rsup|2>>>|)>-<around*|\<lfloor\>|<sqrt|n/d<rsup|2>>|\<rfloor\>><rsup|2>|)>-1|]>/2>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n>>\<mu\><around*|(|d|)>*S<around*|(|<frac|n|d<rsup|2>>;1,<sqrt|<frac|n|d<rsup|2>>>|)>-<around*|(|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n><rsup|>>\<mu\><around*|(|d|)><around*|\<lfloor\>|<frac|<sqrt|n>|d>|\<rfloor\>><rsup|2>+1|)>/2>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n><rsup|>>\<mu\><around*|(|d|)>*S<around*|(|<frac|n|d<rsup|2>>;1,<sqrt|<frac|n|d<rsup|2>>>|)>-<around*|(|-1+1|)>/2
    <around*|(|mod 2|)>,n\<geq\>4>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|d<rsup|>\<leq\><sqrt|n><rsup|>>\<mu\><around*|(|d|)>*S<around*|(|<frac|n|d<rsup|2>>;1,<sqrt|<frac|n|d<rsup|2>>>|)>
    <around*|(|mod 2|)>,n\<geq\>4>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsup|><rsub|2,odd><around*|(|n|)>=<big|sum><rsub|x:x\<leq\>n,x
    odd>\<tau\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2|)>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>|<row|<cell|>|<cell|=>|<cell|2*<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<frac|n|2x>|\<rfloor\>>|)>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>|<row|<cell|>|<cell|=>|<cell|2*<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-2*<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|\<lfloor\>|<frac|n|2x>|\<rfloor\>>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>|<row|<cell|>|<cell|=>|<cell|2*<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-2*<big|sum><rsub|x:x\<leq\><sqrt|n>,x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>>>|<row|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>>>|<row|<cell|<big|sum><rsub|x
    even><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|*T<rsub|2><around*|(|<frac|n|2>|)>>>|<row|<cell|T<rsub|2><around*|(|n|)>=<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<big|sum><rsub|x
    even><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>+*T<rsub|2><around*|(|<frac|n|2>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+2*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>-T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>>>|<row|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>>|<cell|=>|<cell|*T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>-T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>>>|<row|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2,odd><around*|(|n|)>+T<rsub|2,odd><around*|(|<frac|n|2>|)>+<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n/4|x>|\<rfloor\>>>>|<row|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=0>T<rsub|2,odd><around*|(|<frac|n|2<rsup|k>>|)>>>|<row|<cell|T<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=1>T<rsub|2,odd><around*|(|<frac|n|2<rsup|k>>|)>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|1,odd><around*|(|n|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|n+1|2>|\<rfloor\>>=<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>+<around*|\<lfloor\>|n|\<rfloor\>>
    mod 2>>|<row|<cell|T<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x\<leq\><sqrt|n>,x
    odd>T<rsub|1,odd><around*|(|<frac|n|x>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|(|<around*|\<lfloor\>|<frac|n|2*x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|2*x>|\<rfloor\>>+<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>+<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n/2><around*|\<lfloor\>|<frac|n/2|x>|\<rfloor\>>-<big|sum><rsub|x\<leq\>n/4><around*|\<lfloor\>|<frac|n/4|x>|\<rfloor\>>+<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|<frac|n|2>|)>-*T<rsub|2><around*|(|<frac|n|4>|)>+<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>>|<row|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-*T<rsub|2><around*|(|<frac|n|2>|)>>>|<row|<cell|T<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsub|x\<leq\>n/2,x
    odd><around*|\<lfloor\>|<frac|n/2|*x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-*T<rsub|2><around*|(|<frac|n|2>|)>-<around*|(|T<rsub|2><around*|(|<frac|n|2>|)>-T<rsub|2><around*|(|<frac|n|4>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-2*T<rsub|2><around*|(|<frac|n|2>|)>+T<rsub|2><around*|(|<frac|n|4>|)>>>|<row|<cell|T<rsub|2><around*|(|<frac|n|2>|)>-*T<rsub|2><around*|(|<frac|n|4>|)>+<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-2*T<rsub|2><around*|(|<frac|n|2>|)>+T<rsub|2><around*|(|<frac|n|4>|)>>>|<row|<cell|<big|sum><rsub|x\<leq\>n,x
    odd><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>> mod
    2>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-3*T<rsub|2><around*|(|<frac|n|2>|)>+2*T<rsub|2><around*|(|<frac|n|4>|)>>>|<row|<cell|<big|sum><rsub|x\<leq\>n,x
    mod 3\<neq\>0><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-T<rsub|2><around*|(|<frac|n|3>|)>>>|<row|<cell|T<rsub|2,3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n,x
    mod 3\<neq\>0><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<big|sum><rsub|x\<leq\>n/3,x
    mod 3\<neq\>0><around*|\<lfloor\>|<frac|n/3|*x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-T<rsub|2><around*|(|<frac|n|3>|)>-<around*|(|T<rsub|2><around*|(|<frac|n|3>|)>-T<rsub|2><around*|(|<frac|n|9>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-2*T<rsub|2><around*|(|<frac|n|3>|)>+T<rsub|2><around*|(|<frac|n|9>|)>>>>>
  </eqnarray*>

  \;

  <with|gr-mode|<tuple|edit|line>|gr-frame|<tuple|scale|1cm|<tuple|0.5gw|0.5gh>>|gr-geometry|<tuple|geometry|1par|0.6par>|gr-grid|<tuple|cartesian|<point|0|0>|1>|gr-grid-old|<tuple|cartesian|<point|0|0>|1>|gr-edit-grid-aspect|<tuple|<tuple|axes|none>|<tuple|1|none>|<tuple|10|none>>|gr-edit-grid|<tuple|cartesian|<point|0|0>|1>|gr-edit-grid-old|<tuple|cartesian|<point|0|0>|1>|gr-auto-crop|true|<graphics||<line|<point|0|4>|<point|0.0|0.0>|<point|4.0|0.0>>|<spline|<point|0.146187731040926|4.2>|<point|0.5|2.2>|<point|2.5|0.6>|<point|4.0|0.2>>|<line|<point|2.5|0>|<point|2.5|0.6>>>>

  For <math|m\<geq\><sqrt|n>>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|m><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>|<cell|=>|<cell|T<rsub|2><around*|(|n|)>-<big|sum><rsup|n><rsub|x=m+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|n/m|\<rfloor\>>><rsub|x=1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+2*<big|sum><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>><rsub|x=<around*|\<lfloor\>|n/m|\<rfloor\>>+1><around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>-<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>+m*<around*|\<lfloor\>|<frac|n|m>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|S<around*|(|n;1,<around*|\<lfloor\>|<frac|n|m>|\<rfloor\>>|)>+2*S<around*|(|n;<around*|\<lfloor\>|<frac|n|m>|\<rfloor\>>+1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>+<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>><rsup|2>+m*<around*|\<lfloor\>|<frac|n|m>|\<rfloor\>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|f<around*|(|<frac|n|3>|)>+f<around*|(|<frac|n|5>|)>+f<around*|(|<frac|n|7>|)>+\<ldots\>+f<around*|(|<frac|n|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>|)>>>|<row|<cell|>|<cell|+>|<cell|f<around*|(|<frac|n|3*5>|)>+f<around*|(|<frac|n|3*7>|)>+f<around*|(|<frac|n|3*9>|)>+\<ldots\>+f<around*|(|<frac|n|<sqrt|3*n>>|)>>>|<row|<cell|>|<cell|+>|<cell|f<around*|(|<frac|n|5*7>|)>+f<around*|(|<frac|n|5*9>|)>+f<around*|(|<frac|n|5*5>|)>+\<ldots\>+f<around*|(|<frac|n|<sqrt|5*n>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|f<around*|(|<frac|n|3>|)>+f<around*|(|<frac|n|5>|)>+f<around*|(|<frac|n|7>|)>+\<ldots\>+f<around*|(|<frac|n|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>|)>>>|<row|<cell|>|<cell|+>|<cell|f<around*|(|<frac|n|3*5>|)>+f<around*|(|<frac|n|3*7>|)>+f<around*|(|<frac|n|3*9>|)>+\<ldots\>+f<around*|(|<frac|n|<sqrt|3*n>>|)>>>|<row|<cell|>|<cell|+>|<cell|f<around*|(|<frac|n|5*7>|)>+f<around*|(|<frac|n|5*9>|)>+f<around*|(|<frac|n|5*5>|)>+\<ldots\>+f<around*|(|<frac|n|<sqrt|5*n>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|int><rsub|1><rsup|n<rsup|1/4>><frac|<sqrt|z*n>-<sqrt|n>|z>
    d z>|<cell|=>|<cell|<big|int><rsub|1><rsup|n<rsup|1/4>><sqrt|n>*<around*|(|<frac|1|<sqrt|z>>-<frac|1|z>|)>
    d z>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|t<rsub|k><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|k><rsub|j=0><around*|(|-1|)><rsup|k+j><binom|k|j>*\<tau\><rsub|j><around*|(|n|)>>>|<row|<cell|t<rsub|0>*<around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|0><around*|(|n|)>>>|<row|<cell|t<rsub|1><around*|(|n|)>>|<cell|=>|<cell|-\<tau\><rsub|0><around*|(|n|)>+\<tau\><rsub|1><around*|(|n|)>>>|<row|<cell|t<rsub|2><around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|0><around*|(|n|)>-2*\<tau\><rsub|1><around*|(|n|)>+\<tau\><rsub|2><around*|(|n|)>>>|<row|<cell|t<rsub|3><around*|(|n|)>>|<cell|=>|<cell|-\<tau\><rsub|0><around*|(|n|)>+3*\<tau\><rsub|1><around*|(|n|)>-3*\<tau\><rsub|2><around*|(|n|)>+\<tau\><rsub|3><around*|(|n|)>>>|<row|<cell|\<tau\><rsub|0><around*|(|n|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|0>|<cell|if n\<gtr\>1>>>>>>>|<row|<cell|\<tau\><rsub|1><around*|(|n|)>>|<cell|=>|<cell|1>>|<row|<cell|\<tau\><rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|k=1><around*|(|a<rsub|k>+1|)>>>|<row|<cell|\<tau\><rsub|k><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|k=1><binom|a<rsub|k>+k-1|k-1>=<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><rsub|k-1><around*|(|d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|T<rsub|3,odd><around*|(|n|)>+3*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>-2*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|3,odd><around*|(|n|)>+3*T<rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>+9*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>-6*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|8>|\<rfloor\>>|)>-2*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|3,odd><around*|(|n|)>+3*T<rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>+7*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>-6*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|8>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsub|3,odd><around*|(|n|)>+3*T<rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>+7*T<rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n|4>|\<rfloor\>>|)>+15*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|8>|\<rfloor\>>|)>-14*T<rsub|3><around*|(|<frac|n|16>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|a>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>>>|<row|<cell|b>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|2*n|x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|n|x>+<frac|n|x>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<around*|[|2*<around*|(|n
    mod x|)>\<geq\> x|]>>>>>
  </eqnarray*>

  Attempt to calculate <math|T<rsub|2><around*|(|n|)>,T<rsub|2><around*|(|n/2|)>>
  at the same time.

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|n;a,b|)>>|<cell|=>|<cell|2*S<around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>;a,b|)>+<big|sum><rsub|a\<leq\>x\<leq\>b,2*<around*|(|n
    mod x|)>\<geq\> x>1>>|<row|<cell|S<around*|(|n;1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>>|<cell|=>|<cell|2*S<around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>;1,<around*|\<nobracket\>|<around*|\<lfloor\>|<sqrt|<frac|n|2>>|\<rfloor\>>|)>+S<around*|(|n;<around*|\<lfloor\>|<sqrt|<frac|n|2>>|\<rfloor\>>+1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>+<big|sum><rsub|x\<leq\><around*|\<lfloor\>|<sqrt|n/2>|\<rfloor\>>,2*<around*|(|n
    mod x|)>\<geq\> x>1>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|x<rprime|'>>|<cell|=>|<cell|<frac|x+1|2>>>|<row|<cell|y<rprime|'>>|<cell|=>|<cell|<frac|y+1|2>>>|<row|<cell|x>|<cell|=>|<cell|2*x<rprime|'>-1>>|<row|<cell|y>|<cell|=>|<cell|2*y<rprime|'>-1>>|<row|<cell|x*y>|<cell|=>|<cell|n>>|<row|<cell|<around*|(|2*x<rprime|'>-1|)>*<around*|(|2*y<rprime|'>-1|)>>|<cell|=>|<cell|n>>>>
  </eqnarray*>

  Empirically <math|max<around*|(|a<rsub|i>|)>\<leq\>n<rsup|1/3>,max<around*|(|b<rsub|i>|)>\<leq\>n<rsup|1/6>,max<around*|(|c<rsub|i>|)>\<leq\>n<rsup|1/3>>.

  Alternative derivation of Möbius inversion of of
  <math|F<rsub|3><around*|(|n|)>> after Edwards.

  Use the substitution <math|f<around*|(|n|)>\<rightarrow\>f<around*|(|n|)>-p*f<around*|(|n<rsup|1/p>|)>>
  on both sides for <math|p=2,3,5,7,\<ldots\>>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>a*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>>|<row|<cell|F<rsub|3><around*|(|n|)>>|<cell|=>|<cell|*\<pi\><around*|(|n|)>+2*\<pi\><around*|(|*n<rsup|1/2>|)>+3*\<pi\><around*|(|*n<rsup|1/3>|)>+\<ldots\>>>|<row|<cell|F<rsub|3><around*|(|n|)>-2F<rsub|3><around*|(|n<rsup|1/2>|)>>|<cell|=>|<cell|\<pi\><around*|(|n|)>+3\<pi\><around*|(|n<rsup|1/3>|)>+5*\<pi\>*<around*|(|n<rsup|1/5>|)>+\<ldots\>>>|<row|<cell|F<rsub|3><around*|(|n|)>-2F<rsub|3><around*|(|n<rsup|1/2>|)>-3F<rsub|3><around*|(|n<rsup|1/3>|)>+6F<rsub|3><around*|(|n<rsup|1/6>|)>>|<cell|=>|<cell|\<pi\><around*|(|n|)>+5*\<pi\><around*|(|n<rsup|1/5>|)>+7*\<pi\><around*|(|n<rsup|1/7>|)>+\<ldots\>>>|<row|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>\<mu\><around*|(|a|)>*F<rsub|3><around*|(|n|)>>|<cell|=>|<cell|\<pi\><around*|(|n|)>>>>>
  </eqnarray*>

  because the left hand side consists only of primes and prime products with
  the sign depending on the number of prime factors and the right hand side
  terms eventually all be zero.

  How to show these three different formulations are equivalent (confirmed
  empirically)?

  <\eqnarray*>
    <tformat|<table|<row|<cell|2<rsup|\<omega\><around*|(|n|)>>>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|><around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*\<tau\><around*|(|d<rsup|2>|)>>>>>
  </eqnarray*>

  First from Tao, second from Apostol, third from Wikipedia arithmetic
  functions. \ Do they provide alternate formulas for computation?

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1><around*|(|a<rsub|i>+1|)>>>|<row|<cell|\<tau\><around*|(|n<rsup|2>|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1><around*|(|2*a<rsub|i>+1|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<rsup|><around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*\<tau\><around*|(|d<rsup|2>|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|<around*|(|2*a+1|)>-<around*|(|2*<around*|(|*a-1|)>+1|)>+0+\<ldots\>=2>|<cell|if
    n=p<rsup|a>>>|<row|<cell|<big|prod><rsub|i=1><rsup|k>2>|<cell|if
    n=p<rsub|1><rsup|a<rsub|1>>p<rsub|2><rsup|a<rsub|2>>\<ldots\>p<rsub|k<rsup|>><rsup|a<rsub|k>>>>>>>>>|<row|<cell|>|<cell|=>|<cell|2<rsup|\<omega\><around*|(|n|)>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>2<rsup|\<omega\><around*|(|n|)>>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*b=n>\<mu\><rsup|2><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x>\<mu\><rsup|2><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|x|\<rfloor\>>><rsub|b=1><big|sum><rsup|<around*|\<lfloor\>|x/b|\<rfloor\>>><rsub|a=1>\<mu\><rsup|2><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|><rsub|n\<leq\>x>Q<around*|(|<frac|x|n>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d\<leq\><sqrt|n>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|x/n|d<rsup|2>>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|n>>\<mu\><around*|(|d|)><big|sum><rsub|n\<leq\>x><around*|\<lfloor\>|<frac|x/d<rsup|2>|n>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|n>>\<mu\><around*|(|d|)>*T<around*|(|<around*|\<lfloor\>|<frac|x|d<rsup|2>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|Q<around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|x|d<rsup|2>>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a<rsup|2>*b\<leq\>x>\<mu\><around*|(|a|)>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<rsup|2>*b=n>\<mu\><around*|(|a|)>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|a|)>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x>\<mu\><rsup|2><around*|(|a|)>>>>>
  </eqnarray*>

  because

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|a<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|a|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|1>|<cell|if n=p>>|<row|<cell|1-1+0+\<ldots\>=0>|<cell|if
    n=p<rsup|a>,a\<geq\>2>>|<row|<cell|<big|prod><rsup|k><rsub|i=1><around*|[|a<rsub|i>=1|]>>|<cell|if
    n=p<rsub|1><rsup|a<rsub|1>>p<rsub|2><rsup|a<rsub|2>>\<ldots\>p<rsub|k<rsup|>><rsup|a<rsub|k>>>>>>>
    >>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n is squarefree>>|<row|<cell|0>|<cell|otherwise>>>>>>>|<row|<cell|>|<cell|=>|<cell|\<mu\><rsup|2><around*|(|n|)>>>>>
  </eqnarray*>

  Continuing the process

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|a<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|a|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|1>|<cell|if n=p or n=p<rsup|2>>>|<row|<cell|1-1+0+\<ldots\>=0>|<cell|if
    n=p<rsup|a>,a\<geq\>3>>|<row|<cell|<big|prod><rsub|i=1<rsup|>><rsup|k><around*|[|a<rsub|i\<leq\>2>|]>>|<cell|if
    n=p<rsub|1><rsup|a<rsub|1>>p<rsub|2><rsup|a<rsub|2>>\<ldots\>p<rsub|k<rsup|>>>>>>>>>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1
    in n is cubefree>>|<row|<cell|0 otherwise>>>>>>>>>
  </eqnarray*>

  which doesn't appear to have a simple equivalent.

  Bell series

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|\<infty\>><rsub|n=0>x<rsup|n><rsub|>>|<cell|=>|<cell|<frac|1|1-x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|\<infty\>><rsub|n=1>x<rsup|n><rsub|>>|<cell|=>|<cell|<big|sum><rsup|\<infty\>><rsub|n=0>x<rsup|n><rsub|>-1>>|<row|<cell|>|<cell|=>|<cell|<frac|1|1-x>-1>>|<row|<cell|>|<cell|=>|<cell|<frac|1|1-x>-<frac|1-x|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|x|1-x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>>k*x<rsup|2>>|<cell|=>|<cell|x+2*x<rsup|2>+3*x<rsup|3>+4*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|x*<around*|(|1+2*x<rsup|>+3*x<rsup|2>+4*x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|x*<around*|(|x<rsup|>+2*x<rsup|2>+3*x<rsup|3>+\<ldots\>+<around*|(|1+x+x<rsup|2>+x<rsup|3>+\<ldots\>.|)>|)>>>|<row|<cell|>|<cell|=>|<cell|x*<around*|(|<big|sum><rsub|k=1><rsup|\<infty\>>k*x<rsup|2>+<frac|1|1-x>|)>>>|<row|<cell|<around*|(|<big|sum><rsub|k=1><rsup|\<infty\>>k*x<rsup|2>*|)><around*|(|1-x|)>>|<cell|=>|<cell|<frac|x|1-x>>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>>k*x<rsup|2>>|<cell|=>|<cell|<frac|x|<around*|(|1-x|)><rsup|2>>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>|<cell|=>|<cell|x-x<rsup|2>+x<rsup|3>-x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|x-x<rsup|2>+x<rsup|3>-x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>-x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>>|<row|<cell|>|<cell|=>|<cell|x-x<rsup|2>+x<rsup|3>-x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|x<rsup|2>-x<rsup|3>+x<rsup|4>-x<rsup|5>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>>|<row|<cell|>|<cell|=>|<cell|x-x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>>|<row|<cell|<around*|(|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>|)>*<around*|(|1+x|)>>|<cell|=>|<cell|x>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>|<cell|=>|<cell|<frac|x|1+x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>|<cell|=>|<cell|x-2*x<rsup|2>+3*x<rsup|3>-4*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|x-2*x<rsup|2>+3*x<rsup|3>-4*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>-x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>>|<row|<cell|>|<cell|=>|<cell|x-2*x<rsup|2>+3*x<rsup|3>-4*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|x<rsup|2>-2*x<rsup|3>+3*x<rsup|4>-4*x<rsup|5>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>>|<row|<cell|>|<cell|=>|<cell|x-x<rsup|2>+x<rsup|3>-x<rsup|4>+\<ldots\>-x*<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>>|<row|<cell|<around*|(|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>|)>*<around*|(|1+x|)>>|<cell|=>|<cell|<frac|x|1+x>>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>|<cell|=>|<cell|<frac|x|<around*|(|1+x|)><rsup|2>>>>>>
  </eqnarray*>

  \;

  <\equation*>
    <block*|<tformat|<table|<row|<cell|<big|sum><rsup|\<infty\>><rsub|k=0>x<rsup|k><rsub|>>|<cell|1+x+x<rsup|2>+\<ldots\>>|<cell|<frac|1|1-x>>>|<row|<cell|<big|sum><rsup|\<infty\>><rsub|k=1>x<rsup|k><rsub|>>|<cell|x+x<rsup|2>+x<rsup|3>+\<ldots\>>|<cell|<frac|x|1-x>>>|<row|<cell|<big|sum><rsub|k=0><rsup|\<infty\>><around*|(|k+1|)>*x<rsup|k>>|<cell|1+2*x+3*x<rsup|2>+\<ldots\>>|<cell|<frac|1|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>>k*x<rsup|k>>|<cell|x+2*x<rsup|2>+3*x<rsup|3>+\<ldots\>>|<cell|<frac|x|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|<big|sum><rsub|k=0><rsup|\<infty\>><around*|(|-1|)><rsup|k>*x<rsup|k>>|<cell|1-x+x<rsup|2>-x<rsup|3>+\<ldots\>>|<cell|<frac|1|1+x>>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*x<rsup|k>>|<cell|x-x<rsup|2>+x<rsup|3>-\<ldots\>>|<cell|<frac|x|1+x>>>|<row|<cell|<big|sum><rsub|k=1><rsup|\<infty\>><around*|(|-1|)><rsup|k-1>*k*x<rsup|k>>|<cell|x-2*x<rsup|2>+3*x<rsup|3>-\<ldots\>>|<cell|<frac|x|<around*|(|1+x|)><rsup|2>>>>|<row|<cell|<big|sum><rsub|k=0><rsup|\<infty\>><around*|(|2*k+1|)>*x<rsup|k><rsub|>>|<cell|1+3*x+5*x<rsup|2>+7*x<rsup|3>+\<ldots\>>|<cell|<frac|1+x|<around*|(|1-x<rsup|>|)><rsup|2>>>>|<row|<cell|<big|sum><rsub|k=0><rsup|\<infty\>><around*|(|k+1|)><rsup|2>*x<rsup|k>>|<cell|1+4*x+9*x<rsup|2>+16*x<rsup|3>+\<ldots\>>|<cell|<frac|1+x|<around*|(|1-x|)><rsup|3>>>>>>>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|q<rsup|\<omega\><around*|(|n|)>><space|1em>for
    all q\<in\>\<bbb-Z\>>>|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|a=0><rsup|\<infty\>>f<around*|(|p<rsup|a>|)>x<rsup|a>>>|<row|<cell|>|<cell|=>|<cell|1+q*x+q*x<rsup|2>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+q*<around*|(|-1+1+x+x<rsup|2>+x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1+q*<around*|(|-1+<frac|1|1-x>|)>>>|<row|<cell|>|<cell|=>|<cell|1+q*<around*|(|<frac|1-<around*|(|1-x|)>|1-x>|)>>>|<row|<cell|>|<cell|=>|<cell|1+<frac|q*x|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|q*x+<around*|(|1-x|)>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+<around*|(|q-1|)>*x|1-x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|><around*|(|n|)>>|<cell|=>|<cell|q<rsup|\<Omega\><around*|(|n|)>><space|1em>for
    all q\<in\>\<bbb-Z\>>>|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|a=0><rsup|\<infty\>>f<around*|(|p<rsup|a>|)>x<rsup|a>>>|<row|<cell|>|<cell|=>|<cell|1+q*x+q<rsup|2*>x<rsup|2>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<frac|1|1-q*x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|3>>|)>>>|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|1+3*x+6*x<rsup|2>+9*x<rsup|3>+12*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+3*<around*|(|x+2*x<rsup|2>+3*x<rsup|3>+4*x<rsup|4>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1+3*<frac|x|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-2*x+x<rsup|2>+3*x|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+*x+x<rsup|2>|<around*|(|1-x|)><rsup|2>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|1+3*x+9*x<rsup|2>+9*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+3*x+9*x*<around*|(|x+x<rsup|2>+x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1+3*x+9*x<frac|x|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|1-x|)>+3*x*<around*|(|1-x|)>+9*x<rsup|2>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-x+3*x-3x<rsup|2>+9*x<rsup|2>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+2*x+6*x<rsup|2>|1-x>>>|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|?>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|n>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>>>|<row|<cell|\<Lambda\><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1>a<rsub|i>>>|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|3<rsup|\<omega\><around*|(|n|)>>*\<Lambda\><around*|(|n|)>*\<lambda\><around*|(|n|)>>>|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|1-3*x+6*x<rsup|2>-9*x<rsup|3>+12*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1-3*<around*|(|x-2*x<rsup|2>+3*x<rsup|3>-4*x<rsup|4>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1-<frac|3x|<around*|(|1+x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+2*x+x<rsup|2>-3*x|<around*|(|1+x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-x+x<rsup|2>|<around*|(|1+x|)><rsup|2>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|(|\<lambda\>\<star\>\<lambda\>|)><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<lambda\><around*|(|d|)>*\<lambda\><around*|(|<frac|n|d>|)>>>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|<big|sum><rsub|k=0><rsup|a><around*|(|-1|)><rsup|k>*<around*|(|-1|)><rsup|a-k>>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|<big|sum><rsub|k=0><rsup|a><around*|(|-1|)><rsup|a>>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|<around*|(|a+1|)><around*|(|-1|)><rsup|a>>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|n|)>*\<lambda\><around*|(|n|)><space|1em><around*|(|verified|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|1+x+x<rsup|2>>>|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>>>>
  </eqnarray*>

  \;

  <\equation*>
    <block*|<tformat|<table|<row|<cell|f<around*|(|n|)>>|<cell|meaning>|<cell|f<rsub|p><around*|(|n|)>>>|<row|<cell|\<mu\><around*|(|n|)>>|<cell|<choice|<tformat|<table|<row|<cell|<around*|(|-1|)><rsup|\<omega\><around*|(|n|)>>>|<cell|if
    \<omega\><around*|(|n|)>=\<Omega\><around*|(|n|)>>>|<row|<cell|0>|<cell|otherwise>>>>>>|<cell|1-x>>|<row|<cell|\<delta\><around*|(|n|)>>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|0>|<cell|otherwise>>>>>=\<tau\><rsub|0><around*|(|n|)>>|<cell|1>>|<row|<cell|\<lambda\><around*|(|n|)>>|<cell|<around*|(|-1|)><rsup|\<Omega\><around*|(|n|)>>>|<cell|<frac|1|1+x>>>|<row|<cell|\<lambda\><around*|(|n|)>*\<tau\><around*|(|n|)>>|<cell|<around*|(|\<lambda\>\<star\>\<lambda\>|)><around*|(|n|)>>|<cell|<frac|1|<around*|(|1+x|)><rsup|2>>>>|<row|<cell|Id<rsub|k><around*|(|n|)>>|<cell|n<rsup|k>>|<cell|<frac|1|1-p<rsup|k>*x>>>|<row|<cell|Id<rsub|0><around*|(|n|)>>|<cell|1=\<tau\><rsub|1><around*|(|n|)>>|<cell|<frac|1|1-x>>>|<row|<cell|\<sigma\><rsub|k><around*|(|n|)>>|<cell|>|<cell|<frac|1|1-<around*|(|1+p<rsup|k>|)>*x+p<rsup|k>*x<rsup|2>>>>|<row|<cell|\<tau\><rsub|k><around*|(|n|)>>|<cell|<around*|(|\<tau\><rsub|k-1>\<star\>Id<rsub|o>|)><around*|(|n|)>>|<cell|<frac|1|<around*|(|1-x|)><rsup|k>>>>|<row|<cell|\<tau\><around*|(|n|)>>|<cell|\<sigma\><rsub|0><around*|(|n|)>=\<tau\><rsub|2><around*|(|n|)>>|<cell|<frac|1|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|\<mu\><rsup|2><around*|(|n|)>>|<cell|\<lambda\><rsup|-1><around*|(|n|)>>|<cell|1+x>>|<row|<cell|q<rsup|\<omega\><around*|(|n|)>>>|<cell|>|<cell|<frac|1+<around*|(|q-1|)>*x|1-x>>>|<row|<cell|q<rsup|\<Omega\><around*|(|n|)>>>|<cell|>|<cell|<frac|1|1-q*x>>>|<row|<cell|2<rsup|\<omega\><around*|(|n|)>>>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|><around*|(|<frac|n|d<rsup|2>>|)>>|<cell|<frac|1+x|1-x>>>|<row|<cell|<around*|(|-2|)><rsup|\<omega\><around*|(|n|)>>>|<cell|>|<cell|<frac|1-3x|1-x>>>|<row|<cell|\<mu\><rsup|2><around*|(|n|)>*2<rsup|\<omega\><around*|(|n|)>>>|<cell|\<mu\><rsup|2><around*|(|n|)>*\<tau\><around*|(|n|)>>|<cell|1+2*x>>|<row|<cell|3<rsup|\<omega\><around*|(|n|)>>>|<cell|>|<cell|<frac|1+2*x|1-x>>>|<row|<cell|<around*|(|-3|)><rsup|\<omega\><around*|(|n|)>>>|<cell|>|<cell|<frac|1-4*x|1-x>>>|<row|<cell|\<mu\><around*|(|n|)>*q<rsup|\<omega\><around*|(|n|)>>>|<cell|\<mu\><rsup|><around*|(|n|)>*\<tau\><rsub|q><around*|(|n|)>>|<cell|1-q*x>>|<row|<cell|\<mu\><rsup|2><around*|(|n|)>*q<rsup|\<omega\><around*|(|n|)>>>|<cell|\<mu\><rsup|2><around*|(|n|)>*\<tau\><rsub|q><around*|(|n|)>>|<cell|1+q*x>>|<row|<cell|<big|sum><rsub|d<rsup|k><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|Q<rsub|k><around*|(|n|)>>|<cell|<frac|1-x<rsup|k>|1-x>=1+x+x<rsup|2>+\<ldots\>+x<rsup|k-1>>>|<row|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|\<mu\><rsup|2><around*|(|n|)>=<around*|\||\<mu\><around*|(|n|)>|\|>=Q<rsub|2><around*|(|n|)>>|<cell|1+x>>|<row|<cell|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|Q<rsub|3><around*|(|n|)>>|<cell|1+x+x<rsup|2>>>|<row|<cell|\<lambda\><around*|(|n|)><big|sum><rsub|d<rsup|k><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|>|<cell|1-x+x<rsup|2>-\<ldots\>\<pm\>x<rsup|k>>>|<row|<cell|\<lambda\><around*|(|n|)>*<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|>|<cell|1-x>>|<row|<cell|\<lambda\><around*|(|n|)>*<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>|<cell|>|<cell|1-x+x<rsup|2>>>|<row|<cell|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|3>>|)>>|<cell|>|<cell|<frac|1+*x+x<rsup|2>|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|\<tau\><around*|(|n<rsup|2>|)>>|<cell|>|<cell|<frac|1+x|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|\<tau\><rsup|2><around*|(|n|)>>|<cell|>|<cell|<frac|1+x|<around*|(|1-x|)><rsup|3>>>>|<row|<cell|\<mu\><rsub|2><around*|(|n|)>>|<cell|<choice|<tformat|<table|<row|<cell|\<mu\><around*|(|m|)>>|<cell|if
    n=m<rsup|2>>>|<row|<cell|0>|<cell|otherwise>>>>>>|<cell|1-x<rsup|2>>>|<row|<cell|\<mu\><rsub|k><around*|(|n|)>>|<cell|<choice|<tformat|<table|<row|<cell|\<mu\><around*|(|m|)>>|<cell|if
    n=m<rsup|k>>>|<row|<cell|0>|<cell|otherwise>>>>>>|<cell|1-x<rsup|k>>>>>>
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*2<rsup|\<omega\><around*|(|d|)>>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|1+2+0+\<ldots\>=3>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|3<rsup|\<omega\><around*|(|n|)>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*<big|sum><rsub|a<rsup|2><around*|\||d|\<nobracket\>>>\<mu\><around*|(|a|)>*\<tau\><around*|(|<frac|d|a<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b=n>\<mu\><rsup|2><around*|(|a|)>*<big|sum><rsub|c<rsup|2>*d=a>\<mu\><around*|(|c|)>*\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<rsup|<rsup|>><big|sum><rsub|a*b=n>\<mu\><rsup|2><around*|(|a|)>*<big|sum><rsub|d=a>*\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*\<tau\><around*|(|d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<rsup|2>*b\<leq\>x>\<tau\><around*|(|d<rsup|2>*b|)>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>><big|sum><rsub|b\<leq\>x/d<rsup|2>>\<tau\><around*|(|d<rsup|2>*b|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)>*2<rsup|\<omega\><around*|(|n|)>>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)><big|sum><rsub|d<around*|\||n|\<nobracket\>>><big|sum><rsub|a<rsup|2><around*|\||d|\<nobracket\>>>\<mu\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b*c\<leq\>x>\<mu\><around*|(|b*c|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)>*\<tau\><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)><big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*<rsup|2>b\<leq\>x>\<mu\><around*|(|a<rsup|2>*b|)>*\<mu\><around*|(|a|)>*\<tau\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><around*|(|n|)>**\<tau\><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b*\<leq\>x><big|sum><rsub|c\<leq\>x/b>\<mu\><around*|(|b*c|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b*\<leq\>x><big|sum><rsub|c\<leq\>x/b,<around*|(|b,c|)>=1>\<mu\><around*|(|b|)>*\<mu\><around*|(|c|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b*\<leq\>x>\<mu\><around*|(|b|)><big|sum><rsub|c\<leq\>x/b,<around*|(|b,c|)>=1>*\<mu\><around*|(|c|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>3<rsup|\<omega\><around*|(|n|)>>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*2<rsup|\<omega\><around*|(|d|)>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*<big|sum><rsub|a<rsup|2><around*|\||d|\<nobracket\>>>\<mu\><around*|(|a|)>*\<tau\><around*|(|<frac|d|a<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*b=n>\<mu\><rsup|2><around*|(|a|)>*<big|sum><rsub|c<rsup|2>*d=a>\<mu\><around*|(|c|)>*\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<rsup|2>*b*c=n>\<mu\><rsup|2><around*|(|a<rsup|2>*b|)>*\<mu\><around*|(|a|)>*\<tau\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<rsup|>*b=n>\<mu\><rsup|2><around*|(|*a|)>**\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a<rsup|>*b\<leq\>x>\<mu\><rsup|2><around*|(|*a|)>**\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x>\<mu\><rsup|2><around*|(|a|)><big|sum><rsub|b\<leq\>x/a>\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x>\<mu\><rsup|2><around*|(|a|)>*\<tau\><around*|(|a|)>*<around*|\<lfloor\>|<frac|x|a>|\<rfloor\>><space|1em><around*|(|verified|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|*a|)>**\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<tau\><around*|(|a|)><big|sum><rsub|d<rsup|2><around*|\||a|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<rsup|2>b*c=n>\<tau\><around*|(|a<rsup|2>*b|)>*\<mu\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a<rsup|2>*b*c\<leq\>x>\<tau\><around*|(|a<rsup|2>*b|)>*\<mu\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\><sqrt|x>>\<mu\><around*|(|a|)><big|sum><rsub|b\<leq\>x/a<rsup|2>>\<tau\><around*|(|a<rsup|2>*b|)><big|sum><rsub|c\<leq\>x/<around*|(|a<rsup|2>*b|)>>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\><sqrt|x>>\<mu\><around*|(|a|)>*<big|sum><rsub|b\<leq\>x/a<rsup|2>>\<tau\><around*|(|a<rsup|2>*b|)>*<around*|\<lfloor\>|<frac|x|a<rsup|2>*b>|\<rfloor\>><space|1em><around*|(|verified|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><rsup|2><around*|(|d|)>*<big|sum><rsub|a<around*|\||d|\<nobracket\>>>\<mu\><rsup|2><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*b*c=n>\<mu\><rsup|2><around*|(|a*b|)>*\<mu\><rsup|2><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*b*c=n>\<mu\><rsup|2><around*|(|a*b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x>\<mu\><rsup|2><around*|(|a*b|)><big|sum><rsub|c\<less\>x/<around*|(|a*b|)>>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*\<leq\>x><big|sum><rsub|b\<leq\>x/a>\<mu\><rsup|2><around*|(|a*b|)>*<around*|\<lfloor\>|<frac|x|a*b>|\<rfloor\>><space|1em><around*|(|verified|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*b*c=n><big|sum><rsub|d<rsup|2><around*|\||a|\<nobracket\>>*b>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x>\<tau\><around*|(|a|)>*<around*|\<lfloor\>|<frac|x|a>|\<rfloor\>>*<big|sum><rsub|d<rsup|2><around*|\||a|\<nobracket\>>*>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|<rsup|>2>*b\<leq\>x>\<mu\><around*|(|d|)>*\<tau\><around*|(|d<rsup|2>*b|)>*<around*|\<lfloor\>|<frac|x|d<rsup|2>*b>|\<rfloor\>>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>>\<mu\><around*|(|d|)><big|sum><rsub|b\<leq\>x/d<rsup|2>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>\<mu\><rsup|2><around*|(|n|)>*\<tau\><rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<tau\><rsub|3><around*|(|n|)>*<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a<rsup|2>*b\<leq\>x>\<mu\><around*|(|a|)>*\<tau\><rsub|3><around*|(|a<rsup|2>*b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\><sqrt|x>>\<mu\><around*|(|a|)>*<big|sum><rsub|b\<leq\>x/a<rsup|2>>\<tau\><rsub|3><around*|(|a<rsup|2>*b|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<mu\><rsup|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*2<rsup|\<omega\><around*|(|n|)>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|a>|)><big|sum><rsub|d<rsup|2><around*|\||a|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|a|d<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2>*b*c=n>\<mu\><around*|(|c|)>*\<mu\><around*|(|d|)>*\<tau\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*<big|sum><rsub|a<around*|\||n<rsup|>/d<rsup|2>|\<nobracket\>>>\<mu\><around*|(|a|)>*\<tau\><around*|(|<frac|n/d<rsup|2>|a>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*Id<rsub|0><around*|(|<frac|n|d<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|(|\<tau\>\<star\><around*|(|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>|)>|)><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<tau\><around*|(|a|)>*<big|sum><rsub|b<rsup|3><around*|\||n/a|\<nobracket\>>>\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b<rsup|3>*c=n>\<tau\><around*|(|a|)>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|b|)><big|sum><rsub|c<around*|\||n/b<rsup|3>|\<nobracket\>>>*\<tau\><around*|(|<frac|n|b<rsup|3>*c>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|b|)>*\<tau\><rsub|3><around*|(|<frac|n|b<rsup|3>>|)>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|(|<around*|(|\<lambda\><around*|(|n|)>*\<tau\><around*|(|n|)>|)>\<star\><around*|(|\<lambda\><around*|(|n|)>*<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>|)>|)><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|a*b=n>\<lambda\><around*|(|a|)>*\<tau\><around*|(|a|)>*\<lambda\><around*|(|b|)>*<big|sum><rsub|d<rsup|3><around*|\||b|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b=n>\<lambda\><around*|(|a*b|)>*\<tau\><around*|(|a|)>**<big|sum><rsub|d<rsup|3><around*|\||b|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b=n>\<lambda\><around*|(|n|)>*\<tau\><around*|(|a|)>**<big|sum><rsub|d<rsup|3><around*|\||b|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|\<lambda\><around*|(|n|)><big|sum><rsub|a*b=n>*\<tau\><around*|(|a|)>**<big|sum><rsub|d<rsup|3><around*|\||b|\<nobracket\>>>\<mu\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|\<lambda\><around*|(|n|)><big|sum><rsub|a*b<rsup|3>*c=n>*\<tau\><around*|(|a|)>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|\<lambda\><around*|(|n|)><big|sum><rsub|b<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|b|)>*<big|sum><rsub|c<around*|\|||\<nobracket\>>n/b<rsup|3>>\<tau\><around*|(|<frac|n|b<rsup|3>*c>|)>>>|<row|<cell|>|<cell|=>|<cell|\<lambda\><around*|(|n|)><big|sum><rsub|b<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|b|)>*\<tau\><rsub|3><around*|(|<frac|n|b<rsup|3>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x,d
    odd>\<tau\><around*|(|d|)>>|<cell|=>|<cell|\<tau\><around*|(|1|)>+\<tau\><around*|(|3|)>+\<tau\><around*|(|5|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>x>\<tau\><around*|(|d|)>-<big|sum><rsub|d\<leq\>x,d
    even>\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>x>\<tau\><around*|(|d|)>-<around*|(|\<tau\><around*|(|2|)>+\<tau\><around*|(|4|)>+\<tau\><around*|(|6|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>x>\<tau\><around*|(|d|)>-<around*|(|2*\<tau\><around*|(|1|)>+2*\<tau\><around*|(|3|)>+2*\<tau\><around*|(|5|)>+\<ldots\>+\<tau\><around*|(|4|)>+\<tau\><around*|(|8|)>+\<tau\><around*|(|12|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|T<around*|(|x|)>-2*T<around*|(|<frac|x|2>|)>+T<around*|(|<frac|x|4>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x/2<rsup|2>>\<tau\><around*|(|2<rsup|2>*d|)>>|<cell|=>|<cell|\<tau\><around*|(|4|)>+\<tau\><around*|(|8|)>+\<tau\><around*|(|12|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|4|)>*\<tau\><around*|(|*1|)>+*\<tau\><around*|(|8|)>+\<tau\><around*|(|4|)>*\<tau\><around*|(|3|)>+\<tau\><around*|(|16|)>+\<tau\><around*|(|4|)>*t<around*|(|5|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|4|)>*<around*|(|\<tau\><around*|(|1|)>+\<tau\><around*|(|3|)>+\<tau\><around*|(|5|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|+>|<cell|\<tau\>*<around*|(|8|)>+\<tau\><around*|(|16|)>+\<tau\><around*|(|24|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|4|)>*<big|sum><rsub|k\<leq\>d/4,k
    odd>\<tau\><around*|(|k|)>+\<tau\><around*|(|8|)>*<big|sum><rsub|k\<leq\>d/8,k
    odd>\<tau\><around*|(|k|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>i><around*|(|i+2|)>*<big|sum><rsub|k\<leq\>d/2<rsup|i+1>,k
    odd>\<tau\><around*|(|k|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>i><around*|(|i+2|)>*<around*|(|T<around*|(|<frac|x|2<rsup|i+1>>|)>-2*T<around*|(|<frac|x/2|2<rsup|i+1>>|)>+T<around*|(|<frac|x/4|2<rsup|i+1>>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|3*T<around*|(|<frac|x|2<rsup|2>>|)>-2*T<around*|(|<frac|x|2<rsup|3>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p<rsup|2>*d|)>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p<rsup|2>|)>*\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|->|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|2>|)>*\<tau\><around*|(|p*d|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|p<rsup|2>|)>*<around*|(|<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|d|)>-<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p*d|)>|)>+<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>>|<cell|=>|<cell|\<tau\><around*|(|p<rsup|3>|)>*<around*|(|<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|d|)>-<big|sum><rsub|d\<leq\>x/p<rsup|4>>\<tau\><around*|(|p*d|)>|)>+<big|sum><rsub|d\<leq\>x/p<rsup|4>>\<tau\><around*|(|p<rsup|4>*d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|>>\<tau\><around*|(|p*d|)>>|<cell|=>|<cell|\<tau\><around*|(|p|)>*<around*|(|<big|sum><rsub|d\<leq\>x/p>\<tau\><around*|(|d|)>-<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p*d|)>|)>+<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p<rsup|2>*d|)>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|p|)>*<around*|(|<big|sum><rsub|d\<leq\>x/p>\<tau\><around*|(|d|)>-<around*|(|\<tau\><around*|(|p|)>*<around*|(|<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|d|)>-<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p*d|)>|)>+<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>|)>|)>+<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p<rsup|2>*d|)>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p>\<tau\><around*|(|d|)>-\<tau\><rsup|2><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|d|)>+\<tau\><rsup|2><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p*d|)>-\<tau\><rsup|><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>+\<tau\><around*|(|p<rsup|2>|)>*<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|d|)>-\<tau\><around*|(|p<rsup|2>|)>*<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p*d|)>+<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p>\<tau\><around*|(|d|)>-<around*|(|\<tau\><rsup|2><around*|(|p|)>-\<tau\><around*|(|p<rsup|2>|)>|)>*<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|d|)>+<around*|(|\<tau\><rsup|2><around*|(|p|)>-\<tau\><around*|(|p<rsup|2>|)>|)>*<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p*d|)>-<around*|(|\<tau\><rsup|><around*|(|p|)>-1|)>*<big|sum><rsub|d\<leq\>x/p<rsup|3>>\<tau\><around*|(|p<rsup|3>*d|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>x/p<rsup|>>\<tau\><around*|(|p*d|)>+\<tau\><around*|(|p|)>*<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p*d|)>>|<cell|=>|<cell|\<tau\><around*|(|p|)><big|sum><rsub|d\<leq\>x/p>\<tau\><around*|(|d|)>+<big|sum><rsub|d\<leq\>x/p<rsup|2>>\<tau\><around*|(|p<rsup|2>*d|)>>>|<row|<cell|F<rsub|p><around*|(|x|)>+\<tau\><around*|(|p|)>*F<rsub|p><around*|(|x/p|)>>|<cell|=>|<cell|\<tau\><around*|(|p|)>*T<around*|(|x/p|)>+F<rsub|p<rsup|2>><around*|(|x|)>>>|<row|<cell|F<rsub|p><around*|(|x|)>+\<tau\><around*|(|p|)>*F<rsub|p><around*|(|x/p|)>+\<tau\><around*|(|p<rsup|2>|)>*F<rsub|p><around*|(|x/p<rsup|2>|)>>|<cell|=>|<cell|\<tau\><around*|(|p|)>*T<around*|(|x/p|)>+\<tau\><around*|(|p<rsup|2>|)>*T<around*|(|x/p<rsup|2>|)>+F<rsub|p<rsup|3>><around*|(|x|)>>>|<row|<cell|<big|sum><rsub|0\<leq\>k\<leq\>log<rsub|p>
    x>\<tau\><around*|(|p<rsup|k>|)>*F<rsub|p><around*|(|x/p<rsup|k>|)>>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x>\<tau\><around*|(|p<rsup|k>|)>*T<around*|(|x/p<rsup|k>|)>>>|<row|<cell|<big|sum><rsub|0\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*F<rsub|p><around*|(|x/p<rsup|k>|)>>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*T<around*|(|x/p<rsup|k>|)>>>|<row|<cell|F<rsub|p><around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*T<around*|(|x/p<rsup|k>|)>-<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*F<rsub|p><around*|(|x/p<rsup|k>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*T<around*|(|x/p<rsup|k>|)>-2*<around*|(|<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x/p><around*|(|k+1|)>*T<around*|(|x/p<rsup|k+1>|)>-<big|sum><rsub|1\<leq\>k\<leq\>log<rsub|p>
    x/p><around*|(|k+1|)>*F<rsub|p><around*|(|x/p<rsup|k+1>|)>|)>-<big|sum><rsub|2\<leq\>k\<leq\>log<rsub|p>
    x><around*|(|k+1|)>*F<rsub|p><around*|(|x/p<rsup|k>|)>>>|<row|<cell|>|<cell|=>|<cell|2*T<around*|(|x/p|)>+3*T<around*|(|x/p<rsup|2>|)>+4*T<around*|(|x/p<rsup|3>|)>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|4*T<around*|(|x/p<rsup|2>|)>-6*T<around*|(|x/p<rsup|3>|)>-8*T<around*|(|p/<rsup|4>|)>-\<ldots\>>>|<row|<cell|>|<cell|->|<cell|6*T<around*|(|x/p<rsup|3>|)>-9*T<around*|(|x/p<rsup|4>|)>-12*T<around*|(|p/<rsup|5>|)>-\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|2*T<around*|(|x/p|)>+<around*|(|3-2\<cdot\>2|)>*T<around*|(|x/p<rsup|2>|)>+<around*|(|4-2\<cdot\>3-3\<cdot\>2|)>*T<around*|(|x/p<rsup|3>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|5-2\<cdot\>4-3\<cdot\>3-4\<cdot\>2|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>k><around*|[|<around*|(|<around*|(|k+1|)>-<big|sum><rsub|2\<leq\>j\<leq\>k>j*<around*|(|k-j+2|)>|)>*T<around*|(|<frac|x|p<rsup|k>>|)>|]>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<cwith|1|1|1|-1|cell-background|pastel
    cyan>|<cwith|3|3|1|-1|cell-background|pastel
    cyan>|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/p>\<tau\><around*|(|p*n|)>>|<cell|=>|<cell|2*<big|sum><rsub|n\<leq\>x/p>\<tau\><around*|(|n|)>-<big|sum><rsub|n\<leq\>x/p<rsup|2>>\<tau\><around*|(|n|)><space|1em><around*|(|verified|)>>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x/p>\<tau\><around*|(|p*<rsup|2>n|)>>|<cell|=>|<cell|3<big|sum><rsub|n\<leq\>x/p<rsup|2>>\<tau\><around*|(|n|)>-2*<big|sum><rsub|n\<leq\>x/p<rsup|3>>\<tau\><around*|(|n|)><space|1em><around*|(|verified|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x/p*q>\<tau\><around*|(|p*q*n|)>>|<cell|=>|<cell|2*<around*|(|<big|sum><rsub|n\<leq\>x/p>\<tau\><around*|(|n|)>+<big|sum><rsub|n\<leq\>x/q>\<tau\><around*|(|n|)>-<big|sum><rsub|n\<leq\>x/p*q>\<tau\><around*|(|n|)>|)>>>|<row|<cell|>|<cell|->|<cell|<around*|(|<big|sum><rsub|n\<leq\>x/p<rsup|2>>\<tau\><around*|(|n|)>+<big|sum><rsub|n\<leq\>x/q<rsup|2>>\<tau\><around*|(|n|)>-<big|sum><rsub|n\<leq\>x/<around*|(|p*q|)><rsup|2>>\<tau\><around*|(|n|)>|)>>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p|)>+P<around*|(|x;q|)>-P<around*|(|x;p*q|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x/p*q>\<tau\><around*|(|p*q*n|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x,p*q<around*|\||n|\<nobracket\>>>\<tau\><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x,p*q<around*|\||n|\<nobracket\>>><big|sum><rsub|a*b=n>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x,p*q<around*|\||a*b|\<nobracket\>>>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x,p*q<around*|\||a|\<nobracket\>>>1+<big|sum><rsub|a*b\<leq\>x,p*q*<around*|\||b|\<nobracket\>>>1+<big|sum><rsub|a*b\<leq\>x,p*<around*|\||a,q|\|>b>1+<big|sum><rsub|a*b\<leq\>x,p*<around*|\||b,q|\|>a>1>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p*q|)>+2*<big|sum><rsub|a*b\<leq\>x,p*<around*|\||a,q|\|>b>1>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p*q|)>+2*<big|sum><rsub|a*b*p*q\<leq\>x>1>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p*q|)>+2*<big|sum><rsub|a*b*\<leq\>x/<around*|(|p*q|)>>1>>|<row|<cell|>|<cell|=>|<cell|P<around*|(|x;p*q|)>+2*T<around*|(|x/p*q|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x/p*q>\<tau\><around*|(|p*q*n|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/p*q><big|sum><rsub|a*b=n><big|sum><rsub|p<around*|\||n|\<nobracket\>>><big|sum><rsub|q<around*|\||n/p|\<nobracket\>>>1>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\><sqrt|x>>d<around*|(|n<rsup|2>|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\><sqrt|x>><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*\<tau\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\><sqrt|x>>\<mu\><around*|(|a|)>*\<tau\><rsup|2><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\><sqrt|x>>\<mu\><around*|(|a|)>*<big|sum><rsub|b\<leq\><sqrt|x>/a>\<tau\><rsup|2><around*|(|b|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>2<rsup|\<omega\><around*|(|n|)>>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<rsup|><around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*\<tau\><around*|(|d<rsup|2>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x>\<mu\><around*|(|a|)>*\<tau\><around*|(|b<rsup|2>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\>x>\<mu\><around*|(|a|)>*<big|sum><rsub|b\<leq\>x/a>\<tau\><around*|(|b<rsup|2>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><around*|(|d<rsup|2>|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|2*a+1>|<cell|if n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|<big|prod><rsub|i=1><rsup|\<omega\><around*|(|n|)>><around*|(|2*a<rsub|i>+1|)>>>|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+3*x+5*x<rsup|2>+7*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|i=0><rsup|\<infty\>><around*|(|2*i+1|)>*x<rsup|i>>>|<row|<cell|>|<cell|=>|<cell|1+3*x+5*x<rsup|2>+7*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|<around*|(|1+x+x<rsup|2>+x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|1+x+x<rsup|2>+x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|2*x+4*x<rsup|2>+6*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|<frac|1|1-x>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|(|x+2*x<rsup|2>+3*x<rsup|3>+\<ldots\>|)>+<frac|1|1-x>>>|<row|<cell|>|<cell|=>|<cell|2*<frac|x|<around*|(|1-x|)><rsup|2>>+<frac|1|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|2*x+<around*|(|1-x|)>|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+x|<around*|(|1-x<rsup|>|)><rsup|2>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><around*|(|n<rsup|2>|)>>|<cell|=>|<cell|<big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<tau\><around*|(|<frac|n|a>|)><big|sum><rsub|b<rsup|2><around*|\||a|\<nobracket\>>>\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d*b<rsup|2>*c=n>\<tau\><around*|(|c|)>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b<rsup|2>*=n>\<mu\><around*|(|b|)>*<big|sum><rsub|d<around*|\|||\<nobracket\>>a><rsub|>\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a*b<rsup|2>*=n>\<mu\><around*|(|b|)>*\<tau\><rsub|3><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|b|)>*\<tau\><rsub|3><around*|(|<frac|n|b<rsup|2>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>\<tau\><around*|(|n<rsup|2>|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a<around*|\||n|\<nobracket\>>>\<tau\><around*|(|<frac|n|a>|)><big|sum><rsub|b<rsup|2><around*|\||a|\<nobracket\>>>\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|a*c=n>\<tau\><around*|(|c|)>*<big|sum><rsub|b<rsup|2><around*|\||a|\<nobracket\>>>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d*b<rsup|2>*c=n>\<tau\><around*|(|c|)>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d*b<rsup|2>*c\<leq\>x>\<tau\><around*|(|c|)>*\<mu\><around*|(|b|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b\<leq\><sqrt|x>>\<mu\><around*|(|b|)>*<big|sum><rsub|d*c\<leq\>x/b<rsup|2>>\<tau\><around*|(|c|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b\<leq\><sqrt|x>>\<mu\><around*|(|b|)>*<big|sum><rsub|n\<leq\>x/b<rsup|2>><big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b\<leq\><sqrt|x>>\<mu\><around*|(|b|)>*<big|sum><rsub|n\<leq\>x/b<rsup|2>>\<tau\><rsub|3><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|a\<leq\><sqrt|x>>\<mu\><around*|(|a|)>*T<rsub|3><around*|(|<frac|x|*a<rsup|2>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<cwith|1|-1|1|-1|cell-background|pastel
    cyan>|<table|<row|<cell|\<tau\><around*|(|n<rsup|2>|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<mu\><around*|(|<frac|n|d>|)>*\<tau\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><around*|(|<frac|n|d>|)>*\<mu\><rsup|2><around*|(|d|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|2>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x>\<tau\><around*|(|n<rsup|2>|)>*\<mu\><rsup|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><around*|[|<around*|(|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|2>>|)>|)><around*|(|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>|)>|]>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|e<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|e|)><big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|2>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|e\<leq\><sqrt|x>>\<mu\><around*|(|e|)><big|sum><rsub|d\<leq\><sqrt|x>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><rsup|2><around*|(|n|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|<around*|(|a+1|)><rsup|2>>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|>|<cell|=>|<cell|<big|prod><rsub|i=1><rsup|\<omega\><around*|(|n|)>><around*|(|a+1|)><rsup|2>>>|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+4*x+9*x<rsup|2>+16*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+*x+4x<rsup|2>+9*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|x+3*x<rsup|2>+5*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|2*x+2*x<rsup|2>+2*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+x*<around*|(|1+4*x+9*x<rsup|2>+\<ldots\>|)>+x*<frac|1+x|<around*|(|1-x|)><rsup|2>>+2<frac|x|1-x>>>|<row|<cell|>|<cell|=>|<cell|1*+x*f<rsub|p><around*|(|n|)>+<frac|x+x<rsup|2>+2*x*<around*|(|1-x|)>|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-2*x+x<rsup|2>+x+x<rsup|2>+2*x-2*x<rsup|2>|<around*|(|1-x|)><rsup|3>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+x|<around*|(|1-x|)><rsup|3>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>f<around*|(|d|)>*g<around*|(|<frac|n|d<rsup|2>>|)>>|<cell|>|<cell|>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<delta\><around*|(|n|)>>|<cell|=>|<cell|<around*|(|3<rsup|\<Omega\><around*|(|n|)>>\<star\>\<mu\><around*|(|n|)>*3<rsup|\<omega\><around*|(|n|)>>|)><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||n|\<nobracket\>>>3<rsup|\<Omega\><around*|(|n/d|)>>*\<mu\><around*|(|d|)>*3<rsup|\<omega\><around*|(|d|)>>>>|<row|<cell|>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|3<rsup|a>*1-3<rsup|a-1>*3=0>|<cell|if
    n=p<rsup|a>>>>>>>>|<row|<cell|0>|<cell|=>|<cell|<big|sum><rsub|a*b\<leq\>x>3<rsup|\<Omega\><around*|(|a|)>>*\<mu\><around*|(|b|)>*3<rsup|\<omega\><around*|(|b|)>><space|1em>for
    all x\<gtr\>1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|b*\<leq\>x>\<mu\><around*|(|b|)>*3<rsup|\<omega\><around*|(|b|)>>*<big|sum><rsub|a\<leq\>x/b>3<rsup|\<Omega\><around*|(|a|)>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|><around*|(|n|)>>|<cell|=>|<cell|<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|k=1>a<rsub|k>>>|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+x+2*x<rsup|2>+3*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+x*<around*|(|1+2*x+3*x<rsup|2>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1+x*<frac|1|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-2*x+x<rsup|2>+x|<around*|(|1-x|)><rsup|2>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-x+x<rsup|2>|<around*|(|1-x|)><rsup|2>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<mu\><around*|(|n|)>*2<rsup|2*\<omega\><around*|(|n|)>>>|<cell|=>|<cell|\<mu\><around*|(|n|)>*<around*|(|<big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|><around*|(|<frac|n|d<rsup|2>>|)>|)><rsup|2>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|><around*|(|x;c|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/c>\<tau\><around*|(|c*n|)>>>|<row|<cell|P<rsub|><around*|(|x;p|)>>|<cell|=>|<cell|S<rsub|><around*|(|x;p|)>=2*T<around*|(|<frac|x|p>|)>-T<around*|(|<frac|x|p<rsup|2>>|)>>>|<row|<cell|>|<cell|>|<cell|>>|<row|<cell|<big|sum><rsub|n\<leq\>x/6>\<tau\><around*|(|6*n|)>>|<cell|=>|<cell|S<around*|(|x;6|)>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|6\<cdot\>1|)>+\<tau\><around*|(|6\<cdot\>2|)>+\<tau\>*<around*|(|6\<cdot\>3|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|\<tau\><around*|(|6|)>*<around*|(|\<tau\><around*|(|1|)>+\<tau\><around*|(|2|)>+\<tau\><around*|(|3|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|->|<cell|\<tau\><around*|(|6|)>*<around*|(|\<tau\><around*|(|2|)>+\<tau\><around*|(|4|)>+\<tau\><around*|(|6|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|->|<cell|\<tau\><around*|(|6|)>*<around*|(|\<tau\><around*|(|3|)>+\<tau\><around*|(|6|)>+\<tau\><around*|(|9|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|+>|<cell|\<tau\><around*|(|6|)>*<around*|(|\<tau\><around*|(|6|)>+\<tau\><around*|(|12|)>+\<tau\><around*|(|18|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|+>|<cell|\<tau\><around*|(|6\<cdot\>2*\<cdot\>1|)>+\<tau\><around*|(|6\<cdot\>2*\<cdot\>2|)>+\<tau\><around*|(|6\<cdot\>2*\<cdot\>3|)>+\<ldots\>>>|<row|<cell|>|<cell|+>|<cell|\<tau\><around*|(|6\<cdot\>3*\<cdot\>1|)>+\<tau\><around*|(|6\<cdot\>3*\<cdot\>2|)>+\<tau\><around*|(|6\<cdot\>3*\<cdot\>3|)>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|\<tau\><around*|(|6\<cdot\>6*\<cdot\>1|)>+\<tau\><around*|(|6\<cdot\>6*\<cdot\>2|)>+\<tau\><around*|(|6\<cdot\>6*\<cdot\>3|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|4*<around*|(|T<around*|(|<frac|x|6>|)>-P<rsub|><around*|(|<frac|x|6>;2|)>-P<rsub|><around*|(|<frac|x|6>;3|)>+S<rsub|><around*|(|<frac|x|6>;6|)>|)>>>|<row|<cell|>|<cell|+>|<cell|S<rsub|><around*|(|x;6\<cdot\>2|)>+S<rsub|><around*|(|x;6\<cdot\>3|)>-S<rsub|><around*|(|x;6\<cdot\>6|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|><around*|(|x;p,a,q,b|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/c>\<tau\><around*|(|p<rsup|a>*q<rsup|b>*n|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|b+1|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>|)>-P<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>;p|)>-P<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>;q|)>+S<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>;p,1,q,1|)>|)>*>>|<row|<cell|>|<cell|+>|<cell|S<around*|(|x;p,a+1,q,b|)>+S<around*|(|x;p,a,q,b+1|)>-S<around*|(|x;p,a+1,q,b+1|)><space|1em><around*|(|verified|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|b+1|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>|)>-2*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b>>|)>+T<around*|(|<frac|x|p<rsup|a+2>*q<rsup|b>>|)>-2*T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+1>>|)>+T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+2>>|)>+S<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>;p,1,q,1|)>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|a+2|)>*<around*|(|b+1|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b>>|)>-2*T<around*|(|<frac|x|p<rsup|a+2>*q<rsup|b>>|)>+T<around*|(|<frac|x|p<rsup|a+3>*q<rsup|b>>|)>-2*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+1>>|)>+T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+2>>|)>+S<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b>>;p,1,q,1|)>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|a+1|)>*<around*|(|b+2|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+1>>|)>-2*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+1>>|)>+T<around*|(|<frac|x|p<rsup|a+2>*q<rsup|b+1>>|)>-2*T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+2>>|)>+T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+3>>|)>+S<around*|(|<frac|x|p<rsup|a>*q<rsup|b+1>>;p,1,q,1|)>|)>>>|<row|<cell|>|<cell|->|<cell|<around*|(|a+2|)>*<around*|(|b+2|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+1>>|)>-2*T<around*|(|<frac|x|p<rsup|a+2>*q<rsup|b+1>>|)>+T<around*|(|<frac|x|p<rsup|a+3>*q<rsup|b+1>>|)>-2*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+2>>|)>+T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+3>>|)>+S<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+1>>;p,1,q,1|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|b+1|)>*T<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|-2*<around*|(|a+1|)>*<around*|(|b+1|)>+<around*|(|a+2|)>*<around*|(|b+1|)>|)>*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|<around*|(|a+2|)>*<around*|(|b+1|)>-2*<around*|(|a+2|)>*<around*|(|b+1|)>|)>*T<around*|(|<frac|x|p<rsup|a+2>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|<around*|(|a+2|)>*<around*|(|b+1|)>|)>*T<around*|(|<frac|x|p<rsup|a+3>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|+>|<cell|\<ldots\>>>>>
  </eqnarray*>

  All terms containing exponents with <math|2> or more added cancel and so

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|><around*|(|x;p,a,q,b|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/<around*|(|p<rsup|a>*q<rsup|b>|)>>\<tau\><around*|(|p<rsup|a>*q<rsup|b>*n|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|b+1|)>*T<around*|(|<frac|x|p<rsup|a>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|->|<cell|a*<around*|(|b+1|)>*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b>>|)>>>|<row|<cell|>|<cell|->|<cell|<around*|(|a+1|)>*b*T<around*|(|<frac|x|p<rsup|a>*q<rsup|b+1>>|)>>>|<row|<cell|>|<cell|+>|<cell|a*b*T<around*|(|<frac|x|p<rsup|a+1>*q<rsup|b+1>>|)><with|font-series|bold|><space|1em><around*|(|verified|)>>>>>
  </eqnarray*>

  Generalizing

  <\eqnarray*>
    <tformat|<cwith|1|1|1|-1|cell-background|pastel
    cyan>|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/c<rsup|k>>\<tau\><around*|(|c<rsup|k>*n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||c|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|c<rsup|k>|d>|)>T<around*|(|<frac|x|c<rsup|k>*d>|)>>>>>
  </eqnarray*>

  For squarefree factors

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|x;p,1,q,1|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/<around*|(|p<rsup|>*q<rsup|>|)>>\<tau\><around*|(|p<rsup|>*q<rsup|>*n|)>>>|<row|<cell|>|<cell|=>|<cell|4*T<around*|(|<frac|x|p*q>|)>-2*T<around*|(|<frac|x|p<rsup|2>*q>|)>-2*T<around*|(|<frac|x|p*q<rsup|2>>|)>+T<around*|(|<frac|x|p<rsup|2>*q<rsup|2>>|)><rsub|>>>>>
  </eqnarray*>

  Generalizing

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/c>\<tau\><around*|(|c*n|)>>|<cell|=>|<cell|<big|sum><rsub|d<around*|\||c|\<nobracket\>>>\<mu\><around*|(|d|)>*2<rsup|\<omega\><around*|(|c|)>-\<omega\><around*|(|d|)>>*T<around*|(|<frac|x|c*d>|)><space|1em><around*|(|c
    squarefree|)>>>>>
  </eqnarray*>

  for example

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/6>\<tau\><around*|(|6*n|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x,6<around*|\||n|\<nobracket\>>>\<tau\><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|4*T<around*|(|<frac|n|6>|)>-2*T<around*|(|<frac|n|12>|)>-2*T<around*|(|<frac|n|18>|)>+T<around*|(|<frac|n|36>|)>>>>>
  </eqnarray*>

  Because all terms other than the first are multiples of <math|a> or
  <math|b> and all but the last are multiples of <math|a+1> or <math|b+1>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/c<rsup|k>>\<tau\><around*|(|c<rsup|k>*n|)>>|<cell|\<equiv\>>|<cell|\<tau\><around*|(|c<rsup|k>|)>*T<around*|(|<frac|x|c<rsup|k>>|)><space|1em><around*|(|mod
    k|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<around*|(|-1|)><rsup|\<omega\><around*|(|c|)>>*\<tau\><around*|(|c<rsup|k-1>|)>*T<around*|(|<frac|x|c<rsup|k+1>>|)><space|1em><around*|(|mod
    k+1|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|\<tau\><around*|(|c<rsup|k>|)>*T<around*|(|<frac|x|c<rsup|k>>|)>-<big|sum><rsub|p<around*|\||c|\<nobracket\>>>\<tau\><around*|(|<frac|c<rsup|k>|p>|)>*T<around*|(|<frac|x|c<rsup|k>*p>|)><space|1em><around*|(|mod
    k<rsup|2>|)>>>>>
  </eqnarray*>

  but

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<around*|(|x|)>>|<cell|\<equiv\>>|<cell|<around*|\<lfloor\>|<sqrt|x>|\<rfloor\>><space|1em><around*|(|mod
    2|)>>>>>
  </eqnarray*>

  and

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<tau\><around*|(|<frac|c<rsup|2>|p>|)>>|<cell|\<equiv\>>|<cell|0<space|1em><around*|(|mod
    2|)>,for p prime,p<around*|\||c|\<nobracket\>>>>>>
  </eqnarray*>

  so

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x/c<rsup|2>>\<tau\><around*|(|c<rsup|2>*n|)>>|<cell|\<equiv\>>|<cell|\<tau\><around*|(|c<rsup|2>|)>*T<around*|(|<frac|x|c<rsup|2>>|)>-<big|sum><rsub|p<around*|\||c|\<nobracket\>>>\<tau\><around*|(|<frac|c<rsup|2>|p>|)>*<around*|\<lfloor\>|<sqrt|<frac|x|c<rsup|k>*p>>|\<rfloor\>><space|1em><around*|(|mod
    4|)>>>>>
  </eqnarray*>

  which is computable in <math|O<around*|(|<around*|(|<frac|x|c<rsup|2>>|)><rsup|1/3>+log
  x|)>=O<around*|(|<around*|(|<frac|x|c<rsup|2>>|)><rsup|1/3>|)>> time.

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|x;p,a|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x/c>\<tau\><around*|(|p<rsup|a>*n|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a>>|)>-P<around*|(|<frac|x|p<rsup|a>>;p|)>|)>>>|<row|<cell|>|<cell|+>|<cell|S<around*|(|x;p,a+1|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*<around*|(|T<around*|(|<frac|x|p<rsup|a>>|)>-2*T<around*|(|<frac|x|p<rsup|a+1>>|)>+T<around*|(|<frac|x|p<rsup|a+2>>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|a+1|)>*T<around*|(|<frac|x|p<rsup|a>>|)>-a*T<around*|(|<frac|x|p<rsup|a+1>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<around*|(|n|)>=<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|3>>|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|2>|<cell|if n=p>>|<row|<cell|3>|<cell|if
    n=p<rsup|2>>>|<row|<cell|4-1=3>|<cell|if
    n=p<rsup|3>>>|<row|<cell|5-2=3>|<cell|if
    n=p<rsup|4>>>>>>>>|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+2*x+3*x<rsup|2>+3*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+2*x+3*x<rsup|2>*<around*|(|1+x<rsup|2>+x<rsup|3>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|1-x|)>*<around*|(|1+2*x|)>+3*x<rsup|2>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+2*x-x-2*x<rsup|2>+3*x<rsup|2>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+x+x<rsup|2>|1-x>>>|<row|<cell|>|<cell|>|<cell|>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+3*x+3\<cdot\>4*x<rsup|2>+3\<cdot\>9*x<rsup|3>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+3*x*<around*|(|1+4*x+9*x<rsup|2>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|1+3*x*<frac|1+x|<around*|(|1-x|)><rsup|3>>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|1-x|)>*<around*|(|1-2*x+x<rsup|2>|)>+3*x+3*x<rsup|2>|<around*|(|1-x|)><rsup|3>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1-2*x+x<rsup|2>-x+2*x<rsup|2>-x<rsup|3>+3*x+3*x<rsup|2>|<around*|(|1-x|)><rsup|3>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+6*x-x<rsup|3>|<around*|(|1-x|)><rsup|3>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|p><around*|(|x|)>>|<cell|=>|<cell|1+3*x+9*x<rsup|2>*+9x<rsup|3>+9*x<rsup|4>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1+3*x+9*x<rsup|2>*<around*|(|1+x+x<rsup|2>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|1-x|)>+3*x*<around*|(|1-x|)>+9*x<rsup|2>|1-x>>>|<row|<cell|>|<cell|=>|<cell|<frac|1+2*x+12*x<rsup|2>|1-x>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|d<rsup|3><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|><around*|(|<around*|(|<frac|n|d<rsup|3>>|)><rsup|2>|)>>>|<row|<cell|f<rsub|p><around*|(|n|)>>|<cell|=>|<cell|<choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|3>|<cell|if n=p >>|<row|<cell|5>|<cell|if
    n=p<rsup|2>>>|<row|<cell|7-1=6>|<cell|if
    n=p<rsup|3>>>|<row|<cell|9-3=6>|<cell|if n=p<rsup|4>>>>>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<zeta\><around*|(|s|)>>|<cell|=>|<cell|<big|sum><rsub|n=1><rsup|\<infty\>><frac|1|n<rsup|s>>>>|<row|<cell|\<zeta\><around*|(|2*s|)>>|<cell|=>|<cell|<big|sum><rsub|n=1><rsup|\<infty\>><frac|1|n<rsup|2*s>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<frac|1|\<zeta\><around*|(|s|)>>>|<cell|=>|<cell|<big|prod><rsub|p><around*|(|1-<frac|1|p<rsup|s>>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|1-<frac|1|2<rsup|s>>|)>*<around*|(|1-<frac|1|3<rsup|s>>|)>*<around*|(|1-<frac|1|5<rsup|s>>|)>*\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|1-<frac|1|2<rsup|s>>-<frac|1|3<rsup|s>>+<frac|1|<around*|(|2\<cdot\>3|)><rsup|s>>|)>*<around*|(|1-<frac|1|5<rsup|s>>|)>*\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|1-<frac|1|2<rsup|s>>-<frac|1|3<rsup|s>>+<frac|1|<around*|(|2\<cdot\>3|)><rsup|s>>-<frac|1|5<rsup|s>>+<frac|1|<around*|(|2\<cdot\>5|)><rsup|s>>+<frac|1|<around*|(|3\<cdot\>5|)><rsup|s>>-<frac|1|<around*|(|2\<cdot\>3\<cdot\>5|)><rsup|s>>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n=1><rsup|\<infty\>><frac|\<mu\><around*|(|n|)>|n<rsup|s>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n=1><rsup|\<infty\>><frac|<around*|(|-1|)><rsup|n-1>|n<rsup|2>>>|<cell|=>|<cell|<frac|1|1<rsup|2>>-<frac|1|2<rsup|2>>+<frac|1|3<rsup|3>>-<frac|1|4<rsup|2>>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|<frac|1|1<rsup|2>>+<frac|1|2<rsup|2>>+<frac|1|3<rsup|2>>+<frac|1|4<rsup|2>>+\<ldots\>>>|<row|<cell|>|<cell|->|<cell|2*<around*|(|<frac|1|2<rsup|2>>+<frac|1|4<rsup|2>>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|\<zeta\><around*|(|2|)>-2*<around*|(|<frac|1|1<rsup|2>*2<rsup|2>>+<frac|1|2<rsup|2>*2<rsup|2>>+<frac|1|3<rsup|2>*2<rsup|2>>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|\<zeta\>*<around*|(|*2|)>-<frac|1|2>*<around*|(|<frac|1|1<rsup|2>>+<frac|1|2<rsup|2>>+<frac|1|3<rsup|2>>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|\<zeta\><around*|(|2|)>-<frac|1|2>*\<zeta\><around*|(|2|)>>>|<row|<cell|>|<cell|=>|<cell|<frac|1|2>*\<zeta\><around*|(|2|)>*>>>>
  </eqnarray*>

  Proof of Linnik's Mobius identity.

  <\eqnarray*>
    <tformat|<table|<row|<cell|t<rsub|j><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|k=0><rsup|j><around*|(|-1|)><rsup|j-k>*<binom|j|k>*\<tau\><rsub|k><around*|(|n|)>>>|<row|<cell|\<zeta\><around*|(|s|)><rsup|j>>|<cell|=>|<cell|<big|sum><rsup|\<infty\>><rsub|n=1><frac|\<tau\><rsub|j><rsup|><around*|(|n|)>|n<rsup|s>>>>|<row|<cell|<around*|(|\<zeta\><around*|(|s|)>-1|)><rsup|j>>|<cell|=>|<cell|<big|sum><rsub|k=0><rsup|j><around*|(|-1|)><rsup|j-k>*<binom|j|k>*\<zeta\><around*|(|s|)><rsup|k>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|k=0><rsup|j><around*|(|-1|)><rsup|j-k>*<binom|j|k>*<big|sum><rsup|\<infty\>><rsub|n=1><frac|\<tau\><rsub|k><around*|(|n|)>|n<rsup|s>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|\<infty\>><rsub|n=1><frac|t<rsub|j><around*|(|n|)>|n<rsup|s>>>>|<row|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*<big|sum><rsup|\<infty\>><rsub|n=1><frac|t<rsub|j><around*|(|n|)>|n<rsup|s>>>|<cell|=>|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*<around*|(|\<zeta\><around*|(|s|)>-1|)><rsup|j>>>|<row|<cell|<big|sum><rsup|\<infty\>><rsub|n=1><frac|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|n|)>|n<rsup|s>>>|<cell|=>|<cell|<frac|1|1+<around*|(|\<zeta\><around*|(|s|)>-1|)>>>>|<row|<cell|>|<cell|=>|<cell|<frac|1|\<zeta\><around*|(|s|)>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|n=1><rsup|\<infty\>><frac|\<mu\><around*|(|n|)>|n<rsup|s>>>>|<row|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|n|)>>|<cell|=>|<cell|\<mu\><around*|(|n|)>,n\<geq\>1>>>>
  </eqnarray*>

  Mess around with that identity.

  <\eqnarray*>
    <tformat|<table|<row|<cell|t<rsub|0><around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|0><around*|(|n|)>>>|<row|<cell|t<rsub|1><around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|1><around*|(|n|)>-\<tau\><rsub|0><around*|(|n|)>>>|<row|<cell|t<rsub|2><around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|2><around*|(|n|)>-2*\<tau\><rsub|1><around*|(|n|)>+\<tau\><rsub|0><around*|(|n|)>>>|<row|<cell|t<rsub|3><around*|(|n|)>>|<cell|=>|<cell|\<tau\><rsub|3><around*|(|n|)>-3*\<tau\><rsub|2><around*|(|n|)>+3*\<tau\><rsub|1><around*|(|n|)>-\<tau\><rsub|0><around*|(|n|)>>>|<row|<cell|t<rsub|0><around*|(|n|)>-t<rsub|1><around*|(|n|)>+t<rsub|2><around*|(|n|)>-t<rsub|3><around*|(|n|)>>|<cell|=>|<cell|4*\<tau\><rsub|0><around*|(|n|)>-6*\<tau\><rsub|1><around*|(|n|)>+4*\<tau\><rsub|2><around*|(|n|)>-\<tau\><rsub|3><around*|(|n|)>>>|<row|<cell|<big|sum><rsub|j=0><rsup|k><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|k><rsub|j=0>*<around*|(|-1|)><rsup|j>*\<tau\><rsub|j><around*|(|n|)>*<big|sum><rsup|k-j><rsub|i=0><binom|k-i|j>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|k><rsub|j=0>*<around*|(|-1|)><rsup|j>*<binom|k+1|j+1>*\<tau\><rsub|j><around*|(|n|)>>>|<row|<cell|t<rsub|0><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|0>>|<row|<cell|t<rsub|1><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<binom|a|0>-<binom|a-1|-1>>>|<row|<cell|>|<cell|=>|<cell|1-0>>|<row|<cell|>|<cell|=>|<cell|1>>|<row|<cell|t<rsub|2><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<binom|a+1|1>-2*<binom|a|0>-<binom|a-1|-1>>>|<row|<cell|>|<cell|=>|<cell|a-1>>|<row|<cell|t<rsub|3><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<binom|a+2|2>-3*<binom|a+1|1>+3*<binom|a|0>-<binom|a-1|-1>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|a+1|)>*<around*|(|a+2|)>|2>-3*<around*|(|a+1|)>+3-0>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|a-1|)>*<around*|(|a-2|)>|2>>>|<row|<cell|t<rsub|4><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<frac|<around*|(|a-1|)>*<around*|(|a-2|)>*<around*|(|a-3|)>|6>>>|<row|<cell|t<rsub|k><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<binom|a-1|k-1>>>|<row|<cell|\<tau\><rsub|k><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|<binom|a+k-1|k-1>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*T<rprime|'><rsub|j><around*|(|n|)>>|<cell|=>|<cell|M<around*|(|n|)>>>|<row|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*<big|sum><rsub|d\<leq\>n>T<rprime|'><rsub|j><around*|(|<frac|n|d>|)>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>n>M<around*|(|<frac|n|d>|)>>>|<row|<cell|>|<cell|=>|<cell|1>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*T<rprime|'><rsub|j><around*|(|n|)>+<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*<big|sum><rsub|2\<leq\>d\<leq\>n>T<rprime|'><rsub|j><around*|(|<frac|n|d>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*T<rprime|'><rsub|j><around*|(|n|)>+<big|sum><rsub|j=0><rsup|\<infty\>><around*|(|-1|)><rsup|j>*T<rprime|'><rsub|j+1><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|T<rprime|'><rsub|0><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<around*|(|x|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x>\<tau\><around*|(|n|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsup|<around*|(|<wide|2|\<bar\>>|)>><around*|(|x|)>+T<rsup|<around*|(|2|)>><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsup|<around*|(|<wide|3|\<bar\>>|)>><around*|(|x|)>+T<rsup|<around*|(|3|)>><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsup|<around*|(|<wide|3!|\<bar\>>|)>><around*|(|x|)>+T<rsup|<around*|(|2|)>><around*|(|x|)>+T<rsup|<around*|(|3|)>><around*|(|x|)>-T<rsup|<around*|(|6|)>><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|T<rsup|<around*|(|<wide|3!|\<bar\>>|)>><around*|(|x|)>+2*T<around*|(|<frac|n|2>|)>-T<around*|(|<frac|n|4>|)>+2*T<around*|(|<frac|n|3>|)>-T<around*|(|<frac|n|9>|)>>>|<row|<cell|>|<cell|+>|<cell|4*T<around*|(|<frac|n|6>|)>-2*T<around*|(|<frac|n|12>|)>-2*T<around*|(|<frac|n|18>|)>+T<around*|(|<frac|n|36>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|2>>|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x><big|sum><rsub|d<rsup|2>a=n>\<mu\><around*|(|d|)>\<tau\><around*|(|a|)>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsup|2>*a\<leq\>x>\<mu\><around*|(|d|)>*\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>>\<tau\><around*|(|a|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x,n
    even><big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|2>>|)>>|<cell|=>|<cell|<big|sum><rsub|n\<leq\>x,n
    even><big|sum><rsub|d<rsup|2>a=n>\<mu\><around*|(|d|)>\<tau\><around*|(|a|)>*>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>,d
    even>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>>\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>,d
    odd>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>,a
    even>\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>,a
    even>\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>,d
    even>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>,a
    odd>\<tau\><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>>\<mu\><around*|(|d|)>*<around*|(|2*T<around*|(|<frac|x|2*d<rsup|2>>|)>-T<around*|(|<frac|x|4*d<rsup|2>>|)>|)>>>|<row|<cell|>|<cell|->|<cell|<big|sum><rsub|d\<leq\><sqrt|x>,d
    mod 4=2>\<mu\><around*|(|<frac|d-1|2>|)><around*|(|T<around*|(|<frac|x|d<rsup|2>>|)>-2*T<around*|(|<frac|x|2*d<rsup|2>>|)>+T<around*|(|<frac|x|4*d<rsup|2>>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|2<rsup|\<omega\><around*|(|2|)>>+2<rsup|\<omega\><around*|(|4|)>>+2<rsup|\<omega\><around*|(|6|)>>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|(|2<rsup|\<omega\><around*|(|2/2|)>>+2<rsup|\<omega\><around*|(|6/2|)>>+2<rsup|\<omega\><around*|(|10/2|)>>+\<ldots\>|)>+<around*|(|2<rsup|\<omega\><around*|(|4|)>>+2<rsup|\<omega\><around*|(|8|)>>+2<rsup|\<omega\><around*|(|12|)>>+\<ldots\>|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|2*<around*|\<lfloor\>|log<rsub|2>
    x|\<rfloor\>><space|1em><around*|(|mod
    4|)>>>|<row|<cell|<big|sum><rsub|n\<leq\>x,n
    odd><big|sum><rsub|d<rsup|2><around*|\||n|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><around*|(|<frac|n|d<rsup|2>>|)>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|x>,d
    odd>\<mu\><around*|(|d|)>*<big|sum><rsub|a\<leq\>x/d<rsup|2>,a
    odd>\<tau\><around*|(|a|)>>>>>
  </eqnarray*>

  \;

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<alpha\><around*|(|x|)>>|<cell|=>|<cell|x<rsup|2>>>|<row|<cell|\<alpha\><around*|(|x+1|)>>|<cell|=>|<cell|<around*|(|x+1|)><rsup|2>>>|<row|<cell|\<alpha\><around*|(|x|)>-\<alpha\><around*|(|x+1|)>>|<cell|=>|<cell|x<rsup|2>-<around*|(|x+1|)><rsup|2>=x<rsup|2>-<around*|(|x<rsup|2>+2*x+1|)>=-2*x-1>>|<row|<cell|\<beta\><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|n/x<rsup|2>|\<rfloor\>>>>|<row|<cell|\<delta\><rsub|1><around*|(|x|)>>|<cell|=>|<cell|\<beta\><around*|(|x|)>-\<beta\><around*|(|x+1|)>>>|<row|<cell|\<delta\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|1><around*|(|x+1|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>>|<cell|=>|<cell|n-x<rsup|2>*\<beta\><around*|(|x|)>>>|<row|<cell|\<varepsilon\><around*|(|x+1|)>>|<cell|=>|<cell|n-<around*|(|x+1|)><rsup|2>*\<beta\><around*|(|x+1|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>-\<varepsilon\><around*|(|x+1|)>>|<cell|=>|<cell|<around*|(|x+1|)><rsup|2>*\<beta\><around*|(|x+1|)>-x<rsup|2>*\<beta\><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|x+1|)><rsup|2>*\<beta\><around*|(|x+1|)>-x<rsup|2>*<around*|(|\<beta\><around*|(|x+1|)>+\<delta\><rsub|1><around*|(|x+1|)>+\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|x<rsup|2>+2*x+1|)>*\<beta\><around*|(|x+1|)>-x<rsup|2>*<around*|(|\<beta\><around*|(|x+1|)>+\<delta\><rsub|1><around*|(|x+1|)>+\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|2*x+1|)>*\<beta\><around*|(|x+1|)>-x<rsup|2>*\<delta\><rsub|1><around*|(|x+1|)>-x<rsup|2>*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x|)>>|<cell|=>|<cell|<around*|(|2*x-1|)>*\<beta\><around*|(|x|)>-<around*|(|x-1|)><rsup|2>*\<delta\><rsub|1><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x+1|)>>|<cell|=>|<cell|<around*|(|2*x+1|)>*\<beta\><around*|(|x+1|)>-x<rsup|2>*\<delta\><rsub|1><around*|(|x+1|)>>>|<row|<cell|\<gamma\><around*|(|x|)>-\<gamma\><around*|(|x+1|)>>|<cell|=>|<cell|<around*|(|2*x-1|)>*\<beta\><around*|(|x|)>-<around*|(|2*x+1|)>*\<beta\><around*|(|x+1|)>-<around*|(|x-1|)><rsup|2>*\<delta\><rsub|1><around*|(|x|)>+x<rsup|2>*\<delta\><rsub|1><around*|(|x+1|)>>>|<row|<cell|>|<cell|=>|<cell|2*x*<around*|(|\<beta\><around*|(|x|)>-\<beta\><around*|(|x+1|)>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|\<beta\><around*|(|x+1|)>-\<beta\><around*|(|x|)>|)>-<around*|(|x-1|)><rsup|2>*\<delta\><rsub|1><around*|(|x|)>+x<rsup|2>*<around*|(|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|2*x*\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|1><around*|(|x|)>-<around*|(|x-1|)><rsup|2>*\<delta\><rsub|1><around*|(|x|)>+x<rsup|2>*<around*|(|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|2*x-1|)>*\<delta\><rsub|1><around*|(|x|)>-<around*|(|x-1|)><rsup|2>*\<delta\><rsub|1><around*|(|x|)>+x<rsup|2>*<around*|(|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|2*x-1|)>*\<delta\><rsub|1><around*|(|x|)>-<around*|(|x<rsup|2>-2*x+1|)>*\<delta\><rsub|1><around*|(|x|)>+x<rsup|2>*\<delta\><rsub|1><around*|(|x|)>-x<rsup|2>*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|4*x-2|)>*\<delta\><rsub|1><around*|(|x|)>-x<rsup|2>*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<alpha\><around*|(|x|)>>|<cell|=>|<cell|\<alpha\><around*|(|x+1|)>-2*x-1>>|<row|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>>|<cell|=>|<cell|\<varepsilon\><around*|(|x+1|)>+\<gamma\><around*|(|x+1|)>>>|<row|<cell|\<delta\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|<wide|\<varepsilon\>|^><around*|(|x|)>|\<alpha\><around*|(|x|)>>|\<rfloor\>>>>|<row|<cell|\<delta\><rsub|1><around*|(|x|)>>|<cell|=>|<cell|\<delta\><rsub|1><around*|(|x+1|)>+\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>>|<cell|=>|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>-\<alpha\><around*|(|x|)>*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x|)>>|<cell|=>|<cell|\<gamma\><around*|(|x+1|)>+<around*|(|4*x-2|)>*\<delta\><rsub|1><around*|(|x|)>-\<alpha\><around*|(|x|)>*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<beta\><around*|(|x|)>>|<cell|=>|<cell|\<beta\><around*|(|x+1|)>+\<delta\><rsub|1><around*|(|x|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\><sqrt|n>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|n|d<rsup|2>>|\<rfloor\>>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>d<rsub|1>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|n|d<rsup|2>>|\<rfloor\>>+<big|sum><rsub|i\<less\>n/d<rsub|1><rsup|2>>i*<around*|(|M<around*|(|<sqrt|<frac|n|i>>|)>-M<around*|(|<sqrt|<frac|n|i+1>>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>d<rsub|1>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|n|d<rsup|2>>|\<rfloor\>>+<big|sum><rsub|i\<less\>n/d<rsub|1><rsup|2>-1>M<around*|(|<sqrt|<frac|n|i>>|)>-<around*|\<lfloor\>|<frac|n|d<rsub|1><rsup|2>>-1|\<rfloor\>>M<around*|(|<sqrt|<frac|n|i+1>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>d<rsub|1>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|n|d<rsup|2>>|\<rfloor\>>+<big|sum><rsub|i\<less\>n/d<rsub|1><rsup|2>-1><around*|(|1-<big|sum><rsub|d\<geq\>2>M<around*|(|<frac|<sqrt|<frac|n|i>>|d>|)>|)>-<around*|\<lfloor\>|<frac|n|d<rsub|1><rsup|2>>-1|\<rfloor\>>M<around*|(|<sqrt|<frac|n|i+1>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>d<rsub|1>>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|n|d<rsup|2>>|\<rfloor\>>+n/d<rsub|1><rsup|2>-2-<big|sum><rsub|i\<less\>n/d<rsub|1><rsup|2>-1><big|sum><rsub|d\<geq\>2>M<around*|(|<sqrt|<frac|n|d<rsup|2>*i>>|)>-<around*|\<lfloor\>|<frac|n|d<rsub|1><rsup|2>>-1|\<rfloor\>>M<around*|(|<sqrt|<frac|n|i+1>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|i\<less\>I><big|sum><rsub|2\<leq\>d\<leq\><sqrt|n/i>>M<around*|(|<sqrt|<frac|n|d<rsup|2>*i>>|)>>|<cell|=>|<cell|M<around*|(|<sqrt|<frac|n|2<rsup|2>>>|)>+M<around*|(|<sqrt|<frac|n|3<rsup|2>>>|)>+\<ldots\>+M<around*|(|<sqrt|<frac|n|<sqrt|n><rsup|2>>>|)>>>|<row|<cell|>|<cell|+>|<cell|M<around*|(|<sqrt|<frac|n|2*2<rsup|2>>>|)>+M<around*|(|<sqrt|<frac|n|2*3<rsup|2>>>|)>+\<ldots\>+M<around*|(|<sqrt|<frac|n|2*<sqrt|n/2><rsup|2>>>|)>>>|<row|<cell|>|<cell|>|<cell|>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|x\<leq\><sqrt|n>>\<mu\><around*|(|x|)>*T<around*|(|<frac|n|x<rsup|2>>|)>>|<cell|=>|<cell|<big|sum><rsub|x\<leq\>n<rsup|2/7>>\<mu\><around*|(|x|)>*T<around*|(|<frac|n|x<rsup|2>>|)>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|n<rsup|2/7>\<less\>x\<leq\>n<rsup|2/5>>\<mu\><around*|(|x|)>*<around*|[|T<around*|(|<frac|n|<around*|(|x-1|)><rsup|2>>|)>-<big|sum><rsub|n/x<rsup|2>\<leq\>u\<less\>n/<around*|(|x-1|)><rsup|2>>\<tau\><around*|(|u|)>|]>>>|<row|<cell|>|<cell|+>|<cell|<big|sum><rsub|k\<leq\>n<rsup|1/5>>T<around*|(|k|)>*<around*|(|M<around*|(|<sqrt|<frac|n|k>>|)>-M<around*|(|<sqrt|<frac|n|k+1>>|)>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|k\<leq\>n<rsup|1/5>>T<around*|(|k|)>*<around*|(|M<around*|(|<sqrt|<frac|n|k>>|)>-M<around*|(|<sqrt|<frac|n|k+1>>|)>|)>>|<cell|=>|<cell|T<around*|(|1|)>*M<around*|(|<sqrt|n>|)>>>|<row|<cell|>|<cell|->|<cell|T<around*|(|1|)>*M<around*|(|<sqrt|<frac|n|2>>|)>+T<around*|(|2|)>*M<around*|(|<sqrt|<frac|n|2>>|)>>>|<row|<cell|>|<cell|->|<cell|\<ldots\>>>|<row|<cell|>|<cell|->|<cell|T<around*|(|n<rsup|1/5>|)>*M<around*|(|n<rsup|2/5>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|<sqrt|n>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|(|T<around*|(|2|)>-T<around*|(|1|)>|)>*M<around*|(|<sqrt|<frac|n|2>>|)>>>|<row|<cell|>|<cell|+>|<cell|\<ldots\>>>|<row|<cell|>|<cell|->|<cell|T<around*|(|n<rsup|1/5>|)>*M<around*|(|n<rsup|2/5>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|<sqrt|n>|)>+\<tau\><around*|(|2|)>*M<around*|(|<sqrt|<frac|n|2>>|)>+\<ldots\>-T<around*|(|n<rsup|1/5>|)>*M<around*|(|n<rsup|2/5>|)><with|font-series|bold|>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|k\<less\>n<rsup|1/5>>\<tau\><around*|(|k|)>*M<around*|(|<sqrt|<frac|n|k>>|)>+M<around*|(|<sqrt|n>|)>-T<around*|(|n<rsup|1/5>|)>*M<around*|(|n<rsup|2/5>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|k\<less\>n<rsup|1/5>>\<tau\><around*|(|k|)>*M<rprime|'><around*|(|k|)>+M<rprime|'><around*|(|1|)>-T<around*|(|n<rsup|1/5>|)>*M<around*|(|n<rsup|2/5>|)>>>>>
  </eqnarray*>

  Calculating a function over a sequence of integers with ascending divisors
  in square-root time.

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|j=a><rsup|n>f<around*|(|<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsub|j=a><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>f<around*|(|<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>|)>+<big|sum><rsub|j=1><rsup|<around*|\<lfloor\>|n/<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|\<rfloor\>>-1><around*|(|<around*|\<lfloor\>|<frac|n|j>|\<rfloor\>>-<around*|\<lfloor\>|<frac|n|j+1>|\<rfloor\>>|)>*f<around*|(|j|)>>>>>
  </eqnarray*>

  Identity for Mertens function over odd divisors to save roughly a factor of
  three

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n,m
    odd>M<around*|(|<frac|n|m>|)>>|<cell|=>|<cell|0<space|1em><around*|(|n\<geq\>2|)>>>|<row|<cell|M<around*|(|n|)>>|<cell|=>|<cell|-<big|sum><rsub|3\<leq\>k\<leq\>n,j
    odd>M<around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|=>|<cell|-<big|sum><rsub|3\<leq\>m\<leq\><sqrt|n>,j
    odd>M<around*|(|<frac|n|j>|)>>>|<row|<cell|>|<cell|->|<cell|<big|sum><rsub|1\<leq\>k\<less\><sqrt|n>><around*|(|T<rsub|1,odd><around*|(|<frac|n|k>|)>-T<rsub|1,odd><around*|(|<frac|n|k+1>|)>|)>*M<around*|(|k|)>>>|<row|<cell|T<rsub|1,odd><around*|(|<frac|n|k>|)>-T<rsub|1,odd><around*|(|<frac|n|k+1>|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|\<delta\><rsub|1><around*|(|x|)>+\<beta\><around*|(|x|)>
    mod 2|2>|\<rfloor\>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|M<around*|(|<sqrt|<frac|n|m>>|)>>|<cell|=>|<cell|-<big|sum><rsub|3\<leq\>j\<leq\><sqrt|n/m|4>,j
    odd>M<around*|(|<sqrt|<frac|n|j<rsup|2>*m>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|M<rprime|'><around*|(|k|)>>|<cell|=>|<cell|M<around*|(|<sqrt|<frac|n|k>>|)>=M<around*|(|m|)>>>|<row|<cell|>|<cell|=>|<cell|1-<big|sum><rsub|j\<geq\>2>M<around*|(|<sqrt|<frac|n|j<rsup|2>*k>>|)>>>|<row|<cell|>|<cell|=>|<cell|1-<big|sum><rsub|j\<geq\>2,d>M<rprime|'><around*|(|j<rsup|2>k|)>>>|<row|<cell|>|<cell|=>|<cell|1-<big|sum><rsup|<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>><rsub|j=2>M<rprime|'><around*|(|j<rsup|2>k|)>-<big|sum><rsup|<around*|\<lfloor\>|m/<around*|\<lfloor\>|<sqrt|m>|\<rfloor\>>|\<rfloor\>>-1><rsub|j=1><around*|(|<around*|\<lfloor\>|<frac|m|j>|\<rfloor\>>-<around*|\<lfloor\>|<frac|m|j+1>|\<rfloor\>>|)>*M<around*|(|j|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|\<lfloor\>|<frac|x<rsub|i>|j>|\<rfloor\>>>|<cell|\<leq\>>|<cell|x<rsub|max>\<less\><around*|\<lfloor\>|<frac|x<rsub|i>|<around*|(|j-1|)>>|\<rfloor\>>>>|<row|<cell|<around*|\<lfloor\>|<frac|x<rsub|i>|<around*|(|j+1|)>>|\<rfloor\>>>|<cell|\<leq\>>|<cell|x<rsub|max>\<less\><around*|\<lfloor\>|<frac|x<rsub|i>|j>|\<rfloor\>>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|\<lfloor\>|<frac|x|j>|\<rfloor\>>>|<cell|\<leq\>>|<cell|x<rsub|max>>>|<row|<cell|<frac|x<rsub|>|j>>|<cell|\<less\>>|<cell|x<rsub|max>+1>>|<row|<cell|<frac|x|x<rsub|max>+1>>|<cell|\<less\>>|<cell|j>>|<row|<cell|<around*|\<lfloor\>|<frac|x|x<rsub|max>+1>|\<rfloor\>>>|<cell|\<leq\>>|<cell|j-1>>|<row|<cell|<around*|\<lfloor\>|<frac|x|x<rsub|max>+1>|\<rfloor\>>+1>|<cell|\<leq\>>|<cell|j>>|<row|<cell|FirstDivisorNotAbove<around*|(|x,x<rsub|max>|)>>|<cell|=>|<cell|<around*|\<lfloor\>|x/<around*|(|x<rsub|max>+1|)>|\<rfloor\>>+1>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|\<lfloor\>|<frac|x|j>|\<rfloor\>>>|<cell|\<geq\>>|<cell|x<rsub|min>>>|<row|<cell|<frac|x|j>>|<cell|\<geq\>>|<cell|x<rsub|min>>>|<row|<cell|<frac|x|x<rsub|min>>>|<cell|\<geq\>>|<cell|j>>|<row|<cell|<around*|\<lfloor\>|<frac|x|x<rsub|min>>|\<rfloor\>>>|<cell|\<geq\>>|<cell|j>>|<row|<cell|LastDivisorNotBelow<around*|(|x,x<rsub|min>|)>>|<cell|=>|<cell|<around*|\<lfloor\>|x/x<rsub|min>|\<rfloor\>>>>>>
  </eqnarray*>
</body>

<\initial>
  <\collection>
    <associate|sfactor|4>
  </collection>
</initial>

<\references>
  <\collection>
    <associate|auto-1|<tuple|1|?>>
  </collection>
</references>