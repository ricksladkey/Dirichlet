// PiModK.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

int _tmain(int argc, _TCHAR* argv[])
{
    DivisorSummatoryFunctionOdd algorithm;
    CStopWatch timer;
    for (int i = 16; i <= 16; i++)
    {
        Integer n = Power(Integer(10), i);
        Integer x2 = sqrt(n);
        timer.startTimer();
        Integer s = algorithm.Evaluate(n, 1, x2);
        timer.stopTimer();
        printf("i = %d, s = %s, elapsed = %g\n", i, s.get_str().c_str(), timer.getElapsedTime() * 1000);
    }
	return 0;
}

