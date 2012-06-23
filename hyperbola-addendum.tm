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
    from <math|T<rsub|2><around*|(|n/2|)>> and so all terms smaller by a
    power of <math|2> can be calculated together in the same total time as
    for <math|T<rsub|2><around*|(|n|)>> alone.

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

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<around*|(|n;a,b|)>>|<cell|=>|<cell|2*S<around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>;a,b|)>+<big|sum><rsub|a\<leq\>x\<leq\>b,2*<around*|(|n
    mod x|)>\<geq\> x>1>>|<row|<cell|S<around*|(|n;1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>>|<cell|=>|<cell|2*S<around*|(|<around*|\<lfloor\>|<frac|n|2>|\<rfloor\>>|)>;1,<around*|\<nobracket\>|<around*|\<lfloor\>|<sqrt|<frac|n|2>>|\<rfloor\>>|)>+S<around*|(|n;<around*|\<lfloor\>|<sqrt|<frac|n|2>>|\<rfloor\>>+1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>+<big|sum><rsub|x\<leq\><around*|\<lfloor\>|<sqrt|n/2>|\<rfloor\>>,2*<around*|(|n
    mod x|)>\<geq\> x>1>>>>
  </eqnarray*>

  \;
</body>

<\initial>
  <\collection>
    <associate|sfactor|4>
  </collection>
</initial>