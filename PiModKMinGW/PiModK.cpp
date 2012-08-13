#include "stdafx.h"

#include "FixedAllocator.h"

void Compute()
{
    DivisorSummatoryFunctionOdd algorithm(8);
    CStopWatch timer;
    for (int i = 1; i <= 24; i++)
    {
        Integer n = Power(Integer(10), i);
        Integer x2 = FloorSquareRoot(n);
        timer.startTimer();
        Integer s = algorithm.Evaluate(n, 1, x2);
        timer.stopTimer();
        printf("i = %d, s = %s, elapsed = %.3f\n", i, Integer2mpz(s).get_str().c_str(), timer.getElapsedTime() * 1000);
    }
}

int _tmain(int argc, _TCHAR* argv[])
{
    init_func();
    Compute();
    exit_func();
    return 0;
}

