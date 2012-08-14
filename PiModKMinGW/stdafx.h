// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <assert.h>

#include <stack>

#include "Interlocked.h"
#include "ResetEvent.h"
#include "IntegerMath.h"
#include "hr_time.h"
#include "DivisorSummatoryFunctionOdd.h"

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
