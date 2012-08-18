<TeXmacs|1.0.7.15>

<style|generic>

<\body>
  <section|Modular Prime Counting>

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|k><around*|(|n|)>>|<cell|\<assign\>>|<cell|<big|sum><rsub|d:d<rsup|k><around*|\||n|\<nobracket\>>><rsub|>\<mu\><around*|(|d|)>*\<tau\><rsub|k><around*|(|<frac|n|d<rsup|k>>|)>>>>>
  </eqnarray*>

  <\equation*>
    F<rsub|k><around*|(|n|)>\<assign\><around*|(|<big|sum><rsup|n><rsub|m=1>f<rsub|k><around*|(|m|)>-1|)>/k
  </equation*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|k><around*|(|n|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsup|<around*|\<lfloor\>|n<rsup|1/k>|\<rfloor\>>><rsub|d=1>\<mu\><around*|(|d|)>*T<rsub|k><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|k>>|\<rfloor\>>|)>-1|)>/k>>>>
  </eqnarray*>

  <section|Prime Counting Function Modulo 2>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|2><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=1>\<mu\><around*|(|i|)>*F<rsub|2><around*|(|n<rsup|1/k>|)>>>|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|\<pi\><rsub|2><around*|(|n|)><space|1em><around*|(|mod
    2|)>>>>>
  </eqnarray*>

  We can avoid all the even numbers because every multiple of two is either a
  prime power <math|2<rsup|a>> or contributes a multiple of <math|4> to
  <math|\<pi\><rsub|2><around*|(|n|)>>. \ If <math|2> is omitted from each
  term in <math|\<pi\><rsub|2><around*|(|n|)>>, <math|2> will not be counted
  as a prime nor will any powers of two be subtracted away, leaving
  <math|\<pi\><rsub|2>> as a whole too small by one, which can be corrected
  at the end.

  <\eqnarray*>
    <tformat|<table|<row|<cell|S<rsub|odd><around*|(|n;a,b|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsub|x:a\<leq\>x\<leq\>b,x
    odd><around*|(|<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>+<around*|\<lfloor\>|<frac|n|x>|\<rfloor\>>
    mod 2|)>|)>/2>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsup|><rsub|2,odd><around*|(|n|)>=<big|sum><rsub|x:x\<leq\>n,x
    odd>\<tau\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|2*S<rsub|odd><around*|(|n;1,<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>|)>-<around*|(|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n>|\<rfloor\>>+1|2>|\<rfloor\>>|)><rsup|2>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsub|d:d\<leq\><sqrt|n>,d
    odd>\<mu\><around*|(|d|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|2>>|\<rfloor\>>|)>-1|)>/2>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|2,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=1>\<mu\><around*|(|i|)>*F<rsub|2,odd><around*|(|n<rsup|1/k>|)>>>|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|\<pi\><rsub|2,odd><around*|(|n|)>+1<space|1em><around*|(|mod
    2|)>,<space|1em>n\<geq\>2>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|x<rsub|max>\<less\>d\<leq\><sqrt|n>,d
    odd>\<mu\><around*|(|d|)>*T<rsub|2,odd><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|2>>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsub|1\<leq\>i\<leq\>i<rsub|max>>T<rsub|2,odd><around*|(|i|)>*<around*|(|M<rsub|odd><around*|(|<sqrt|<frac|n|i>>|)>-M<rsub|odd><around*|(|<sqrt|<frac|n|i+1>>|)>|)>>>>>
  </eqnarray*>

  Formulas for <math|T<rsub|2,odd><around*|(|n|)>> for successive
  approximation algorithm

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|1,odd><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|x+1|2>|\<rfloor\>>=<frac|x+x
    mod 2|2>>>|<row|<cell|H<around*|(|u,v|)>>|<cell|=>|<cell|<around*|(|2*<around*|(|b<rsub|2>*<around*|(|u+c<rsub|1>|)>-b<rsub|1>*<around*|(|v+c<rsub|2>|)>|)>-1|)>*<around*|(|2*<around*|(|a<rsub|1>*<around*|(|*v+c<rsub|2>|)>-a<rsub|2>*<around*|(|u+c<rsub|1>|)>|)>-1|)>>>|<row|<cell|Y<rsub|tan><rsup|><around*|(|a|)>>|<cell|=>|<cell|T<rsub|1,odd><around*|(|<sqrt|<frac|n|a>>|)>>>|<row|<cell|Y<rsub|floor><around*|(|x|)>>|<cell|=>|<cell|T<rsub|1,odd><around*|(|<frac|n|2*x-1>|)>>>|<row|<cell|U<around*|(|v|)>>|<cell|=>|<cell|<frac|2*<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>|)>*<around*|(|v+c<rsub|2>|)>+a<rsub|2>-b<rsub|2>-<sqrt|<around*|(|2*<around*|(|v+c<rsub|2>|)>-a<rsub|2>-b<rsub|2>|)><rsup|2>-4*a<rsub|2>*b<rsub|2>*n>|4*a<rsub|2>*b<rsub|2>>-c<rsub|1>>>|<row|<cell|V<around*|(|u|)>>|<cell|=>|<cell|<frac|2*<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>|)>*<around*|(|u+c<rsub|1>|)>-a<rsub|1>+b<rsub|1>-<sqrt|<around*|(|2*<around*|(|u+c<rsub|1>|)>-a<rsub|1>-b<rsub|1>|)><rsup|2>-4*a<rsub|1>*b<rsub|1>*n>|4*a<rsub|1>*b<rsub|1>>-c<rsub|2>>>|<row|<cell|U<rsub|tan><around*|(|v|)>>|<cell|=>|<cell|<frac|<around*|(|a<rsub|1>+b<rsub|1>|)>*a<rsub|3>*b<rsub|3>+<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>+2*a<rsub|1>*b<rsub|1>|)>*<sqrt|a<rsub|3>*b<rsub|3>*n>|2*a<rsub|3>*b<rsub|3>>-c<rsub|1>>>|<row|<cell|>|<cell|=>|<cell|<frac|a<rsub|1>+b<rsub|1>+*<sqrt|<around*|(|a<rsub|1>*b<rsub|2>+b<rsub|1>*a<rsub|2>+2*a<rsub|1>*b<rsub|1>|)><rsup|2>*n/<around*|(|a<rsub|3>*b<rsub|3>|)>>|2>-c<rsub|1>>>>>
  </eqnarray*>

  Although counter-intuitive, multiplying, e.g., <math|c<rsub|1>> by
  <math|4*a<rsub|2>*b*<rsub|2>> and subtracting it from the numerator of the
  first term in <math|U<around*|(|v|)>> before dividing will reduce the
  magnitude of the dividend for the division operation and therefore may be
  beneficial.

  <section|Prime Counting Function Modulo 3>

  The normal divisor function <math|\<tau\><around*|(|n|)>> (also known as
  <math|d<around*|(|n|)>>) counts the number of ways that <math|n> can be
  expressed as the ordered product of two integers

  <\equation*>
    \<tau\><rsub|><around*|(|n|)>=<big|sum><rsub|d<rsup|><rsub|1>*d<rsub|2>=n>1=<big|sum><rsub|d<around*|\||n|\<nobracket\>>>1
  </equation*>

  which can be generalized to other numbers of factors and so we can use the
  notation <math|\<tau\><rsub|2><around*|(|n|)>=\<tau\><around*|(|n|)>> and
  define <math|\<tau\><rsub|k><around*|(|n|)>> to be the number of ways that
  <math|n> can be expressed as an ordered product of <math|k> integers.
  \ Then

  <\equation*>
    \<tau\><rsub|3><around*|(|n|)>=<big|sum><rsub|d<rsub|1>*d<rsub|2>*d<rsub|3>=n>1=<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><rsub|2><around*|(|d|)>=<big|sum><rsub|d<around*|\||n|\<nobracket\>>>\<tau\><rsub|2><around*|(|<frac|n|d>|)>
  </equation*>

  and therefore <math|\<tau\><rsub|3><around*|(|n|)>> is multiplicative
  becaue it is the Dirichlet convolution <math|1<around*|(|n|)>\<ast\>\<tau\><rsub|2><around*|(|n|)>>
  of multiplicative functions.

  The values of all <math|\<tau\><rsub|k><around*|(|n|)>> functions are
  determined entirely by the exponents in the prime decomposition of
  <math|n>. By the fundemental theorem of arithmetic any whole number
  <math|n> can be expressed uniquely as

  <\equation*>
    n=<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1>p<rsub|i><rsup|a<rsub|i>>
  </equation*>

  where <math|\<omega\><around*|(|n|)>> is the number of distinct prime
  factors of <math|n>, <math|p<rsub|i>> is prime, and
  <math|a<rsub|i>\<geq\>1>. A prime power <math|p<rsup|a>> has the divisors
  <math|1,p,p<rsup|2>,\<ldots\>,p<rsup|a>> each of which has
  <math|\<tau\><rsub|2><rsub|><around*|(|<frac|n|d>|)>> equal to
  <math|a+1,a,a-1,\<ldots\>,1> and so <math|\<tau\><rsub|3><around*|(|p<rsup|a>|)>>
  is

  <\equation*>
    \<tau\><rsub|3><around*|(|p<rsup|a>|)>=<around*|(|a+1|)>+a+<around*|(|a-1|)>+\<ldots\>+1=<frac|<around*|(|a+1|)>*<around*|(|a+2|)>|2>=<binom|a+2|a>=<binom|a+2|2>
  </equation*>

  and then using multiplicativity

  <\equation*>
    \<tau\><rsub|3><around*|(|n|)>=<big|prod><rsup|\<omega\><around*|(|n|)>><rsub|i=1><binom|a<rsub|i>+2|2>
  </equation*>

  Define

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|n|)>>|<cell|\<assign\>>|<cell|<big|sum><rsub|d:d<rsup|3><around*|\||n|\<nobracket\>>><rsub|>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|n|d<rsup|3>>|)>>>>>
  </eqnarray*>

  We will show that <math|f<rsub|3><around*|(|n|)>> is multiplicative and
  that it has useful properties for computing the prime counting function
  <math|\<pi\><around*|(|n|)> mod 3>.

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
    \<tau\><rsub|3><around*|(|p<rsup|<rsup|a>>|)>=<binom|a+2|2>
  </equation*>

  For <math|1\<leq\>a\<less\>3> the only cube <math|p<rsup|a>> is divisible
  by is <math|1<rsup|3>>

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|1<rsup|3>>|)>=1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a>|)>=<binom|a+2|2>,f<rsub|3><around*|(|p|)>=<binom|3|2>=3,f<rsub|3><around*|(|p<rsup|2>|)>=<binom|4|2>=6
  </equation*>

  and so

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a\<nocomma\><space|1em>for all
    1\<leq\>a\<less\>3
  </equation*>

  For <math|a\<geq\>3> <math|p<rsup|a>> is divisible by the cubes
  <math|1<rsup|3>> and <math|p<rsup|3>> and possibly higher powers
  <math|<around*|(|p<rsup|b>|)><rsup|3>> where <math|b\<gtr\>1> but in those
  cases <math|\<mu\><around*|(|p<rsup|b>|)>=0> because the value being cubed
  is not square-free. \ Therefore

  <\eqnarray*>
    <tformat|<table|<row|<cell|f<rsub|3><around*|(|p<rsup|a>|)>>|<cell|=>|<cell|\<mu\><around*|(|1|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|1<rsup|3>>|)>+\<mu\><around*|(|p|)>*\<tau\><rsub|3><around*|(|<frac|p<rsup|a>|p<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a>|)>-1\<cdot\>\<tau\><rsub|3><around*|(|p<rsup|a-3>|)>>>|<row|<cell|>|<cell|=>|<cell|<binom|a+2|2>-<binom|a-1|2>>>|<row|<cell|>|<cell|=>|<cell|<frac|<around*|(|a+1|)>*<around*|(|a+2|)>|2>-<frac|<around*|(|a-2|)>*<around*|(|*a-1|)>|2>>>|<row|<cell|>|<cell|=>|<cell|3*a>>>>
  </eqnarray*>

  because <math|\<mu\><around*|(|p|)>=-1> for <math|p> prime and so

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a<space|1em>for all a\<geq\>3
  </equation*>

  and combining these results

  <\equation*>
    f<rsub|3><around*|(|p<rsup|a>|)>=3*a<space|1em>for all a\<geq\>1
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
    n=1>>|<row|<cell|3*a>|<cell|if n=p<rsup|a>>>|<row|<cell|0>|<cell|otherwise>>>>><space|1em><around*|(|mod
    9|)>
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
    <tformat|<table|<row|<cell|<big|sum><rsup|n><rsub|m=1>f<rsub|3><around*|(|m|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n><big|sum><rsub|d:d<rsup|3><around*|\||m|\<nobracket\>>>\<mu\><around*|(|d|)>*\<tau\><rsub|3><around*|(|<frac|m|d<rsup|3>>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d>\<mu\><around*|(|d|)><big|sum><rsub|m\<leq\>n/d<rsup|3>>\<tau\><rsub|3><around*|(|m|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|n<rsup|1/3>|\<rfloor\>>><rsub|d=1>\<mu\><around*|(|d|)>*T<rsub|3><around*|(|<around*|\<lfloor\>|<frac|n|d<rsup|3>>|\<rfloor\>>|)>>>>>
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
  <math|C*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>
  (because <math|\<pi\><around*|(|n|)>> is eventually zero for <math|n> small
  enough) and so

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|3><around*|(|n|)>=F<rsub|3><around*|(|n|)>-<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=2>a*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>c<around*|(|a|)>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  for some coefficient function <math|c<around*|(|a|)>>.

  Expanding the first two summation levels we obtain

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|3><around*|(|n|)>>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|d<rsub|1>\<gtr\>1><around*|[|d<rsub|1>*<around*|(|F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/d<rsub|1>>|\<rfloor\>>|)>-<big|sum><rsub|d<rsub|2>\<gtr\>1>d<rsub|2>*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|d<rsub|1>*d<rsub|2>|)>>|\<rfloor\>>|)>|)>|]>>>|<row|<cell|>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|d<rsub|1>\<gtr\>1>d<rsub|1>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/d<rsub|1>>|\<rfloor\>>|)>+<big|sum><rsub|d<rsub|1>\<gtr\>1><big|sum><rsub|d<rsub|2>\<gtr\>1>d<rsub|1>*d<rsub|2>*\<pi\><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|d<rsub|1>*d<rsub|2>|)>>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|=>|<cell|F<rsub|3><around*|(|n|)>-<big|sum><rsub|d<rsub|1>\<gtr\>1>d<rsub|1>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/d<rsub|1>>|\<rfloor\>>|)>+<big|sum><rsub|d<rsub|2>\<gtr\>1><big|sum><rsub|d<rsub|2>\<gtr\>1>d<rsub|1>*d<rsub|2>*F<rsub|3><around*|(|<around*|\<lfloor\>|n<rsup|1/<around*|(|d<rsub|1>d<rsub|2>|)>>|\<rfloor\>>|)>-\<ldots\>>>>>
  </eqnarray*>

  and so the first summation contributes an additional <math|-d<rsub|1>> to
  <math|c<around*|(|d<rsub|1>|)>> for each <math|d<rsub|1>\<gtr\>1> and the
  double summation contributes an additional <math|d<rsub|1*>*d<rsub|2>> to
  <math|c<around*|(|d<rsub|1*>*d<rsub|2>|)>> for each distinct pair of
  divisors <math|d<rsub|1>,d<rsub|2>\<gtr\>1 >of <math|a>.

  Continuing the process and summing like terms by coefficient
  <math|c<around*|(|a|)>> gives

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|<big|sum><rsub|1=a,d<rsub|i>\<gtr\>1>a-<big|sum><rsub|d<rsub|1>=a,d<rsub|i>\<gtr\>1>a+<big|sum><rsub|d<rsub|1>*d<rsub|2>=a,d<rsub|i>\<gtr\>1>a-<big|sum><rsub|d<rsub|1>*d<rsub|2>*d<rsub|3>=a,d<rsub|i>\<gtr\>1>a+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|a*<around*|(|<big|sum><rsub|1=a,d<rsub|i>\<gtr\>1>1-<big|sum><rsub|d<rsub|1>=a,d<rsub|i>\<gtr\>1>1+<big|sum><rsub|d<rsub|1>*d<rsub|2>=a,d<rsub|i>\<gtr\>1>1-<big|sum><rsub|d<rsub|1>*d<rsub|2>*d<rsub|3>=a,d<rsub|i>\<gtr\>1>1+\<ldots\>|)>>>>>
  </eqnarray*>

  Introducing the notation <math|t<rsub|j><around*|(|a|)>> for the number of
  ways of writing <math|a> as a product of <math|j> integers strictly greater
  than one (order being distinguished) and using the identity (due to Linnik)

  <\equation*>
    \<mu\><around*|(|a|)>=<big|sum><rsup|\<infty\>><rsub|j=0><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|a|)>,a\<geq\>1
  </equation*>

  we obtain

  <\eqnarray*>
    <tformat|<table|<row|<cell|c<around*|(|a|)>>|<cell|=>|<cell|a*<around*|(|t<rsub|0><around*|(|a|)>-t<rsub|1><around*|(|a|)>+t<rsub|2><around*|(|a|)>-t<rsub|3><around*|(|a|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|a*<big|sum><rsup|\<infty\>><rsub|j=0><around*|(|-1|)><rsup|j>*t<rsub|j><around*|(|a|)>>>|<row|<cell|>|<cell|=>|<cell|a*\<mu\><around*|(|a|)>>>>>
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

  Then with some simple modifications, we can now apply the odd-only divisor
  method which reduces the number of operations by roughly a factor of four.

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|3*<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|2*S<rsub|odd><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>;z+2,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+1|2>|\<rfloor\>>|)>>>|<row|<cell|>|<cell|+>|<cell|<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|<around*|(|<big|sum><rsub|d\<leq\><sqrt|n|3>,d
    odd>\<mu\><around*|(|d|)>*T<rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|3>>|\<rfloor\>>|)>-1|)>/3>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|a=1>a*\<mu\><around*|(|a|)>*F<rsub|3,odd><around*|(|<around*|\<lfloor\>|n<rsup|1/a>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|\<pi\><rsub|3,odd><around*|(|n|)>+1<space|1em><around*|(|mod
    3|)><space|1em>for all n\<geq\>2>>>>
  </eqnarray*>

  or using the Iverson bracket

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<pi\><around*|(|n|)>>|<cell|\<equiv\>>|<cell|\<pi\><rsub|3,odd><around*|(|n|)>+<around*|[|n\<geq\>2|]><space|1em><around*|(|mod
    3|)>>>>>
  </eqnarray*>

  <section|Simplifying Summation Expressions>

  Although it is not a significant part of the computation, we observe for
  small values of <math|n> that

  <\eqnarray*>
    <tformat|<table|<row|<cell|<around*|(|<big|sum><rsub|d\<leq\><sqrt|n|3>,d
    odd>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|3>>|\<rfloor\>><rsup|1/3>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>-1|)>/3>|<cell|\<equiv\>>|<cell|<choice|<tformat|<cwith|1|-1|1|1|cell-halign|r>|<table|<row|<cell|0>|<cell|if
    n\<less\>27>>|<row|<cell|2>|<cell|if n\<geq\>27>>>>> <around*|(|mod
    3|)>>>>>
  </eqnarray*>

  We now prove that this expression is valid for all <math|n> but first we
  will need some additional machinery before we can prove the final result.

  Characterize <math|M<rsub|odd><around*|(|n|)>>, the sum of the Mobius
  function over odd arguments.

  <\eqnarray*>
    <tformat|<table|<row|<cell|M<rsub|odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|i\<leq\>n,i
    odd>\<mu\><around*|(|i|)>>>|<row|<cell|>|<cell|=>|<cell|\<mu\><around*|(|1|)>+\<mu\><around*|(|3|)>+\<mu\><around*|(|5|)>+\<ldots\>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<around*|(|\<mu\><around*|(|2|)>+\<mu\><around*|(|4|)>+\<mu\><around*|(|6|)>+\<mu\><around*|(|8|)>+\<mu\><around*|(|10|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<around*|(|\<mu\><around*|(|2|)>+0+\<mu\><around*|(|6|)>+0+\<mu\><around*|(|10|)>+0+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<around*|(|\<mu\><around*|(|2|)>+\<mu\><around*|(|6|)>+\<mu\><around*|(|10|)>+\<mu\><around*|(|14|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<around*|(|\<mu\><around*|(|2\<cdot\>1|)>+\<mu\><around*|(|2\<cdot\>3|)>+\<mu\><around*|(|2\<cdot\>5|)>+\<mu\><around*|(|2\<cdot\>7|)>+\<ldots\>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<big|sum><rsub|i\<leq\>m/2,i
    odd>\<mu\><around*|(|2*i|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<big|sum><rsub|i\<leq\>m/2,i
    odd>\<mu\><around*|(|2|)>*\<mu\><around*|(|i|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>-<big|sum><rsub|i\<leq\>m/2,i
    odd>-1\<cdot\>\<mu\><around*|(|i|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>+<big|sum><rsub|i\<leq\>m/2,i
    odd>\<mu\><around*|(|i|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>+M<rsub|odd><around*|(|<frac|n|2>|)>>>>>
  </eqnarray*>

  Fully expanding the recurrence

  <\eqnarray*>
    <tformat|<table|<row|<cell|M<rsub|odd><around*|(|n|)>=<big|sum><rsub|d\<leq\>n,d
    odd>\<mu\><around*|(|d|)>>|<cell|=>|<cell|M<around*|(|n|)>+M<rsub|odd><around*|(|<frac|n|2>|)>>>|<row|<cell|>|<cell|=>|<cell|M<around*|(|n|)>+M<around*|(|<frac|n|2>|)>+M<rsub|odd><around*|(|<frac|n|4>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=0>M<around*|(|<frac|n|2<rsup|k>>|)>>>>>
  </eqnarray*>

  \;

  Identities involving the Mertens function. \ Starting with

  <\equation*>
    <tabular|<tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n>M<around*|(|<frac|n|m>|)>=1>|<cell|for
    n\<geq\>1 <around*|(|Lehman|)>>>>>>
  </equation*>

  which for completeness we can prove easily using the identity

  <\equation*>
    <big|sum><rsub|d<around*|\||m|\<nobracket\>>>\<mu\><around*|(|d|)>=<choice|<tformat|<table|<row|<cell|1>|<cell|if
    m=1>>|<row|<cell|0>|<cell|if m\<gtr\>1>>>>>
  </equation*>

  and so

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsup|n><rsub|i=1>M<around*|(|<frac|n|i>|)>>|<cell|=>|<cell|<big|sum><rsub|x*y\<leq\>n>\<mu\><around*|(|y|)>=<big|sum><rsub|m=1><rsup|n><big|sum><rsub|d<around*|\||m|\<nobracket\>>>\<mu\><around*|(|d|)>=1+<big|sum><rsup|n><rsub|m=2>0=1>>>>
  </eqnarray*>

  and this leads to related identities

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n,m
    even>M<around*|(|<frac|n|m>|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n/2>M<around*|(|<frac|n|2*m>|)>=<big|sum><rsub|m\<leq\>n/2>M<around*|(|<frac|n/2|m>|)>=1,n\<geq\>2>>|<row|<cell|<big|sum><rsub|m\<leq\>n,m
    odd>M<around*|(|<frac|n|m>|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n>M<around*|(|<frac|n|m>|)>-<big|sum><rsub|m\<leq\>n,m
    even>M<around*|(|<frac|n|m>|)>=1-1=0,n\<geq\>3>>|<row|<cell|<big|sum><rsub|m\<leq\>n><around*|(|-1|)><rsup|m+1>*M<around*|(|<frac|n|m>|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n>M<around*|(|<frac|n|m>|)>-2*<big|sum><rsub|m\<leq\>n/2>M<around*|(|<frac|n/2|m>|)>=1-2=-1,n\<geq\>2>>>>
  </eqnarray*>

  Finally, we can apply these identities to obtain identities for the
  odd-Mertens function.

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|m\<leq\>n,m
    even>M<rsub|odd><around*|(|<frac|n|m>|)>>|<cell|=>|<cell|<big|sum><rsub|m\<leq\>n,m
    even><big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=0>M<around*|(|<frac|n|2<rsup|k>*m>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=0><big|sum><rsub|m\<leq\>n,m
    even>M<around*|(|<frac|n/2<rsup|k>|m>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>-1><rsub|k=0>1+<big|sum><rsub|m\<leq\>1,m
    even>M<around*|(|<frac|1|m>|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>+0>>|<row|<cell|>|<cell|=>|<cell|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>>>|<row|<cell|<big|sum><rsub|m\<leq\>n,m
    odd>M<rsub|odd><around*|(|<frac|n|m>|)>>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>><rsub|k=0><big|sum><rsub|m\<leq\>n,m
    odd>M<around*|(|<frac|n/2<rsup|k>|m>|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|<around*|\<lfloor\>|log<rsub|2>
    n|\<rfloor\>>-1><rsub|k=0>0+<big|sum><rsub|m\<leq\>1,m
    odd>M<around*|(|<frac|1|m>|)>>>|<row|<cell|>|<cell|=>|<cell|0+1>>|<row|<cell|>|<cell|=>|<cell|1>>>>
  </eqnarray*>

  If <math|m mod 3=b> then <math|m<rsup|3>> can only take on three values
  modulo <math|9> because

  <\equation*>
    m<rsup|3>=<around*|(|3*a+b|)><rsup|3>\<equiv\><choice|<tformat|<cwith|1|-1|3|3|cell-halign|r>|<table|<row|<cell|27*a<rsup|3>>|<cell|=>|<cell|0>|<cell|if
    b =0>>|<row|<cell|27a<rsup|3>+27*a<rsup|2>+9*a+1>|<cell|=>|<cell|1>|<cell|if
    b=1>>|<row|<cell|27*a<rsup|3>+54*a<rsup|2>+36*a+8>|<cell|=>|<cell|-1>|<cell|if
    b=2>>>>><space|1em><around*|(|mod 9|)>
  </equation*>

  First simplify and isolate the expression.

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\><sqrt|n|3>,d
    odd>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|3>>|\<rfloor\>><rsup|1/3>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>>|<cell|=>|<cell|<big|sum><rsub|d\<leq\><sqrt|n|3>,d
    odd>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<frac|<sqrt|n|3>|d>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>m,d
    odd>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<frac|m|d>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|3>,m=<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsub|d\<leq\>m,d
    odd>\<mu\><around*|(|d|)>*<around*|\<lfloor\>|<frac|m+d|2*d>|\<rfloor\>><rsup|><rsup|3>,m=<around*|\<lfloor\>|<sqrt|n|3>|\<rfloor\>>>>>>
  </eqnarray*>

  \;

  Finally, show that the expression evalutes to <math|7 mod 9>, for
  <math|n\<geq\>27>.

  <\eqnarray*>
    <tformat|<table|<row|<cell|<big|sum><rsub|d\<leq\>m,d
    odd><rsup|m>\<mu\><around*|(|d|)><around*|\<lfloor\>|<frac|m+d|*2*d>|\<rfloor\>><rsup|3>>|<cell|=>|<cell|<big|sum><rsup|m><rsub|i=1>i<rsup|3>*<big|sum><rsup|<around*|\<lfloor\>|m/2*i-1|\<rfloor\>>><rsub|j=<around*|\<lfloor\>|m/<around*|(|2*i+1|)>|\<rfloor\>>+1,j
    odd>\<mu\><around*|(|j|)>>>|<row|<cell|>|<cell|=>|<cell|<big|sum><rsup|m><rsub|i=1>i<rsup|3>*<around*|(|M<rsub|odd><around*|(|<around*|\<lfloor\>|<frac|m|2*i-1>|\<rfloor\>>|)>-M<rsub|odd><around*|(|<around*|\<lfloor\>|<frac|m|2*i+1>|\<rfloor\>>|)>|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|1\<cdot\><around*|(|
    M<rsub|odd><around*|(|<frac|m|1>|)>-M<rsub|odd><around*|(|<frac|m|3>|)>|)>-1\<cdot\><around*|(|M<rsub|odd><around*|(|<frac|m|3>|)>-M<rsub|odd><around*|(|<frac|m|5>|)>|)>>>|<row|<cell|>|<cell|>|<cell|0\<cdot\><around*|(|M<rsub|odd><around*|(|<frac|m|5>|)>-M<rsub|odd><around*|(|<frac|m|7>|)>|)>+\<ldots\><space|1em><around*|(|mod
    9|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|
    M<rsub|odd><around*|(|<frac|m|1>|)>-M<rsub|odd><around*|(|<frac|m|3>|)>-M<rsub|odd><around*|(|<frac|m|3>|)>+M<rsub|odd><around*|(|<frac|m|5>|)>>>|<row|<cell|>|<cell|+>|<cell|M<rsub|odd><around*|(|<frac|m|7>|)>-M<rsub|odd><around*|(|<frac|m|9>|)>-M<rsub|odd><around*|(|<frac|m|9>|)>+M<rsub|odd><around*|(|<frac|m|11>|)>>>|<row|<cell|>|<cell|+>|<cell|\<ldots\>.>>|<row|<cell|>|<cell|\<equiv\>>|<cell|
    M<rsub|odd><around*|(|<frac|m|1>|)>+M<rsub|odd><around*|(|<frac|m|3>|)>-3*M<rsub|odd><around*|(|<frac|m|3>|)>+M<rsub|odd><around*|(|<frac|m|5>|)>>>|<row|<cell|>|<cell|+>|<cell|M<rsub|odd><around*|(|<frac|m|7>|)>+M<rsub|odd><around*|(|<frac|m|9>|)>-3*M<rsub|odd><around*|(|<frac|m|9>|)>+M<rsub|odd><around*|(|<frac|m|11>|)>>>|<row|<cell|>|<cell|+>|<cell|\<ldots\>.>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|i\<leq\>m,i
    odd>M<rsub|odd><around*|(|<frac|m|i>|)>-<big|sum><rsub|i\<leq\>m/3,i
    odd>3*M<rsub|odd><around*|(|<frac|m|3*i>|)><rsub|>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|<big|sum><rsub|i\<leq\>m,i
    odd>M<rsub|odd><around*|(|<frac|m|i>|)>-3*<big|sum><rsub|i\<leq\>m/3,i
    odd>M<rsub|odd><around*|(|<frac|m/3|i>|)>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|1-3\<cdot\><around*|[|m\<geq\>3|]>>>|<row|<cell|>|<cell|\<equiv\>>|<cell|7
    <around*|(|mod 9|)>,m\<geq\>3>>>>
  </eqnarray*>

  Modifying <math|T<rsub|3,odd>> slightly gives

  <\eqnarray*>
    <tformat|<table|<row|<cell|T<rprime|'><rsub|3,odd><around*|(|n|)>>|<cell|=>|<cell|<big|sum><rsub|z\<leq\><sqrt|n,|3>z
    odd><rsup|><around*|(|2*S<rsub|odd><around*|(|<around*|\<lfloor\>|<frac|n|z>|\<rfloor\>>;z+2,<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>|)>-<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<sqrt|<frac|n|z>>|\<rfloor\>>+1|2>|\<rfloor\>><rsup|2>+<around*|\<lfloor\>|<frac|<around*|\<lfloor\>|<frac|n|z<rsup|2>>|\<rfloor\>>+1|2>|\<rfloor\>>|)>>>>>
  </eqnarray*>

  <\eqnarray*>
    <tformat|<table|<row|<cell|F<rsub|3,odd><around*|(|n|)>>|<cell|\<equiv\>>|<cell|<big|sum><rsub|d\<leq\><sqrt|n|3>,d
    odd>\<mu\><around*|(|d|)>*T<rprime|'><rsub|3,odd><around*|(|<around*|\<lfloor\>|<frac|n<rsup|>|d<rsup|3>>|\<rfloor\>>|)>+2*<around*|[|n\<geq\>27|]>
    <around*|(|mod 3|)>>>>>
  </eqnarray*>

  <section|Division-Free Counting for Odd Divisors>

  The division-free counting method is easily adapted to processing alternate
  indices.

  <\eqnarray*>
    <tformat|<table|<row|<cell|\<beta\><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|n/x|\<rfloor\>>>>|<row|<cell|\<delta\><rsub|1><around*|(|x|)>>|<cell|=>|<cell|\<beta\><around*|(|x|)>-\<beta\><around*|(|x+2|)>>>|<row|<cell|\<delta\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|1><around*|(|x+2|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>>|<cell|=>|<cell|n-x*\<beta\><around*|(|x|)>>>|<row|<cell|\<varepsilon\><around*|(|x+2|)>>|<cell|=>|<cell|n-<around*|(|x+2|)>*\<beta\><around*|(|x+2|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>-\<varepsilon\><around*|(|x+2|)>>|<cell|=>|<cell|<around*|(|x+2|)>*\<beta\><around*|(|x+2|)>-x*\<beta\><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|<around*|(|x+2|)>*\<beta\><around*|(|x+2|)>-x*<around*|(|\<beta\><around*|(|x+2|)>+\<delta\><rsub|1><around*|(|x+2|)>+\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|2*\<beta\><around*|(|x+2|)>-x*\<delta\><rsub|1><around*|(|x+2|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x|)>>|<cell|=>|<cell|2*\<beta\><around*|(|x|)>-<around*|(|x-2|)>*\<delta\><rsub|1><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x+2|)>>|<cell|=>|<cell|2*\<beta\><around*|(|x+2|)>-x*\<delta\><rsub|1><around*|(|x+2|)>>>|<row|<cell|\<gamma\><around*|(|x|)>-\<gamma\><around*|(|x+2|)>>|<cell|=>|<cell|2*\<beta\><around*|(|x|)>-2*\<beta\><around*|(|x+2|)>-<around*|(|x-2|)>*\<delta\><rsub|1><around*|(|x|)>+x*\<delta\><rsub|1><around*|(|x+2|)>>>|<row|<cell|>|<cell|=>|<cell|2*<around*|(|\<beta\><around*|(|x|)>-\<beta\><around*|(|x+2|)>|)>-<around*|(|x-2|)>*\<delta\><rsub|1><around*|(|x|)>+x*<around*|(|\<delta\><rsub|1><around*|(|x|)>-\<delta\><rsub|2><around*|(|x|)>|)>>>|<row|<cell|>|<cell|=>|<cell|2*\<delta\><rsub|1><around*|(|x|)>+2*\<delta\><rsub|1><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|>|<cell|=>|<cell|4*\<delta\><rsub|1><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>>|<cell|=>|<cell|\<varepsilon\><around*|(|x+2|)>+\<gamma\><around*|(|x+1|)>>>|<row|<cell|\<delta\><rsub|2><around*|(|x|)>>|<cell|=>|<cell|<around*|\<lfloor\>|<frac|<wide|\<varepsilon\>|^><around*|(|x|)>|x>|\<rfloor\>>>>|<row|<cell|\<delta\><rsub|1><around*|(|x|)>>|<cell|=>|<cell|\<delta\><rsub|1><around*|(|x+2|)>+\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<varepsilon\><around*|(|x|)>>|<cell|=>|<cell|<wide|\<varepsilon\>|^><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<gamma\><around*|(|x|)>>|<cell|=>|<cell|\<gamma\><around*|(|x+2|)>+4*\<delta\><rsub|1><around*|(|x|)>-x*\<delta\><rsub|2><around*|(|x|)>>>|<row|<cell|\<beta\><around*|(|x|)>>|<cell|=>|<cell|\<beta\><around*|(|x+2|)>+\<delta\><rsub|1><around*|(|x|)>>>>>
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
    <associate|auto-2|<tuple|2|?>>
    <associate|auto-3|<tuple|3|?>>
    <associate|auto-4|<tuple|4|?>>
    <associate|auto-5|<tuple|5|?>>
  </collection>
</references>

<\auxiliary>
  <\collection>
    <\associate|toc>
      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Modular
      Prime Counting> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-1><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Prime
      Counting Function Modulo 2> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-2><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Prime
      Counting Function Modulo 3> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-3><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Simplifying
      Summation Expressions> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-4><vspace|0.5fn>

      <vspace*|1fn><with|font-series|<quote|bold>|math-font-series|<quote|bold>|Division-Free
      Counting for Odd Divisors> <datoms|<macro|x|<repeat|<arg|x>|<with|font-series|medium|<with|font-size|1|<space|0.2fn>.<space|0.2fn>>>>>|<htab|5mm>>
      <no-break><pageref|auto-5><vspace|0.5fn>
    </associate>
  </collection>
</auxiliary>