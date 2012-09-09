#include "stdafx.h"

DivisorSummatoryFunctionOdd::DivisorSummatoryFunctionOdd(int threads)
{
    this->threads = threads;
    C1 = 250;
    C2 = 5;
    nslow = (Integer)1 << 92;
    nsmall = (Integer)1 << 60;
    tmax = (UInt64)1 << 62;
    maximumBatchSize = (Int64)1 << 28;
    pthread_mutex_init(&lock, NULL);
}

DivisorSummatoryFunctionOdd::~DivisorSummatoryFunctionOdd()
{
    pthread_mutex_destroy(&lock);
}

Integer DivisorSummatoryFunctionOdd::Evaluate(Integer n)
{
    Integer xmax = FloorSquareRoot(n);
    Integer s = Evaluate(n, 1, xmax);
    return 2 * s - xmax * xmax;
}

Integer DivisorSummatoryFunctionOdd::Evaluate(Integer n, Integer x0, Integer xmax)
{
    regions = new BlockingCollection<Region>();
    sum = 0;
    if (threads == 0)
    {
        AddToSum(EvaluateInternal(n, x0, xmax));
        AddToSum(S1(n, 2 * x0 - 1, 2 * xmanual - 3));
        return sum;
    }

    // Create consumers.
    int consumers = threads;
    pthread_t *tasks = new pthread_t[consumers];
    for (int consumer = 0; consumer < consumers; consumer++)
    {
        int rc = pthread_create(&tasks[consumer], NULL, ConsumeRegions, this);
        if (rc != 0)
            fprintf(stderr, "failed to create thread\n");
    }

    // Produce work items.
    unprocessed = 1;
    finished.Reset();
    AddToSum(Processed(EvaluateInternal(n, x0, xmax)));
    finished.Wait();
    regions->CompleteAdding();

    // Add manual portion.
    S1Parallel(2 * x0 - 1, 2 * xmanual - 3);

    // Wait for completion.
    for (int consumer = 0; consumer < consumers; consumer++)
        pthread_join(tasks[consumer], NULL);
    delete[] tasks;
    delete regions;

    return sum;
}

void *DivisorSummatoryFunctionOdd::ConsumeRegions(void *data)
{
    ((DivisorSummatoryFunctionOdd *)data)->ConsumeRegions();
    return 0;
}

void DivisorSummatoryFunctionOdd::ConsumeRegions()
{
    Integer s = 0;
    Region r;
    while (regions->TryTake(r))
        s += ProcessRegion(r.w, r.h, r.a1, r.b1, r.c1, r.a2, r.b2, r.c2);
    AddToSum(s);
}

void DivisorSummatoryFunctionOdd::Enqueue(Region r)
{
    Interlocked::Add(unprocessed, 1);
    regions->Add(r);
}

Integer DivisorSummatoryFunctionOdd::Processed(Integer result)
{
    int value = Interlocked::Add(unprocessed, -1);
    if (value == 0)
        finished.Set();
    return result;
}

Integer DivisorSummatoryFunctionOdd::EvaluateInternal(Integer n, Integer xfirst, Integer xlast)
{
    this->n = n;
    Integer x0 = T1(xfirst + 1);
    Integer xmax = T1(xlast);
    if (x0 > xmax)
        return 0;
    Integer ymin = YFloor(xmax);
    Integer xmin = Max(x0, Min(T1(C1 * CeilingRoot(2 * n, 3)), xmax));
#if DEBUG
    printf("n = %s, xmin = %s, xmax = %s\n", Integer2mpz(n).get_str().c_str(), Integer2mpz(xmin).get_str().c_str(), Integer2mpz(xmax).get_str().c_str());
#endif
    Integer s = (Integer)0;
    Integer a2 = (Integer)1;
    Integer x2 = xmax;
    Integer y2 = ymin;
    Integer c2 = a2 * x2 + y2;
    while (true)
    {
        Integer a1 = a2 + 1;
        Integer x4 = YTan(a1);
        Integer y4 = YFloor(x4);
        Integer c4 = a1 * x4 + y4;
        Integer x5 = x4 + 1;
        Integer y5 = YFloor(x5);
        Integer c5 = a1 * x5 + y5;
        if (x4 < xmin)
            break;
        s += Triangle(c4 - c2 - x0) - Triangle(c4 - c2 - x5) + Triangle(c5 - c2 - x5);
        if (threads == 0)
        {
            s += ProcessRegion(a1 * x2 + y2 - c5, a2 * x5 + y5 - c2, a1, 1, c5, a2, 1, c2);
            while (regions->GetSize() > 0)
            {
                Region r = regions->Take();
                s += ProcessRegion(r.w, r.h, r.a1, r.b1, r.c1, r.a2, r.b2, r.c2);
            }
        }
        else
            Enqueue(Region(a1 * x2 + y2 - c5, a2 * x5 + y5 - c2, a1, 1, c5, a2, 1, c2));
        a2 = a1;
        x2 = x4;
        y2 = y4;
        c2 = c4;
    }
    s += (xmax - x0 + 1) * ymin + Triangle(xmax - x0);
    Integer rest = x2 - x0;
    s -= y2 * rest + a2 * Triangle(rest);
    xmanual = x2;
    return s;
}

Integer DivisorSummatoryFunctionOdd::ProcessRegion(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
{
#if DEBUG
    printf("ProcessRegion: w = %s, h = %s, a1/b1 = %s/%s, a2/b2 = %s/%s, c1 = %s, c2 = %s\n", Integer2mpz(w).get_str().c_str(), Integer2mpz(h).get_str().c_str(), Integer2mpz(a1).get_str().c_str(), Integer2mpz(b1).get_str().c_str(), Integer2mpz(a2).get_str().c_str(), Integer2mpz(b2).get_str().c_str(), Integer2mpz(c1).get_str().c_str(), Integer2mpz(c2).get_str().c_str());
#endif
    Integer s = (Integer)0;
    while (true)
    {
        Integer a3 = a1 + a2;
        Integer b3 = b1 + b2;
        if (h > 0 && H(w, 1, a1, b1, c1, a2, b2, c2) <= n)
        {
            s += w;
            ++c2;
            --h;
        }
        Assert(h == 0 || H(w, 0, a1, b1, c1, a2, b2, c2) <= n && H(w, 1, a1, b1, c1, a2, b2, c2) > n);
        if (w > 0 && H(1, h, a1, b1, c1, a2, b2, c2) <= n)
        {
            s += h;
            ++c1;
            --w;
        }
        Assert(w == 0 || H(0, h, a1, b1, c1, a2, b2, c2) <= n && H(1, h, a1, b1, c1, a2, b2, c2) > n);
        Integer ab1 = a1 + b1;
        Integer a3b3 = (a1 + a2) * (b1 + b2);
        Integer abba = a1 * b2 + b1 * a2;
        Integer ab2 = 2 * a1 * b1;
        Integer u4 = UTan(ab1, abba, ab2, a3b3, c1);
        if (u4 <= 0)
            return Processed(s + ProcessRegionManual(w, h, a1, b1, c1, a2, b2, c2));
        Integer u5 = u4 + 1;
        Integer v4, v5;
        VFloor2(u4, a1, b1, c1, c2, abba, ab2, v4, v5);
        Assert(v4 == VFloor(u4, a1, b1, c1, a2, b2, c2));
        Assert(v5 == VFloor(u5, a1, b1, c1, a2, b2, c2));
#if DEBUG
        printf("u4 = %d, v4 = %d, H(u4, v4) = %d\n",
            (int)u4, (int)v4, (int)H(u4, v4, a1, b1, c1, a2, b2, c2));
#endif
        Assert(H(u4, v4, a1, b1, c1, a2, b2, c2) <= n && H(u4, v4 + 1, a1, b1, c1, a2, b2, c2) > n);
        Assert(H(u5, v5, a1, b1, c1, a2, b2, c2) <= n && H(u5, v5 + 1, a1, b1, c1, a2, b2, c2) > n);
        Integer v6 = u4 + v4;
        Integer u7 = u5 + v5;
        if (u4 <= C2 || v5 <= C2 || v6 >= h || u7 >= w)
            return Processed(s + ProcessRegionManual(w, h, a1, b1, c1, a2, b2, c2));
        if (v6 != u7)
            s += Triangle(v6 - 1) - Triangle(v6 - u5) + Triangle(u7 - u5);
        else
            s += Triangle(v6 - 1);
#if DEBUG
        printf("ProcessRegion: s = %s\n", Integer2mpz(s).get_str().c_str());
#endif
        if (threads == 0)
        {
            Enqueue(Region(u4, h - v6, a1, b1, c1, a3, b3, c1 + c2 + v6));
            w -= u7;
            h = v5;
            a1 = a3;
            b1 = b3;
            c1 += c2 + u7;
        }
        else
        {
            Enqueue(Region(u4, h - v6, a1, b1, c1, a3, b3, c1 + c2 + v6));
            Enqueue(Region(w - u7, v5, a3, b3, c1 + c2 + u7, a2, b2, c2));
            return Processed(s);
        }
    }
}

Integer DivisorSummatoryFunctionOdd::ProcessRegionManual(Integer w,  Integer h,  Integer a1,  Integer b1,  Integer c1,  Integer a2,  Integer b2,  Integer c2)
{
    return w < h ? ProcessRegionManual(w, a1, b1, c1, a2, b2, c2) : ProcessRegionManual(h, b2, a2, c2, b1, a1, c1);
}

Integer DivisorSummatoryFunctionOdd::ProcessRegionManual(Integer w,  Integer a1,  Integer b1,  Integer c1,  Integer a2,  Integer b2,  Integer c2)
{
    if (w <= 1)
        return 0;

    Integer s = (Integer)0;
    Integer umax = (Integer)w - 1;
    Integer t1 = 2 * (a1 * b2 + b1 * a2);
    Integer t2 = 2 * c1 - a1 - b1;
    Integer t3 = 4 * t2 + 12;
    Integer t4 = 4 * a1 * b1;
    Integer t5 = t1 * (1 + c1) - a1 + b1 - t4 * c2;
    Integer t6 = Square(t2 + 2) - t4 * n;

    Integer u = (Integer)1;
    mpz_t reg1, reg2, reg3;
    mpz_init(reg1); mpz_init(reg2); mpz_init(reg3);
    while (true)
    {
        Assert((t5 - CeilingSquareRoot(t6)) / t4 == VFloor(u, a1, b1, c1, a2, b2, c2));
        s += (t5 - CeilingSquareRoot(t6, reg1, reg2, reg3)) / t4;
        if (u >= umax)
            break;
        t5 += t1;
        t6 += t3;
        t3 += 8;
        ++u;
    }
    mpz_clear(reg1); mpz_clear(reg2); mpz_clear(reg3);

    Assert(s == ProcessRegionHorizontal(w, 0, a1, b1, c1, a2, b2, c2));
#if DEBUG
    printf("ProcessRegionManual: s = %s\n", Integer2mpz(s).get_str().c_str());
#endif
    return s;
}

Integer DivisorSummatoryFunctionOdd::ProcessRegionHorizontal(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
{
    Integer s = (Integer)0;
    for (Integer u = (Integer)1; u < w; u++)
        s += VFloor(u, a1, b1, c1, a2, b2, c2);
    return s;
}

Integer DivisorSummatoryFunctionOdd::ProcessRegionVertical(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
{
    Integer s = (Integer)0;
    for (Integer v = (Integer)1; v < h; v++)
        s += UFloor(v, a1, b1, c1, a2, b2, c2);
    return s;
}

void DivisorSummatoryFunctionOdd::S1Parallel(Integer xmin, Integer xmax)
{
    // Create consumers.
    ranges = new BlockingCollection<Range>();
    int consumers = threads;
    pthread_t *tasks = new pthread_t[consumers];
    for (int consumer = 0; consumer < consumers; consumer++)
    {
        int rc = pthread_create(&tasks[consumer], NULL, ConsumeRanges, this);
        if (rc != 0)
            fprintf(stderr, "failed to create thread\n");
    }

    // Produce work items.
    ProduceRanges(xmin, xmax);

    // Wait for completion.
    ranges->CompleteAdding();
    for (int consumer = 0; consumer < consumers; consumer++)
        pthread_join(tasks[consumer], NULL);
    delete[] tasks;
    delete ranges;
}

void DivisorSummatoryFunctionOdd::ProduceRanges(Integer imin, Integer imax)
{
    int batchSize = Min(maximumBatchSize, (imax - imin + 1 + threads - 1) / threads);
    for (Integer i = imin; i <= imax; i += batchSize)
        ranges->Add(Range(i, Min(i + batchSize - 1, imax)));
}

void *DivisorSummatoryFunctionOdd::ConsumeRanges(void *data)
{
    ((DivisorSummatoryFunctionOdd *)data)->ConsumeRanges();
}

void DivisorSummatoryFunctionOdd::ConsumeRanges()
{
    Range r;
    while (ranges->TryTake(r))
        AddToSum(S1(n, r.Min, r.Max));
}

Integer DivisorSummatoryFunctionOdd::S1(Integer n, Integer x1, Integer x2)
{
    if (x1 > x2)
        return 0;
    return (n <= nslow ? S1Fast(n, x1, x2) : S1Slow(n, x1, x2)) / 2;
}

Integer DivisorSummatoryFunctionOdd::S1Fast(Integer n, Int64 x1, Int64 x2)
{
    if (n < nsmall)
        return S1SmallFast(n, x1, x2);
    Integer s = (Integer)0;
    UInt64 t = (UInt64)0;
    Int64 x = (x2 - 1) | 1;
    UInt64 beta = Integer(n / Integer(x + 2));
    Int64 eps = Integer(n % Integer(x + 2));
    Int64 delta = Integer(n / Integer(x) - Integer(beta));
    Int64 gamma = 2 * beta - x * delta;
    while (x >= x1)
    {
        eps += gamma;
        if (eps >= x)
        {
            ++delta;
            gamma -= x;
            eps -= x;
            if (eps >= x)
            {
                ++delta;
                gamma -= x;
                eps -= x;
                if (eps >= x)
                    break;
            }
        }
        else if (eps < 0)
        {
            --delta;
            gamma += x;
            eps += x;
        }
        beta += delta;
        gamma += delta << 2;
        Assert(Integer(eps) == n % Integer(x));
        Assert(Integer(beta) == n / Integer(x));
        Assert(Integer(delta) == Integer(beta) - n / Integer((x + 2)));
        Assert(Integer(gamma) == 2 * Integer(beta) - Integer(x - 2) * Integer(delta));
        t += beta + (beta & 1);
        if (t >= tmax)
        {
            s += Integer(t);
            t = 0;
        }
        x -= 2;
    }
    return s + Integer(t) + S1Slow(n, x1, x);
}

Integer DivisorSummatoryFunctionOdd::S1Slow(Integer n, Integer x1, Integer x2)
{
    Integer s = (Integer)0;
    for (Integer x = (x2 - 1) | 1; x >= x1; x -= 2)
    {
        Integer beta = n / x;
        s += beta + (beta & 1);
    }
    return s;
}

Integer DivisorSummatoryFunctionOdd::S1SmallFast(Integer n, Int64 x1, Int64 x2)
{
    UInt64 t = (UInt64)0;
    Int64 x = (x2 - 1) | 1;
    UInt64 beta = Integer(n / Integer(x + 2));
    Int64 eps = Integer(n % Integer(x + 2));
    Int64 delta = Integer(n / Integer(x) - Integer(beta));
    Int64 gamma = 2 * beta - x * delta;
    while (x >= x1)
    {
        eps += gamma;
        if (eps >= x)
        {
            ++delta;
            gamma -= x;
            eps -= x;
            if (eps >= x)
            {
                ++delta;
                gamma -= x;
                eps -= x;
                if (eps >= x)
                    break;
            }
        }
        else if (eps < 0)
        {
            --delta;
            gamma += x;
            eps += x;
        }
        beta += delta;
        gamma += delta << 2;
        Assert(Integer(eps) == n % Integer(x));
        Assert(Integer(beta) == n / Integer(x));
        Assert(Integer(delta) == Integer(beta) - n / Integer((x + 2)));
        Assert(Integer(gamma) == 2 * Integer(beta) - Integer(x - 2) * Integer(delta));
        t += beta + (beta & 1);
        x -= 2;
    }
    return Integer(t) + S1SmallSlow(n, x1, x);
}

Integer DivisorSummatoryFunctionOdd::S1SmallSlow(UInt64 n, Int64 x1, Int64 x2)
{
    UInt64 s = (UInt64)0;
    for (Int64 x = (x2 - 1) | 1; x >= x1; x -= 2)
    {
        UInt64 beta = n / x;
        s += beta + (beta & 1);
    }
    return s;
}
