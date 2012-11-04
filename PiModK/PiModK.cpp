#include "stdafx.h"

#include "FixedAllocator.h"

void Compute()
{
    DivisorSummatoryFunctionOdd algorithm;
    CStopWatch timer;
    for (int i = 1; i <= 24; i++)
    {
        Integer n = Power(Integer(10), i);
        Integer x2 = sqrt(n);
        timer.startTimer();
        Integer s = algorithm.Evaluate(n, 1, x2);
        timer.stopTimer();
        std::string sRep = s.get_str();
        printf("i = %d, s = %s, elapsed = %.3f\n", i, sRep.c_str(), timer.getElapsedTime() * 1000);
    }
}

int _tmain(int argc, _TCHAR* argv[])
{
    init_func();
    Compute();
    exit_func();
	return 0;
}

