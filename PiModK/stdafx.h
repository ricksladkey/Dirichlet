// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <assert.h>

#define _STDINT_H
typedef long long intmax_t;
typedef unsigned long long uintmax_t;

#include "mpirxx.h"

#include <stack>

typedef long long Int64;
typedef unsigned long long UInt64;
typedef mpz_class Integer;

static inline Integer Square(Integer a)
{
    return a * a;
}

static inline Integer Min(Integer a, Integer b)
{
    return a < b ? a : b;
}

static inline Integer Max(Integer a, Integer b)
{
    return a > b ? a : b;
}

static inline Integer Power(Integer a, int b)
{
    Integer result;
    mpz_pow_ui(result.get_mpz_t(), a.get_mpz_t(), b);
    return result;
}

static inline Integer FloorSquareRoot(Integer a)
{
    return sqrt(a);
}

static inline Integer CeilingSquareRoot(Integer a)
{
    Integer result, rem;
    mpz_sqrtrem(result.get_mpz_t(), rem.get_mpz_t(), a.get_mpz_t());
    return rem == 0 ? result : result + 1;
}

static inline Integer CeilingRoot(Integer a, int root)
{
    Integer result, rem;
    mpz_root(result.get_mpz_t(), a.get_mpz_t(), root);
    return Power(result, root) == a ? result : result + 1;
}

#ifdef _DEBUG
static inline void Assert(bool value)
{
    assert(value);
}
#else
#define Assert(value)
#endif

#include "hr_time.h"
#include "DivisorSummatoryFunctionOdd.h"
