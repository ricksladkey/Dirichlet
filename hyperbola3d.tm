<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  By the fundemental theorem of arithmetic any whole number <math|n> can be
  expressed as

  <\equation*>
    n=<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>
  </equation*>

  Calculating <math|\<tau\><rsub|3><around*|(|n|)>>

  <\equation*>
    \<tau\><rsub|3><around*|(|n|)>=<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1><binom|a<rsub|i>+2|a<rsub|i>>
  </equation*>

  Define

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|n|)>>|<cell|\<assign\>>|<cell|<big|sum><rsub|d:d<rsup|3><around*|\||n|\<nobracket\>>><rsub|>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|3>>|)>>>>>
  </eqnarray*>

  Clearly

  <\equation*>
    f<rsub|3><around*|(|1|)>=\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|1|)>=1\<cdot\>1=1
  </equation*>

  For <math|n=n<rsub|1>*n<rsub|2>> with <math|gcd<around*|(|n<rsub|1>,n<rsub|2>|)>=1>,
  any divisor <math|d<rsup|3>> of <math|n> can be expressed uniquely as
  <math|<around*|(|d<rsub|1>*d<rsub|2>|)><rsup|3>> where
  <math|d<rsub|1><around*|\||n<rsub|1>|\<nobracket\>>>,
  <math|d<rsub|2><around*|\||n<rsub|2>|\<nobracket\>>>,
  <math|gcd<around*|(|d<rsub|1>,d<rsub|2>|)>=1> and
  <math|gcd<around*|(|n<rsub|1>/d<rsub|1><rsup|3>,n<rsub|2>/d<rsup|3><rsub|2>|)>=1>
  giving

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|n|)>=f<rsub|3><around*|(|n<rsub|1>*n<rsub|2>|)>>|<cell|=>|<cell|<big|sum><rsub|d<rsup|3><around*|\||n<rsub|1>*n2|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|1>*n<rsub|2>|d<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|<around*|(|d<rsub|1>*d<rsub|2>|)><rsup|3><around*|\||n<rsub|1>n<rsub|2>|\<nobracket\>>>\<mu\><around*|(|d<rsub|1>*d<rsub|2>|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|1>*n<rsub|2>|<around*|(|d<rsub|1>*d<rsub|2>|)><rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d<rsub|1><rsup|3><around*|\||n<rsub|1>,d<rsub|2><rsup|3>|\|>n<rsub|2>>\<mu\><around*|(|d<rsub|1>|)>*\<mu\><around*|(|d<rsub|2>|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|1>*|d<rsub|1><rsup|3>>|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|2>|d<rsub|2><rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|<big|sum><rsub|d<rsub|1><rsup|3><around*|\||n<rsub|1>|\<nobracket\>>>\<mu\><around*|(|d<rsub|1>|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|1>|d<rsub|1><rsup|3>>|)>|)>*<around*|(|<big|sum><rsub|d<rsub|2><rsup|3><around*|\||n<rsub|2>|\<nobracket\>>>\<mu\><around*|(|d<rsub|2>|)>*\<tau\><rsub|3><around*|(|<frac|n<rsub|2>|d<rsub|2><rsup|3>>|)>|)>>>|<row|<cell|>|<cell|=>|<cell|f<rsub|3><around*|(|n<rsub|1>|)>*f<rsub|3><around*|(|n<rsub|2>|)>>>>>
  </eqnarray*>

  since <math|\<mu\><around*|(|n|)>> and <math|\<tau\><rsub|3><around*|(|n|)>>
  are multiplicative and therefore <math|f<rsub|3><around*|(|n|)>> is also
  multiplicative.

  Now let us characterize the behavior of <math|f<rsub|3><around*|(|n|)>> at
  prime powers. \ For prime power <math|p<rsup|a>>

  <\equation*>
    \<tau\><rsub|3><around*|(|p<rsup|<rsup|a>>|)>=<binom|a+2|a>
  </equation*>

  For <math|1\<leq\>a\<less\>3> the only cube <math|p<rsup|a>> is divisible
  by is <math|1<rsup|3>>

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|1<rsup|3>>|)>=1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a>|)>=<binom|a+2|a>,f<rsub|3><around*|(|p|)>=<binom|3|1>=3,f<rsub|3><around*|(|p<rsup|2>|)>=<binom|4|2>=6
  </equation*>

  and so

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a\<nocomma\>,1\<leq\>a\<less\>3
  </equation*>

  For <math|a\<geq\>3> <math|p<rsup|a>> is divisible by the cubes
  <math|1<rsup|3>> and <math|p<rsup|3>> resulting in

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|1<rsup|3>>|)>+\<mu\><around*|(|p|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|p<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a>|)>-1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a-3>|)>>>|<row|<cell|>|<cell|=>|<cell|<binom|a+2|a>-<binom|a-1|a-3>>>|<row|<cell|>|<cell|=>|<cell|3*a>>>>
  </eqnarray*>

  because <math|\<mu\><around*|(|p|)>=-1> for <math|p> prime and so

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a,a\<geq\>3
  </equation*>

  and combining

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a,a\<geq\>1
  </equation*>

  Then using multiplicativity we obtain:

  <\equation*>
    f<rsub|3><around*|(|n|)>=f<rsub|3><around*|(|<big|prod><rsub|i><rsup|\<omega\><around*|(|n|)>>p<rsub|i><rsup|a<rsub|i>>|)>=<big|prod><rsub|i><rsup|\<omega\><around*|(|n|)>>f<rsub|3><around*|(|p<rsub|i><rsup|a<rsub|i>>|)>=<big|prod><rsub|i><rsup|\<omega\><around*|(|n|)>>3*a<rsub|i>=3<rsup|\<omega\><around*|(|n|)>>*<big|prod><rsub|i><rsup|\<omega\><around*|(|n|)>>a<rsub|i>
  </equation*>

  When <math|n> is not unity or a prime power,
  <math|\<omega\><around*|(|n|)>\<gtr\>1> and therefore
  <math|f<rsub|3><around*|(|n|)>> is a multiple of <math|9> and so

  <\equation*>
    f<rsub|3><around*|(|n|)>\<equiv\><choice|<tformat|<table|<row|<cell|1>|<cell|if
    n=1>>|<row|<cell|3*a>|<cell|if n=p<rsup|a>>>|<row|<cell|0>|<cell|otherwise>>>>>
    <around*|(|mod 9|)>
  </equation*>

  Defining

  <\equation*>
    F<rsub|3><around*|(|n|)>\<assign\><around*|(|<big|sum><rsup|n><rsub|m=1>f<rsub|3><around*|(|m|)>-1|)>/3
  </equation*>

  we observe that <math|F<rsub|3><around*|(|n|)>> is a whole number because
  <math|f<rsub|3><around*|(|m|)>=1> for <math|m=1> or a multiple of three
  otherwise.

  Let us rewrite the sum in the definition of <math|F<rsub|3><around*|(|n|)>>
  as

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n>f<rsub|3><around*|(|m|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n><big|sum><rsub|d:d<rsup|3><around*|\||m|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|m|d<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d>\<mu\><around*|(|d|)><big|sum><rsub|m\<leq\>n/d<rsup|3>>\<tau\><rsub|3><around*|(|m|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|n<rsup|1/3>|\<rfloor\>>><rsub|d=1>\<mu\><around*|(|d|)>*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|d<rsup|3>>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  and so

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|3><around*|(|n|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsup|<around*|\<lfloor\>|n<rsup|1/3>|\<rfloor\>>><rsub|d=1>\<mu\><around*|(|d|)>*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|3>>|\<rfloor\>>|)>-1|)>/3>>>>
  </eqnarray*>

  Because of the properties of <math|f<rsub|3><around*|(|n|)>> we can now see
  that <math|F<rsub|3><around*|(|n|)>> is a counting function that counts
  (mod 3) the prime powers less than or equal to <math|n> weighted by their
  exponent

  <\equation*>
    F<rsub|3><around*|(|n|)>\<equiv\><big|sum><rsub|p<rsup|a>\<leq\>n>a
    <around*|(|mod 3|)>
  </equation*>

  and so we obtain a recursive formula for <math|\<pi\><around*|(|n|)> mod 3>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=2>a*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  Expanding all the <math|\<pi\><around*|(|n|)>> terms on the right hand side
  we can turn this into an explicit formula for <math|\<pi\><around*|(|n|)>
  mod 3> in terms of <math|F<rsub|3>>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>a*\<mu\><around*|(|a|)>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  \;
</body>

<\initial>
  <\collection>
    <associate|sfactor|4>
  </collection>
</initial>