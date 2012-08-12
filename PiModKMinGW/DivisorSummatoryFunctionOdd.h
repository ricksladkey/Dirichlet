#include "stdafx.h"
#include "BlockingCollection.h"

class DivisorSummatoryFunctionOdd
{
public:

    struct Region
    {
    public:

        Region()
        {
        }

        Region(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
        {
            this->w = w;
            this->h = h;
            this->a1 = a1;
            this->b1 = b1;
            this->c1 = c1;
            this->a2 = a2;
            this->b2 = b2;
            this->c2 = c2;
        }

        Integer w;
        Integer h;
        Integer a1;
        Integer b1;
        Integer c1;
        Integer a2;
        Integer b2;
        Integer c2;
    };

    struct Range
    {
    public:
        Range()
        {
        }

        Range(Integer min, Integer max)
        {
            Min = min;
            Max = max;
        }
        Integer Min;
        Integer Max;
    };

    Integer C1;
    Integer C2;
    Integer nslow;
    Integer nsmall;
    UInt64 tmax;
    Int64 maximumBatchSize;

    int threads;
    Integer n;
    Integer sum;
    int unprocessed;
    Integer xmanual;
    ManualResetEvent finished;
    BlockingCollection<Region> regions;
    BlockingCollection<Range> ranges;

    DivisorSummatoryFunctionOdd(int threads);
    Integer Evaluate(Integer n);
    Integer Evaluate(Integer n, Integer x0, Integer xmax);

    static void *ConsumeRegions(void *data);
    void ConsumeRegions();
    void Enqueue(Region r);
    Integer Processed(Integer result);
    Integer EvaluateInternal(Integer n, Integer xfirst, Integer xlast);

    Integer ProcessRegion(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2);
    Integer ProcessRegionManual(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2);
    Integer ProcessRegionManual(Integer w, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2);
    Integer ProcessRegionHorizontal(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2);
    Integer ProcessRegionVertical(Integer w, Integer h, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2);

    Integer H(Integer u, Integer v, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
    {
        Integer uu = u + c1;
        Integer vv = v + c2;
        return (2 * (b2 * uu - b1 * vv) - 1) * (2 * (a1 * vv - a2 * uu) - 1);
    }

    Integer UTan(Integer ab1, Integer abba, Integer ab2, Integer a3b3, Integer c1)
    {
        return (ab1 + FloorSquareRoot(Square(abba + ab2) * n / a3b3) - 2 * c1) / 2;
    }

    Integer UFloor(Integer v, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
    {
        return (2 * (a1 * b2 + b1 * a2) * (v + c2) + a2 - b2 - CeilingSquareRoot(Square(2 * (v + c2) - a2 - b2) - 4 * a2 * b2 * n)) / (4 * a2 * b2) - c1;
    }

    Integer VFloor(Integer u, Integer a1, Integer b1, Integer c1, Integer a2, Integer b2, Integer c2)
    {
        return (2 * (a1 * b2 + b1 * a2) * (u + c1) - a1 + b1 - CeilingSquareRoot(Square(2 * (u + c1) - a1 - b1) - 4 * a1 * b1 * n)) / (4 * a1 * b1) - c2;
    }

    void VFloor2(Integer u1, Integer a1, Integer b1, Integer c1, Integer c2, Integer abba, Integer ab2, Integer& v1, Integer& v2)
    {
        Integer uu = 2 * (u1 + c1);
        Integer t1 = 2 * ab2;
        Integer t2 = abba * uu - a1 + b1 - t1 * c2;
        Integer t3 = uu - a1 - b1;
        Integer t4 = Square(t3) - t1 * n;
        v1 = (t2 - CeilingSquareRoot(t4)) / t1;
        v2 = (t2 + 2 * abba - CeilingSquareRoot(t4 + 4 * (t3 + 1))) / t1;
    }

    static Integer Triangle(Integer a)
    {
        return a * (a + 1) / 2;
    }

    Integer T1(Integer x)
    {
        return (x + 1) / 2;
    }

    Integer YTan(Integer a)
    {
        return T1(FloorSquareRoot(n / a));
    }

    Integer YFloor(Integer x)
    {
        return (n / (2 * x - 1) + 1) / 2;
    }

    void AddToSum(Integer s)
    {
        sum += s;
    }

    void S1Parallel(Integer xmin, Integer xmax);
    void ProduceRanges(Integer imin, Integer imax);
    static void *ConsumeRanges(void *data);
    void ConsumeRanges();

    Integer S1(Integer n, Integer x1, Integer x2);
    Integer S1Fast(Integer n, Int64 x1, Int64 x2);
    Integer S1Slow(Integer n, Integer x1, Integer x2);
    Integer S1SmallFast(Integer n, Int64 x1, Int64 x2);
    Integer S1SmallSlow(UInt64 n, Int64 x1, Int64 x2);
};
