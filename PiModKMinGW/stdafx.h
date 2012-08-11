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
typedef __int128_t Integer;

static inline Integer mpz2Integer(mpz_class a)
{
    assert(mp_bits_per_limb == 64);
    union {
        UInt64 words[2];
        Integer res;
    } u;
    u.words[0] = u.words[1] = 0;
    mpz_export(u.words, 0, -1, sizeof(UInt64), 0, 0, a.get_mpz_t());
    return u.res;
}

static inline mpz_class Integer2mpz(Integer a)
{
    assert(mp_bits_per_limb == 64);
    union {
        UInt64 words[2];
        Integer i;
    } u;
    u.i = a;
    mpz_class res;
    mpz_import(res.get_mpz_t(), 2, -1, sizeof(UInt64), 0, 0, u.words);
    return res;
}

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
    Integer result = 1;
    for (int i = 1; i <= b; i++)
        result *= a;
    return result;
}

static inline Integer FloorSquareRoot(Integer a)
{
    return mpz2Integer(sqrt(Integer2mpz(a)));
}

static inline Integer CeilingSquareRoot(Integer a)
{
    mpz_class result, rem;
    mpz_sqrtrem(result.get_mpz_t(), rem.get_mpz_t(), Integer2mpz(a).get_mpz_t());
    return mpz2Integer(rem == 0 ? result : result + 1);
}

static inline Integer CeilingRoot(Integer a, int root)
{
    mpz_class result, rem;
    mpz_root(result.get_mpz_t(), Integer2mpz(a).get_mpz_t(), root);
    Integer result2 = mpz2Integer(result);
    return Power(result2, root) == a ? result2 : result2 + 1;
}

#ifdef _DEBUG
#define Assert(value) \
    do \
    { \
        if (!(value)) \
        { \
            ReportFailure(#value, __FILE__, __LINE__); \
            exit(1); \
        } \
    } while(0)  
static inline void ReportFailure(const char *value, const char *file, int line)
{
    printf("Assert: %s, %s:%d", value, file, line);
}
#else
#define Assert(value)
#endif

#include "hr_time.h"
#include "DivisorSummatoryFunctionOdd.h"
