<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  By the fundemental theorem of arithmetic any whole number <math|n> can be
  expressed as

  <\equation*>
    n=<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>
  </equation*>

  where <math|\<omega\><around*|(|n|)>> is the number of distinct prime
  factors of <math|n>, <math|p<rsub|i>> is prime, and
  <math|a<rsub|i>\<geq\>1>

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
  prime powers. \ For the prime power <math|p<rsup|a>>

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
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|1<rsup|3>>|)>+\<mu\><around*|(|p|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|p<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a>|)>-1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a-3>|)>>>|<row|<cell|>|<cell|=>|<cell|<binom|a+2|a>-<binom|a-1|a-3>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|a+1|)>*<around*|(|a+2|)>|2>-<frac|<around*|(|a-2|)>*<around*|(|*a-1|)>|2>>>|<row|<cell|>|<cell|=>|<cell|3*a>>>>
  </eqnarray*>

  because <math|\<mu\><around*|(|p|)>=-1> for <math|p> prime and so

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a,a\<geq\>3
  </equation*>

  and combining these results

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
  <math|f<rsub|3><around*|(|m|)>=1> for <math|m=1> and is a multiple of three
  for <math|m\<gtr\>1>.

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
  (mod 3) the prime powers less than or equal to <math|n>, weighted by their
  exponent

  <\equation*>
    F<rsub|3><around*|(|n|)>\<equiv\><big|sum><rsub|p<rsup|a>\<leq\>n>a
    <around*|(|mod 3|)>
  </equation*>

  and so we obtain a recurrence relation for <math|\<pi\><around*|(|n|)> mod
  3> by subtracting the weighted counts of prime powers <math|p<rsup|a>> with
  <math|a\<gtr\>1> leaving only the primes

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=2>a*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  Next by expanding all the <math|\<pi\><around*|(|n|)>> terms on the right
  hand side we can turn this into an closed-form expression for
  <math|\<pi\><around*|(|n|)> mod 3> in terms of <math|F<rsub|3>>. \ All the
  terms in the expansion are expressions of the form
  <math|c*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>
  (because <math|\<pi\><around*|(|n|)>> is eventually zero for <math|n> small
  enough) and so

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|3><around*|(|n|)>=F<rsub|3><around*|(|n|)>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=2>a*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>c<around*|(|a|)>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  Expanding the first two summation levels we obtain

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|3><around*|(|n|)>>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|a\<gtr\>1><around*|[|a*<around*|(|F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>-<big|sum><rsub|b\<gtr\>1>b*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|a*b|)>>|\<rfloor\>>|)>|)>|]>>>|<row|<cell|>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|a\<gtr\>1>a*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>+<big|sum><rsub|d<rsub|1>\<gtr\>1><big|sum><rsub|d<rsub|2>\<gtr\>1>d<rsub|1>*d<rsub|2>*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|d<rsub|1>*d<rsub|2>|)>>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|a\<gtr\>1>a*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>+<big|sum><rsub|d<rsub|2>\<gtr\>1><big|sum><rsub|d<rsub|2>\<gtr\>1>d<rsub|1>*d<rsub|2>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|d<rsub|1>d<rsub|2>|)>>|\<rfloor\>>|)>+\<ldots\>>>>>
  </eqnarray*>

  and so the double summation contributes an additional
  <math|d<rsub|1*>*d<rsub|2>=a> to <math|c<around*|(|a|)>> for each distinct
  pair of divisors <math|d<rsub|1>,d<rsub|2>\<gtr\>1 >of <math|a>.

  Continuing the process and summing by coefficient <math|c<around*|(|a|)>>
  gives

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|-<big|sum><rsub|d<rsub|1>=a><rsup|d<rsub|i>\<gtr\>1>a+<big|sum><rsup|d<rsub|i>\<gtr\>1><rsub|d<rsub|1>*d<rsub|2>=a>a-<big|sum><rsup|d<rsub|i>\<gtr\>1><rsub|d<rsub|1>*d<rsub|2>*d<rsub|3>=a>a+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|a*<around*|(|-<big|sum><rsup|d<rsub|i>\<gtr\>1><rsub|d<rsub|1>=a>1+<big|sum><rsup|d<rsub|i>\<gtr\>1><rsub|d<rsub|1>*d<rsub|2>=a>1-<big|sum><rsup|d<rsub|i>\<gtr\>1><rsub|d<rsub|1>*d<rsub|2>*d<rsub|3>=a>1+\<ldots\>|)>>>>>
  </eqnarray*>

  Introducing the notation <math|t<rsub|j><around*|(|a|)>> for the number of
  ways of writing <math|a> as a product of <math|j> integers strictly greater
  than one (order being distinguished) and using the identity

  <\equation*>
    \<mu\><around*|(|a|)>=<big|sum><rsup|\<infty\>><rsub|j=1><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|a|)>
  </equation*>

  we obtain

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|a*<around*|(|-t<rsub|1><around*|(|a|)>+t<rsub|2><around*|(|a|)>-t<rsub|3><around*|(|a|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|a*<big|sum><rsup|\<infty\>><rsub|j=1><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|a*\<mu\><around*|(|a|)>>>>>
  </eqnarray*>

  Then substituting for <math|c<around*|(|a|)>> in
  <math|\<pi\><rsub|3><around*|(|n|)>> yields the closed-form expression for
  <math|\<pi\><around*|(|n|)> mod 3>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>a*\<mu\><around*|(|a|)>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  noting that the <math|F<rsub|3>> terms where
  <math|3<around*|\||a|\<nobracket\>>> or <math|\<mu\><around*|(|a|)>=0> need
  not be computed.

  \;
</body>

<\initial>
  <\collection>
    <associate|sfactor|4>
  </collection>
</initial>