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
    <tformat|<table|<row|<cell|T<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|n>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>+<big|sum><rsub|z=1><rsup|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1>z*<around*|(|T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>-T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z+1>|\<rfloor\>>|)>|)>>>|<row|<cell|>|<cell|>|<cell|>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|z=1><rsup|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>>\<tau\><rsub|2><around*|(|z|)>*<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>+<big|sum><rsub|z=1><rsup|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1>T<rsub|2><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>|)>-<around*|(|<around*|\<lceil\>|<sqrt|n>|\<rceil\>>-1|)>*T<rsub|2><around*|(|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>>>>>
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
    n=<big|prod><rsup|k><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>,d<rsub|k><around*|(|n|)>=<big|sum><rsup|k><rsub|i=1><binom|a<rsub|i>+k-1|a<rsub|i>>
  </equation*>

  <\equation*>
    <tabular|<tformat|<table|<row|<cell|b,>|<cell|a,>|<cell|c>>>>>
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

  Other stuff:

  <\equation*>
    <binom|n|k>=<frac|n!|k!*<around*|(|n-k|)>!>
  </equation*>

  \;

  <math|\<pi\><around*|(|n|)> mod 3>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|p><around*|(|x|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsup|<around*|\<lfloor\>|x<rsup|1/p>|\<rfloor\>>><rsub|j=1>\<mu\><around*|(|j|)>*T<rsub|p><around*|(|<around*|\<lfloor\>|<frac|x<rsup|>|j<rsup|p>>|\<rfloor\>>|)>-1|)>/p>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=1>\<mu\><around*|(|k|)>*F<rsub|2><around*|(|n<rsup|1/k>|)>
    <around*|(|mod 2|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=1>k*\<mu\><around*|(|k|)>*F<rsub|3><around*|(|n<rsup|1/k>|)>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  \;
</body>

<\initial>
  <\collection>
    <associate|sfactor|4>
  </collection>
</initial>