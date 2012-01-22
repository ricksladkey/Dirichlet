using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using CountInt = System.Byte;

#if false
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
using Solver = Decompose.Numerics.GaussianElimination<Decompose.Numerics.Word64BitArray>;
#endif

#if false
using BitMatrix = Decompose.Numerics.Word64BitMatrix;
using Solver = Decompose.Numerics.StructuredGaussianElimination<Decompose.Numerics.Word64BitArray, Decompose.Numerics.Word64BitMatrix>;
#endif

#if true
using BitMatrix = Decompose.Numerics.SetBitMatrix;
using Solver = Decompose.Numerics.StructuredGaussianElimination<Decompose.Numerics.Word64BitArray, Decompose.Numerics.Word64BitMatrix>;
#endif

namespace Decompose.Numerics
{
    public class QuadraticSieve : IFactorizationAlgorithm<BigInteger>
    {
        [Flags]
        public enum Diag
        {
            None = 0x0,
            Summary = 0x1,
            Solutions = 0x2,
            Sieve = 0x4,
            Timing = 0x8,
            Solving = 0x10,
            SaveMatrix = 0x20,
            Polynomials = 0x40,
            Verbose = Summary | Sieve | Timing | Solving,
        }

        public enum Algorithm
        {
            None = 0,
            SelfInitializingQuadraticSieve = 1,
        }

        public class Config
        {
            public Algorithm Algorithm { get; set; }
            public Diag Diagnostics { get; set; }
            public int Threads { get; set; }
            public int FactorBaseSize { get; set; }
            public int BlockSize { get; set; }
            public int IntervalSize { get; set; }
            public double ThresholdExponent { get; set; }
            public int Multiplier { get; set; }
            public int ReportingInterval { get; set; }
            public int MergeLimit { get; set; }
            public int SieveTimeLimit { get; set; }
            public int CofactorCutoff { get; set; }
            public double ErrorLimit { get; set; }
            public int NumberOfFactors { get; set; }
        }

        public QuadraticSieve(Config config)
        {
            this.config = config;
            diag = config.Diagnostics;
            sieveTimeLimit = config.SieveTimeLimit;
            random = new MersenneTwister32(0);
            smallIntegerFactorer = new TrialDivisionFactorization();
            allPrimes = new SieveOfErostothones();
            solver = new Solver(config.Threads, config.MergeLimit, (diag & Diag.Solving) != 0);
        }

        public IEnumerable<BigInteger> Factor(BigInteger n)
        {
            if (n.IsOne)
            {
                yield return BigInteger.One;
                yield break;
            }
            while (!IntegerMath.IsPrime(n))
            {
                var divisor = GetDivisor(n);
                if (divisor.IsZero || divisor.IsOne)
                    yield break;
                foreach (var factor in Factor(divisor))
                    yield return factor;
                n /= divisor;
            }
            yield return n;
        }

        private struct ExponentEntry
        {
            public int Row { get; set; }
            public int Exponent { get; set; }
            public override string ToString()
            {
                return string.Format("Row[{0}] ^ {1}", Row, Exponent);
            }
        }

        private class ExponentEntries : IEnumerable<ExponentEntry>, IEquatable<ExponentEntries>
        {
            private ExponentEntry[] entries;
            public ExponentEntries(ExponentEntry[] entries) { this.entries = entries; }
            public int Count { get { return entries.Length; } }
            public ExponentEntry this[int index] { get { return entries[index]; } }
            public IEnumerator<ExponentEntry> GetEnumerator() { return (entries as IEnumerable<ExponentEntry>).GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public bool Equals(ExponentEntries other)
            {
                if (Count != other.Count)
                    return false;
                for (int i = 0; i < Count; i++)
                {
                    if (entries[i].Row != other.entries[i].Row)
                        return false;
                    if (entries[i].Exponent != other.entries[i].Exponent)
                        return false;
                }
                return true;
            }
            public override int GetHashCode()
            {
                var hashCode = 0;
                for (int i = 0; i < Count; i++)
                    hashCode = (hashCode << 5) ^ ((entries[i].Row << 8) | entries[i].Exponent);
                return hashCode;
            }
        }

        private class FactorBaseEntry
        {
            public int P { get; set; }
            public CountInt LogP { get; set; }
            public int Root { get; set; }
            public int RootDiff { get; set; }
            public FactorBaseEntry(int p, BigInteger n)
            {
                P = p;
                LogP = LogScale(p);
                Root = n % p == 0 ? 0 : IntegerMath.ModularSquareRoot(n, p);
                Debug.Assert(((BigInteger)Root * Root - n) % p == 0);
                RootDiff = ((P - Root) - Root) % p;
            }
            public override string ToString()
            {
                return string.Format("P = {0}", P);
            }
        }

        private struct LargePrimeEntry
        {
            public int P { get; set; }
            public CountInt LogP { get; set; }
            public int Offset1 { get; set; }
            public int Offset2 { get; set; }
        }

        private struct CountEntry
        {
            public int Index { get; set; }
            public CountInt Count { get; set; }
        }

        private class CountTable
        {
            int numberOfBlocks;
            private CountEntry[][] lists;
            private int[] used;
            private CountInt[] counts;
            public CountTable(int numberOfBlocks, int capacity, CountInt[] counts)
            {
                this.numberOfBlocks = numberOfBlocks;
                var listCapacity = capacity / numberOfBlocks;
                this.counts = counts;
                lists = new CountEntry[numberOfBlocks][];
                used = new int[numberOfBlocks];
                for (int j = 0; j < numberOfBlocks; j++)
                    lists[j] = new CountEntry[listCapacity];
            }
            public void AddEntry(int block, int index, CountInt count)
            {
                var list = lists[block];
                var slot = used[block]++;
                list[slot].Index = index;
                list[slot].Count = count;
            }
            public void AddToCounts(int block)
            {
                var list = lists[block];
                var length = used[block];
                for (int i = 0; i < length; i++)
                    counts[list[i].Index] += list[i].Count;
                used[block] = 0;
            }
        }

        private class Polynomial
        {
            public BigInteger A { get; set; }
            public BigInteger B { get; set; }
        }

        private class Relation
        {
            public BigInteger X { get; set; }
            public Polynomial Polynomial { get; set; }
            public ExponentEntries Entries { get; set; }
            public long Cofactor { get; set; }
            public int Exponent { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Interval
        {
            public int Id { get; set; }
            public int X { get; set; }
            public Polynomial Polynomial { get; set; }
            public int Size { get; set; }
            public byte[] Exponents { get; set; }
            public CountInt[] Cycle { get; set; }
            public int CycleOffset { get; set; }
            public CountInt[] Counts { get; set; }
            public CountTable CountTable { get; set; }
            public int[] Offsets1 { get; set; }
            public int[] Offsets2 { get; set; }
            public int[] Increments { get; set; }
            public int RelationsFound { get; set; }
            public int PartialRelationsFound { get; set; }
            public Siqs Siqs { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}, Size = {1}", X, Size);
            }
        }

        private struct PartialRelation
        {
            public Polynomial Polynomial { get; set; }
            public long X { get; set; }
            public ExponentEntries Entries { get; set; }
            public override string ToString()
            {
                return string.Format("X = {0}", X);
            }
        }

        private class Siqs
        {
            public int Index { get; set; }
            public int[] QMap { get; set; }
            public bool[] IsQIndex { get; set; }
            public int S { get; set; }
            public BigInteger[] CapB { get; set; }
            public int[][] Bainv2 { get; set; }
            public int[] Bainv2v { get; set; }
            public LargePrimeEntry[] LargePrimes { get; set; }
            public Polynomial Polynomial { get; set; }
            public int X { get; set; }
            public double Error { get; set; }
            public int[] Solution1 { get; set; }
            public int[] Solution2 { get; set; }
            public CountInt[] Threshold { get; set; }
        }

        private const int blockSizeDefault = 256 * 1024;
        private const int intervalSizeDefault = 256 * 1024;
        private const int maximumCycleLenth = 32 * 1024;
        private const int thresholdInterval = 1024;
        private const int thresholdShift = 10;
        private const double thresholdExponentDefault = 1.4;
        private const double errorLimitDefault = 0.1;
        private const int cofactorCutoffDefault = 4096;
        private const int surplusRelations = 10;
        private const int reportingIntervalDefault = 10;
        private const int maximumMultiplier = 73;
        private const int maximumScorePrimes = 100;
        private readonly BigInteger smallFactorCutoff = (BigInteger)int.MaxValue;
        private const int minimumAFactor = 2000;
        private const int maximumAfactor = 4000;
        private const int largePrimeOptimizationDigits = 80;

        private readonly Tuple<int, int>[] sizePairs =
        {
            Tuple.Create(1, 2),
            Tuple.Create(6, 5),
            Tuple.Create(10, 30),
            Tuple.Create(20, 60),
            Tuple.Create(30, 300),
            Tuple.Create(40, 900),
            Tuple.Create(50, 2500),
            Tuple.Create(60, 4000),
            Tuple.Create(70, 15000),
            Tuple.Create(80, 45000),
            Tuple.Create(90, 100000),
            Tuple.Create(100, 150000),

            // Untested.
            Tuple.Create(110, 300000),
            Tuple.Create(120, 600000),
        };

        private Config config;
        private IRandomNumberAlgorithm<uint> random;
        private IFactorizationAlgorithm<int> smallIntegerFactorer;
        private IEnumerable<int> allPrimes;
        private INullSpaceAlgorithm<IBitArray, IBitMatrix> solver;

        private Diag diag;
        private bool largePrimeOptimization;
        private bool useCountTable;
        private int sieveTimeLimit;
        private Algorithm algorithm;
        private int multiplier;
        private int[] multiplierFactors;
        private BigInteger nOrig;
        private BigInteger n;
        private BigInteger sqrtN;
        private int powerOfTwo;
        private int factorBaseSize;
        private int desired;
        private volatile bool sievingAborted;
        private int digits;
        private FactorBaseEntry[] factorBase;
        private int[] primes;
        private int maximumDivisor;
        private long maximumCofactorSize;
        private int mediumPrimeIndex;
        private int moderatePrimeIndex;
        private int largePrimeIndex;
        private double thresholdExponent;
        private int reportingInterval;
        private int cofactorCutoff;
        private double errorLimit;
        private int blockSize;
        private int intervalSize;
        private int[][] intervalIncrements;
        private int numberOfBlocks;
        private int blockShift;
        private int blockMask;

        private int[] candidateMap;
        private double[] candidateSizes;
        private int numberOfFactors;
        private double targetSize;

        private int threads;
        private Dictionary<ExponentEntries, Relation> relationBuffer;
        private Relation[] relations;
        private Dictionary<long, PartialRelation> partialRelations;
        private IBitMatrix matrix;
        private Stopwatch timer;

        private int cycleLength;

        private int intervalsProcessed;
        private int valuesChecked;
        private int partialRelationsProcessed;
        private int partialRelationsConverted;
        private int duplicateRelationsFound;
        private int duplicatePartialRelationsFound;

        private void FactorCore(BigInteger n, List<BigInteger> factors)
        {
            if (n == 1)
                return;
            if (IntegerMath.IsPrime(n))
            {
                factors.Add(n);
                return;
            }
            var divisor = GetDivisor(n);
            if (!divisor.IsZero)
            {
                FactorCore(divisor, factors);
                FactorCore(n / divisor, factors);
            }
        }

        private BigInteger GetDivisor(BigInteger nOrig)
        {
            if (nOrig.IsEven)
                return BigIntegers.Two;

            if ((diag & Diag.Timing) != 0)
            {
                timer = new Stopwatch();
                timer.Start();
            }

            Initialize(nOrig);

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Initialization: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("algorithm = {0}", algorithm);
                Console.WriteLine("digits = {0}; factorBaseSize = {1:N0}; desired = {2:N0}", digits, factorBaseSize, desired);
                Console.WriteLine("block size = {0:N0}; interval size = {1:N0}; threads = {2}", blockSize, intervalSize, threads);
                Console.WriteLine("error limit = {0}, cofactor cutoff = {1}; threshold exponent = {2}", errorLimit, cofactorCutoff, thresholdExponent);
                Console.WriteLine("first few factors: {0}", string.Join(", ", primes.Take(15)));
                Console.WriteLine("last few factors: {0}", string.Join(", ", primes.Skip(factorBaseSize - 5)));
                Console.WriteLine("small prime cycle length = {0}, last small prime = {1}", cycleLength, primes[mediumPrimeIndex - 1]);
                Console.WriteLine("multiplier = {0}, power of two = {1}", multiplier, powerOfTwo);
            }

            Sieve();

            if (relations.Length < desired)
                return BigInteger.Zero;

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Sieving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            if ((diag & Diag.Summary) != 0)
            {
                Console.WriteLine("intervals processed = {0:N0}; values processsed = {1:N0}; values checked = {2:N0}", intervalsProcessed, (long)intervalsProcessed * intervalSize, valuesChecked);
                Console.WriteLine("partial relations processed = {0:N0}; partial relations converted = {1:N0}", partialRelationsProcessed, partialRelationsConverted);
                Console.WriteLine("duplicate relations found = {0:N0}; duplicate partial relations found = {1:N0}", duplicateRelationsFound, duplicatePartialRelationsFound);
            }

            ProcessRelations();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Restart();
                Console.WriteLine("Processing relations: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            var result = Solve();

            if ((diag & Diag.Timing) != 0)
            {
                var elapsed = timer.ElapsedTicks;
                timer.Stop();
                Console.WriteLine("Solving: {0:F3} msec", 1000.0 * elapsed / Stopwatch.Frequency);
            }

            return result;
        }

        private void Initialize(BigInteger nOrig)
        {
            algorithm = config.Algorithm != Algorithm.None ? config.Algorithm : Algorithm.SelfInitializingQuadraticSieve;
            this.nOrig = nOrig;
            ChooseMultiplier();
            multiplierFactors = smallIntegerFactorer.Factor(multiplier).ToArray();
            n = nOrig * multiplier;
            sqrtN = IntegerMath.Sqrt(n);
            powerOfTwo = IntegerMath.Modulus(n, 8) == 1 ? 3 : IntegerMath.Modulus(n, 8) == 5 ? 2 : 1;
            digits = (int)Math.Ceiling(BigInteger.Log(n, 10));
            factorBaseSize = CalculateFactorBaseSize();
            factorBase = allPrimes
                .Where(p => p == 2 || multiplier % p == 0 || IntegerMath.JacobiSymbol(n, p) == 1)
                .Take(factorBaseSize)
                .Select(p => new FactorBaseEntry(p, n))
                .ToArray();
            primes = factorBase.Select(entry => entry.P).ToArray();
            desired = factorBaseSize + 1 + surplusRelations;
            maximumDivisor = factorBase[factorBaseSize - 1].P;
            long maximumDivisorSquared = (long)maximumDivisor * maximumDivisor;
            cofactorCutoff = config.CofactorCutoff != 0 ? config.CofactorCutoff : cofactorCutoffDefault;
            maximumCofactorSize = Math.Min((long)maximumDivisor * cofactorCutoff, maximumDivisorSquared);
            thresholdExponent = config.ThresholdExponent != 0 ? config.ThresholdExponent : thresholdExponentDefault;
            reportingInterval = config.ReportingInterval != 0 ? config.ReportingInterval : reportingIntervalDefault;
            errorLimit = config.ErrorLimit != 0 ? config.ErrorLimit : errorLimitDefault;

            intervalsProcessed = 0;
            valuesChecked = 0;
            partialRelationsProcessed = 0;
            partialRelationsConverted = 0;
            duplicateRelationsFound = 0;
            duplicatePartialRelationsFound = 0;

            CalculateNumberOfThreads();
            SetupIntervals();
            SetupSmallPrimeCycle();

            InitializeSiqs();

            moderatePrimeIndex = Enumerable.Range(0, factorBaseSize + 1)
                .Where(index => index == factorBaseSize ||
                    index >= mediumPrimeIndex && primes[index] >= intervalSize / 2)
                .First();
            largePrimeIndex = Enumerable.Range(0, factorBaseSize + 1)
                .Where(index => index == factorBaseSize ||
                    index >= moderatePrimeIndex && primes[index] >= intervalSize)
                .First();
        }

        private void ChooseMultiplier()
        {
            if (config.Multiplier != 0)
            {
                multiplier = config.Multiplier;
                return;
            }
            multiplier = Enumerable.Range(1, maximumMultiplier)
                .Where(value => IntegerMath.IsSquareFree(smallIntegerFactorer.Factor(value)))
                .OrderByDescending(value => ScoreMultiplier(value))
                .First();
        }

        private double ScoreMultiplier(int multiplier)
        {
            var n = nOrig * multiplier;
            var score = -0.5 * Math.Log(multiplier);
            var log2 = Math.Log(2);
            switch (IntegerMath.Modulus(n, 8))
            {
                case 1:
                    score += 2 * log2;
                    break;
                case 5:
                    score += log2;
                    break;
                case 3:
                case 7:
                    score += 0.5 * log2;
                    break;
                default:
                    break;
            }
            foreach (var p in allPrimes.Skip(1).Take(maximumScorePrimes))
            {
                if (n % p == 0 || IntegerMath.JacobiSymbol(n, p) == 1)
                {
                    var contribution = Math.Log(p) / (p - 1);
                    if (n % p == 0)
                        score += contribution;
                    else
                        score += 2 * contribution;
                }
            }
            return score;
        }

        private void InitializeSiqs()
        {
            // Choose minum and maximum factors of A so that M is near
            // the correct size for a multiple of those primes.
            var min = minimumAFactor;
            var max = maximumAfactor;
            if (min > maximumDivisor || max > maximumDivisor)
            {
                min = primes[factorBaseSize / 2];
                max = primes[factorBaseSize - 1];
            }
            var m = (intervalSize - 1) / 2;
            var logSqrt2N = BigInteger.Log(n * 2) / 2;
            targetSize = logSqrt2N - Math.Log(m);
            var preliminaryAverageSize = (Math.Log(min) + Math.Log(max)) / 2;
            var preliminaryNumberOfFactors = (int)Math.Ceiling(targetSize / preliminaryAverageSize);
            numberOfFactors = config.NumberOfFactors != 0 ? config.NumberOfFactors : preliminaryNumberOfFactors;
            var averageSize = targetSize / numberOfFactors;
            var center = Math.Exp(averageSize);
            var ratio = Math.Sqrt((double)max / min);
            min = (int)Math.Round(center / ratio);
            max = (int)Math.Round(center * ratio);

            candidateMap = Enumerable.Range(0, factorBaseSize)
                .Where(index => primes[index] >= min && primes[index] <= max)
                .ToArray();
            candidateSizes = candidateMap
                .Select(index => Math.Log(primes[index]))
                .ToArray();

            largePrimeOptimization = digits >= largePrimeOptimizationDigits;
            useCountTable = false;

            if ((diag & Diag.Summary) != 0)
                Console.WriteLine("number of factors of A = {0}, min = {1}, max = {2}", numberOfFactors, min, max);
        }

        private Siqs FirstPolynomial(Siqs siqs)
        {
            var s = numberOfFactors;
            var permutation = null as int[];
            var error = 0.0;
            while (true)
            {
                var numbers = random.Series((uint)candidateMap.Length)
                    .Take(candidateMap.Length)
                    .ToArray();
                permutation = Enumerable.Range(0, candidateMap.Length)
                    .OrderBy(index => numbers[index])
                    .Take(s)
                    .OrderBy(index => index)
                    .ToArray();

                error = permutation.Select(index => candidateSizes[index]).Sum() - targetSize;
                if (Math.Abs(error) < errorLimit)
                    break;
            }

            // Allocate and initialize one-time memory.
            if (siqs == null)
            {
                siqs = new Siqs
                {
                    IsQIndex = new bool[factorBaseSize],
                    Solution1 = new int[factorBaseSize],
                    Solution2 = new int[factorBaseSize],
                    Threshold = new CountInt[intervalSize >> thresholdShift],
                };
            }
            else
            {
                for (int i = 0; i < factorBaseSize; i++)
                    siqs.IsQIndex[i] = false;
            }
            if (siqs.Bainv2 == null || siqs.S != s)
            {
                var length = !largePrimeOptimization ? s - 1 : 2 * (s - 1);
                siqs.Bainv2 = new int[length][];
                for (int i = 0; i < length; i++)
                    siqs.Bainv2[i] = new int[factorBaseSize];
            }

            var qMap = permutation
                .Select(index => candidateMap[index])
                .ToArray();
            var q = qMap
                .Select(index => primes[index])
                .ToArray();
            var a = q.Select(p => (BigInteger)p).Product();
            var capB = new BigInteger[s];
            var b = BigInteger.Zero;
            for (int l = 0; l < s; l++)
            {
                var j = qMap[l];
                siqs.IsQIndex[j] = true;
                var r = q.Where(p => p != q[l]).ProductModulo(q[l]);
                var rInv = IntegerMath.ModularInverse(r, q[l]);
                Debug.Assert((long)r * rInv % q[l] == 1);
                var tSqrt = factorBase[j].Root;
                var gamma = (long)tSqrt * rInv % q[l];
                if (gamma > q[l] / 2)
                    gamma = q[l] - gamma;
                capB[l] = q.Where(p => p != q[l]).Select(p => (BigInteger)p).Product() * gamma;
                Debug.Assert((capB[l] * capB[l] - n) % q[l] == 0);
                b += capB[l];
            }
            b %= a;
            Debug.Assert((b * b - n) % a == 0);
            var polynomial = new Polynomial { A = a, B = b };


            var bainv2 = siqs.Bainv2;
            var soln1 = siqs.Solution1;
            var soln2 = siqs.Solution2;
            var x = -intervalSize / 2;
            for (int i = 0; i < factorBaseSize; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var entry = factorBase[i];
                var p = entry.P;
                var aInv = (long)IntegerMath.ModularInverse(a, p);
                Debug.Assert(a * aInv % p == 1);
                for (int l = 0; l < s - 1; l++)
                    bainv2[l][i] = (int)(2 * (long)(capB[l] % p) * aInv % p);
                if (largePrimeOptimization)
                {
                    for (int l = 0; l < s - 1; l++)
                        bainv2[l + s - 1][i] = (p - bainv2[l][i]) % p;
                }
                var root1 = (int)((entry.Root - b) % p);
                if (root1 < 0)
                    root1 += p;
                var root2 = root1 + entry.RootDiff;
                soln1[i] = (int)((aInv * root1 - x) % p);
                soln2[i] = (int)((aInv * root2 - x) % p);
                Debug.Assert(soln1[i] >= 0 && EvaluatePolynomial(polynomial, x + soln1[i]) % p == 0);
                Debug.Assert(soln1[i] >= 0 && EvaluatePolynomial(polynomial, x + soln2[i]) % p == 0);
            }

            var threshold = siqs.Threshold;
            var logSqrtN = BigInteger.Log(n, 2) / 2;
            var denominator = thresholdExponent * Math.Log(maximumDivisor, 2);
            var m = intervalSize / 2;
            for (int k = 0; k < intervalSize; k += thresholdInterval)
            {
                var y = EvaluatePolynomial(polynomial, x + k + thresholdInterval / 2);
                var logY = BigInteger.Log(BigInteger.Abs(y), 2);
                threshold[k >> thresholdShift] = (CountInt)Math.Round(logY - denominator);
            }

            if (largePrimeOptimization)
            {
                var l = new LargePrimeEntry[factorBaseSize - largePrimeIndex];
                for (int i = largePrimeIndex; i < factorBaseSize; i++)
                {
                    var j = i - largePrimeIndex;
                    var entry = factorBase[i];
                    l[j].P = entry.P;
                    l[j].LogP = entry.LogP;
                    l[j].Offset1 = soln1[i];
                    l[j].Offset2 = soln2[i];
                }
                siqs.LargePrimes = l;
            }

            siqs.Index = 0;
            siqs.QMap = qMap;
            siqs.S = s;
            siqs.CapB = capB;
            siqs.Polynomial = polynomial;
            siqs.X = x;
            siqs.Bainv2v = null;
            siqs.Error = error;

            return siqs;
        }

        private Siqs ChangePolynomial(Siqs siqs)
        {
            if (siqs == null || siqs.Index == (1 << (siqs.S - 1)) - 1)
                return FirstPolynomial(siqs);

            int index = siqs.Index;
            var a = siqs.Polynomial.A;
            var b = siqs.Polynomial.B;
            var capB = siqs.CapB;

            // Advance index; calculate v & e.
            var v = 0;
            int ii = index + 1;
            while ((ii & 1) == 0)
            {
                ++v;
                ii >>= 1;
            }
            var e = (ii & 2) == 0 ? -1 : 1;

            // Advance b and record new polynomial.
            b += 2 * e * capB[v];
            var polynomial = new Polynomial { A = a, B = b };
            Debug.Assert((b * b - n) % a == 0);

            // Calculate new offsets.
            var m = intervalSize;
            var x = -m / 2;
            var soln1 = siqs.Solution1;
            var soln2 = siqs.Solution2;
            if (!largePrimeOptimization)
            {
                var bainv2v = siqs.Bainv2[v];
                for (int i = 0; i < factorBaseSize; i++)
                {
                    if (siqs.IsQIndex[i])
                        continue;
                    var p = primes[i];
                    var step = bainv2v[i];
                    if (e == 1)
                        step = p - step;
                    var s1 = soln1[i] + step;
                    if (s1 >= p)
                        s1 -= p;
                    soln1[i] = s1;
                    Debug.Assert(s1 >= 0 && s1 < p && EvaluatePolynomial(polynomial, x + s1) % p == 0);
                    var s2 = soln2[i] + step;
                    if (s2 >= p)
                        s2 -= p;
                    soln2[i] = s2;
                    Debug.Assert(s2 >= 0 && s2 < p && EvaluatePolynomial(polynomial, x + s2) % p == 0);
                }
            }
            else
            {
                var bainv2v = siqs.Bainv2[v + (e == -1 ? 0 : siqs.S - 1)];
                for (int i = 0; i < largePrimeIndex; i++)
                {
                    if (siqs.IsQIndex[i])
                        continue;
                    var p = primes[i];
                    var step = bainv2v[i];
                    var s1 = soln1[i] + step;
                    if (s1 >= p)
                        s1 -= p;
                    soln1[i] = s1;
                    Debug.Assert(s1 >= 0 && s1 < p && EvaluatePolynomial(polynomial, x + s1) % p == 0);
                    var s2 = soln2[i] + step;
                    if (s2 >= p)
                        s2 -= p;
                    soln2[i] = s2;
                    Debug.Assert(s2 >= 0 && s2 < p && EvaluatePolynomial(polynomial, x + s2) % p == 0);
                }
                siqs.Bainv2v = bainv2v;
            }

            // Update siqs.
            siqs.Index = index + 1;
            siqs.Polynomial = polynomial;

            return siqs;
        }

        private BigInteger Solve()
        {
            if ((diag & Diag.SaveMatrix) != 0)
            {
                using (var stream = new StreamWriter(File.OpenWrite("matrix.txt")))
                {
                    stream.WriteLine("{0} {1}", matrix.Rows, matrix.Cols);
                    for (int i = 0; i < matrix.Rows; i++)
                        stream.WriteLine(string.Join(" ", matrix.GetNonZeroCols(i)));
                }
            }

            if ((diag & Diag.Solving) == 0)
            {
                return solver.Solve(matrix)
                    .Select(v => ComputeFactor(v))
                    .Where(factor => !factor.IsZero)
                    .FirstOrDefault();
            }

            var timer = new Stopwatch();
            timer.Start();
            var solutions = solver.Solve(matrix).GetEnumerator();
            var next = solutions.MoveNext();
            var elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
            Console.WriteLine("first solution: {0:F3} msec", elapsed);
            if (!next)
            {
                Console.WriteLine("no solutions!");
                return BigInteger.Zero;
            }
            do
            {
                var v = solutions.Current;
                if ((diag & Diag.Solutions) != 0)
                    Console.WriteLine("v = {0}", string.Join(", ", v.GetNonZeroIndices().ToArray()));
                int numberOfIndices = v.GetNonZeroIndices().Count();
                timer.Restart();
                var factor = ComputeFactor(v);
                elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency * 1000;
                Console.WriteLine("compute factor: {0:F3} msec ({1} indices)", elapsed, numberOfIndices);
                if (!factor.IsZero)
                    return factor;
            } while (solutions.MoveNext());
            Console.WriteLine("failed!");
            return BigInteger.Zero;
        }

        private void CalculateNumberOfThreads()
        {
            threads = config.Threads != 0 ? config.Threads : 1;
            if (digits < 10)
                threads = 1;
        }

        private int CalculateFactorBaseSize()
        {
            if (config.FactorBaseSize != 0)
                return config.FactorBaseSize;
            for (int i = 0; i < sizePairs.Length - 1; i++)
            {
                var pair = sizePairs[i];
                if (digits >= sizePairs[i].Item1 && digits <= sizePairs[i + 1].Item1)
                {
                    // Interpolate.
                    double x0 = sizePairs[i].Item1;
                    double y0 = sizePairs[i].Item2;
                    double x1 = sizePairs[i + 1].Item1;
                    double y1 = sizePairs[i + 1].Item2;
                    double x = y0 + (digits - x0) * (y1 - y0) / (x1 - x0);
                    return (int)Math.Ceiling(x);
                }
            }
            throw new InvalidOperationException("table entry not found");
        }

        private BigInteger ComputeFactor(IBitArray v)
        {
            var indices = v.GetNonZeroIndices().ToArray();
            Debug.Assert(indices
                .Select(index => relations[index])
                .All(relation => (relation.X * relation.X - MultiplyFactors(relation)) % n == 0));
            var xPrime = indices
                .Select(index => relations[index].X)
                .ProductModulo(n);
            var exponents = SumExponents(indices);
            var yFactorBase = new[] { -1 }
                .Concat(factorBase.Select(entry => entry.P))
                .Zip(exponents, (p, exponent) => BigInteger.ModPow(p, exponent, n));
            var yCofactors = indices
                .Select(index => (BigInteger)relations[index].Cofactor)
                .Where(cofactor => cofactor != 1);
            var yPrime = yFactorBase
                .Concat(yCofactors)
                .ProductModulo(n);
            var factor = BigInteger.GreatestCommonDivisor(xPrime + yPrime, n);
            foreach (var multiplierFactor in multiplierFactors)
            {
                if (factor % multiplierFactor == 0)
                    factor /= multiplierFactor;
            }
            return !factor.IsOne && factor != nOrig ? factor : BigInteger.Zero;
        }

        private int[] SumExponents(IEnumerable<int> indices)
        {
            var results = new int[factorBaseSize + 1];
            foreach (int index in indices)
            {
                foreach (var entry in relations[index].Entries)
                    results[entry.Row] += entry.Exponent;
            }
            Debug.Assert(results.All(exponent => exponent % 2 == 0));
            for (int i = 0; i < results.Length; i++)
                results[i] /= 2;
            return results;
        }

        private static CountInt LogScale(int n)
        {
            return (CountInt)Math.Round(Math.Log(Math.Abs(n), 2));
        }

        private static CountInt LogScale(long n)
        {
            return (CountInt)Math.Round(Math.Log(Math.Abs(n), 2));
        }

        private static CountInt LogScale(BigInteger n)
        {
            return (CountInt)Math.Round(BigInteger.Log(BigInteger.Abs(n), 2));
        }

        private void Sieve()
        {
            sievingAborted = false;
            relationBuffer = new Dictionary<ExponentEntries, Relation>();
            partialRelations = new Dictionary<long, PartialRelation>();

            if (threads == 1)
                SieveTask();
            else
            {
                var tasks = new Task[threads];
                for (int i = 0; i < threads; i++)
                    tasks[i] = Task.Factory.StartNew(SieveTask);
                WaitForTasks(tasks);
            }

            relations = relationBuffer.Values.ToArray();
            relationBuffer = null;
            partialRelations = null;
        }

        private bool SievingCompleted
        {
            get { return sievingAborted || relationBuffer.Count >= desired; }
        }

        private void SieveTask()
        {
            var interval = CreateInterval();
            while (!SievingCompleted)
            {
                Sieve(interval);
                Interlocked.Increment(ref intervalsProcessed);
            }
        }

        private void WaitForTasks(Task[] tasks)
        {
            if ((diag & Diag.Sieve) == 0 && sieveTimeLimit == 0)
            {
                Task.WaitAll(tasks);
                return;
            }

            var timer = new Stopwatch();
            timer.Start();
            double percentCompleteSofar = 0;
            int totalTime = 0;
            while (!Task.WaitAll(tasks, reportingInterval * 1000))
            {
                int current = relationBuffer.Count;
                double percentComplete = (double)current / desired * 100;
                double percentLatest = percentComplete - percentCompleteSofar;
                double percentRemaining = 100 - percentComplete;
                double percentRate = (double)percentLatest / reportingInterval;
                double timeRemainingSeconds = percentRate == 0 ? 0 : percentRemaining / percentRate;
                var timeRemaining = TimeSpan.FromSeconds(Math.Ceiling(timeRemainingSeconds));
                if ((diag & Diag.Sieve) != 0)
                {
                    Console.WriteLine("{0:F3}% complete, rate = {1:F6} %/sec, sieve time remaining = {2}",
                        percentComplete, percentRate, timeRemaining);
                }
                percentCompleteSofar = percentComplete;
                totalTime += reportingInterval;
                if (sieveTimeLimit != 0 && totalTime >= sieveTimeLimit)
                {
                    sievingAborted = true;
                    Task.WaitAll(tasks);
                    break;
                }
            }
            double elapsed = (double)timer.ElapsedTicks / Stopwatch.Frequency;
            double overallPercentRate = (double)relationBuffer.Count / desired * 100 / elapsed;
            Console.WriteLine("overall rate = {0:F6} %/sec", overallPercentRate);
        }

        private Interval CreateInterval()
        {
            var interval = new Interval();
            interval.Exponents = new byte[factorBaseSize + 1];
            interval.Cycle = new CountInt[cycleLength];
            interval.Counts = new CountInt[Math.Min(intervalSize, blockSize) + 1];
            interval.CountTable = new CountTable(numberOfBlocks, intervalSize, interval.Counts);
            interval.Offsets1 = new int[factorBaseSize];
            interval.Offsets2 = new int[factorBaseSize];
            return interval;
        }

        private void SetupIntervals()
        {
            blockSize = config.BlockSize != 0 ? config.BlockSize : blockSizeDefault;
            intervalSize = config.IntervalSize != 0 ? config.IntervalSize : intervalSizeDefault;
            int numberOfIncrements = intervalSize / blockSize;
            intervalIncrements = new int[numberOfIncrements + 1][];
            for (int j = 1; j < numberOfIncrements; j++)
            {
                intervalIncrements[j] = new int[factorBaseSize];
                var offset = -blockSize * j;
                for (int i = 0; i < factorBaseSize; i++)
                {
                    var p = primes[i];
                    intervalIncrements[j][i] = (offset % p + p) % p;
                }
            }
            Debug.Assert(intervalSize % blockSize == 0);
            numberOfBlocks = intervalSize / blockSize;
            blockShift = (intervalSize / numberOfBlocks).GetBitLength() - 1;
            blockMask = intervalSize / numberOfBlocks - 1;
        }

        private Interval GetNextInterval(Interval interval)
        {
            if ((diag & Diag.Polynomials) != 0 && interval.Siqs != null && interval.Siqs.Index == (1 << (interval.Siqs.S - 1)) - 1)
            {
                Console.WriteLine("polynomial results: relations found = {0}, partial relations found = {1}, error = {2:F3}",
                    interval.RelationsFound, interval.PartialRelationsFound, interval.Siqs.Error);
                interval.RelationsFound = 0;
                interval.PartialRelationsFound = 0;
            }
            interval.Siqs = ChangePolynomial(interval.Siqs);
            if ((diag & Diag.Polynomials) != 0 && interval.Siqs.Index == 0)
                Console.WriteLine("A = {0}", interval.Siqs.Polynomial.A);
            var x = -intervalSize / 2;
            interval.X = x;
            interval.Polynomial = interval.Siqs.Polynomial;
            interval.Size = intervalSize;
            interval.Offsets1 = interval.Siqs.Solution1;
            interval.Offsets2 = interval.Siqs.Solution2;
            return interval;
        }

        private void SetupSmallPrimeCycle()
        {
            int c = 1;
            int i = 0;
            while (i < factorBaseSize && c * factorBase[i].P < maximumCycleLenth)
            {
                var p = primes[i];
                c *= p;
                ++i;
            }
            mediumPrimeIndex = i;
            cycleLength = c;
        }

        private void Sieve(Interval interval)
        {
            GetNextInterval(interval);
            int intervalSize = interval.Size;
            SetupLargePrimesSieving(interval, blockSize);
            for (int k0 = 0; k0 < intervalSize; k0 += blockSize)
            {
                int size = Math.Min(blockSize, intervalSize - k0);
                interval.Increments = intervalIncrements[k0 / blockSize];
                SieveSmallPrimes(interval, k0, size);
                SieveMediumPrimes(interval, k0, size);
                SieveModeratePrimes(interval, k0, size);
                SieveLargePrimes(interval, k0, size);
                CheckForSmooth(interval, k0, size);
                if (SievingCompleted)
                    break;
            }
        }

        private void SieveSmallPrimes(Interval interval, int k0, int size)
        {
            var cycle = interval.Cycle;
            var counts = interval.Counts;

            if (k0 == 0)
                InitializeSmallPrimeCycle(interval);

            // Initialize the remainder of the counts array with the cycle.
            int k = interval.CycleOffset;
            Array.Copy(cycle, cycleLength - k, counts, 0, Math.Min(k, size));
            while (k < size)
            {
                Array.Copy(cycle, 0, counts, k, Math.Min(cycleLength, size - k));
                k += cycleLength;
            }
            interval.CycleOffset = k - size;
        }

        private void InitializeSmallPrimeCycle(Interval interval)
        {
            var cycle = interval.Cycle;
            var counts = interval.Counts;
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            var log2 = factorBase[0].LogP;
            var count1 = (CountInt)(log2 * powerOfTwo);
            var count2 = (CountInt)(log2 * powerOfTwo);
            if (offsets1[0] == 1)
                count1 = 0;
            else
                count2 = 0;
            int k;
            Debug.Assert(Enumerable.Range(0, cycleLength)
                .All(k2 => k2 % 2 != offsets1[0] ||
                    EvaluatePolynomial(interval.Polynomial, interval.X + k2) % IntegerMath.Pow(2, powerOfTwo) == 0));
            for (k = 0; k < cycleLength; k += 2)
            {
                cycle[k] = count1;
                Debug.Assert(count1 == 0 || EvaluatePolynomial(interval.Polynomial, interval.X + k) % 2 == 0);
                cycle[k + 1] = count2;
                Debug.Assert(count2 == 0 || EvaluatePolynomial(interval.Polynomial, interval.X + k + 1) % 2 == 0);
            }
            for (int i = 1; i < mediumPrimeIndex; i++)
            {
                var entry = factorBase[i];
                var p = entry.P;
                var logP = entry.LogP;
                var offset1 = offsets1[i];
                var offset2 = offsets2[i];
                for (k = 0; k < cycleLength; k += p)
                {
                    cycle[k + offset1] += logP;
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k + offset1) % p == 0);
                    if (offset1 == offset2)
                        continue;
                    cycle[k + offset2] += logP;
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k + offset2) % p == 0);
                }
            }
            interval.CycleOffset = 0;
        }

        private void SieveMediumPrimes(Interval interval, int k0, int size)
        {
            var siqs = interval.Siqs;
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            var counts = interval.Counts;
            var increments = interval.Increments;
            for (int i = mediumPrimeIndex; i < moderatePrimeIndex; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var entry = factorBase[i];
                int p = entry.P;
                var logP = entry.LogP;
                int k1 = offsets1[i];
                int k2 = offsets2[i];
                Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k1) % p == 0);
                Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k2) % p == 0);
                if (k0 != 0)
                {
                    var increment = increments[i];
                    k1 += increment;
                    if (k1 >= p)
                        k1 -= p;
                    k2 += increment;
                    if (k2 >= p)
                        k2 -= p;
                }
                if (k1 < k2)
                {
                    var kDiff = k2 - k1;
                    int kLimit = size - kDiff;
                    while (k1 < kLimit)
                    {
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k1) % p == 0);
                        counts[k1] += logP;
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k1 + kDiff) % p == 0);
                        counts[k1 + kDiff] += logP;
                        k1 += p;
                    }
                    if (k1 < size)
                    {
                        counts[k1] += logP;
                        k1 += p;
                    }
                }
                else if (k2 < k1)
                {
                    var kDiff = k1 - k2;
                    int kLimit = size - kDiff;
                    while (k2 < kLimit)
                    {
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k2) % p == 0);
                        counts[k2] += logP;
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k2 + kDiff) % p == 0);
                        counts[k2 + kDiff] += logP;
                        k2 += p;
                    }
                    if (k2 < size)
                    {
                        counts[k2] += logP;
                        k2 += p;
                    }
                }
                else
                {
                    while (k1 < size)
                    {
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k1) % p == 0);
                        counts[k1] += logP;
                        k1 += p;
                    }
                }
            }
        }

        private void SieveModeratePrimes(Interval interval, int k0, int size)
        {
            var siqs = interval.Siqs;
            var offsets1 = interval.Offsets1;
            var offsets2 = interval.Offsets2;
            var counts = interval.Counts;
            var increments = interval.Increments;
            for (int i = moderatePrimeIndex; i < largePrimeIndex; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var entry = factorBase[i];
                int p = entry.P;
                var logP = entry.LogP;
                int k1 = offsets1[i];
                int k2 = offsets2[i];
                if (k0 != 0)
                {
                    var increment = increments[i];
                    k1 += increment;
                    if (k1 >= p)
                        k1 -= p;
                    k2 += increment;
                    if (k2 >= p)
                        k2 -= p;
                }
                while (k1 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k1) % p == 0);
                    counts[k1] += logP;
                    k1 += p;
                }
                if (entry.RootDiff == 0)
                {
                    offsets2[i] = offsets1[i];
                    continue;
                }
                while (k2 < size)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k2) % p == 0);
                    counts[k2] += logP;
                    k2 += p;
                }
            }
        }

        private void SetupLargePrimesSieving(Interval interval, int size)
        {
            if (!largePrimeOptimization || !useCountTable)
                return;
            var counts = interval.Counts;
            var l = interval.Siqs.LargePrimes;
            var bainv2v = interval.Siqs.Bainv2v;
            int intervalSize = interval.Size;
            var countTable = interval.CountTable;
            for (int i = largePrimeIndex; i < factorBaseSize; i++)
            {
                var j = i - largePrimeIndex;
                var p = l[j].P;
                var logP = l[j].LogP;
                var step = bainv2v != null ? bainv2v[i] : 0;
                int k1 = l[j].Offset1 + step;
                if (k1 >= p)
                    k1 -= p;
                if (k1 < intervalSize)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k1) % p == 0);
                    countTable.AddEntry(k1 >> blockShift, k1 & blockMask, logP);
                }
                l[j].Offset1 = k1;
                int k2 = l[j].Offset2 + step;
                if (k2 >= p)
                    k2 -= p;
                if (k2 < intervalSize)
                {
                    Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k2) % p == 0);
                    countTable.AddEntry(k2 >> blockShift, k2 & blockMask, logP);
                }
                l[j].Offset2 = k2;
            }
        }

        private void SieveLargePrimes(Interval interval, int k0, int size)
        {
            if (largePrimeOptimization)
            {
                if (useCountTable)
                {
                    interval.CountTable.AddToCounts(k0 / size);
                    return;
                }
                var counts = interval.Counts;
                var l = interval.Siqs.LargePrimes;
                var bainv2v = interval.Siqs.Bainv2v;
                int intervalSize = interval.Size;
                if (bainv2v == null || k0 != 0)
                {
                    var increments = interval.Increments;
                    for (int i = largePrimeIndex; i < factorBaseSize; i++)
                    {
                        var j = i - largePrimeIndex;
                        var p = l[j].P;
                        var logP = l[j].LogP;
                        int k1 = l[j].Offset1;
                        int k2 = l[j].Offset2;
                        if (k0 != 0)
                        {
                            var increment = increments[i];
                            k1 += increment;
                            if (k1 >= p)
                                k1 -= p;
                            k2 += increment;
                            if (k2 >= p)
                                k2 -= p;
                        }
                        if (k1 < intervalSize)
                        {
                            Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k1) % p == 0);
                            counts[k1] += logP;
                        }
                        if (k2 < intervalSize)
                        {
                            Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k2) % p == 0);
                            counts[k2] += logP;
                        }
                    }
                }
                else
                {
                    for (int i = largePrimeIndex; i < factorBaseSize; i++)
                    {
                        var j = i - largePrimeIndex;
                        var p = l[j].P;
                        var logP = l[j].LogP;
                        var step = bainv2v[i];
                        int k1 = l[j].Offset1 + step;
                        if (k1 >= p)
                            k1 -= p;
                        if (k1 < intervalSize)
                        {
                            Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k1) % p == 0);
                            counts[k1] += logP;
                        }
                        l[j].Offset1 = k1;
                        int k2 = l[j].Offset2 + step;
                        if (k2 >= p)
                            k2 -= p;
                        if (k2 < intervalSize)
                        {
                            Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k2) % p == 0);
                            counts[k2] += logP;
                        }
                        l[j].Offset2 = k2;
                    }
                }
            }
            else
            {
                var siqs = interval.Siqs;
                var offsets1 = interval.Offsets1;
                var offsets2 = interval.Offsets2;
                var counts = interval.Counts;
                var increments = interval.Increments;
                for (int i = largePrimeIndex; i < factorBaseSize; i++)
                {
                    var entry = factorBase[i];
                    int p = entry.P;
                    var logP = entry.LogP;
                    int k1 = offsets1[i];
                    int k2 = offsets2[i];
                    if (k0 != 0)
                    {
                        var increment = increments[i];
                        k1 += increment;
                        if (k1 >= p)
                            k1 -= p;
                        k2 += increment;
                        if (k2 >= p)
                            k2 -= p;
                    }
                    if (k1 < size)
                    {
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k1) % p == 0);
                        counts[k1] += logP;
                    }
                    if (k2 < size)
                    {
                        Debug.Assert(EvaluatePolynomial(interval.Polynomial, interval.X + k0 + k2) % p == 0);
                        counts[k2] += logP;
                    }
                }
            }
        }

        private void CheckForSmooth(Interval interval, int k0, int size)
        {
            var counts = interval.Counts;
            var threshold = interval.Siqs.Threshold;
            counts[size] = CountInt.MaxValue;
            for (int k = 0; k < size; k += thresholdInterval)
            {
                var limit = threshold[(k0 + k) >> thresholdShift];
                int jMax = k + thresholdInterval;
                for (int j = k; j < jMax; j++)
                {
                    if (counts[j] >= limit)
                    {
                        CheckValue(interval, k0 + j);
                        if (SievingCompleted)
                            return;
                    }
                }
            }
        }

        private void CheckValue(Interval interval, int k)
        {
            Interlocked.Increment(ref valuesChecked);
            ClearExponents(interval);
            long cofactor = FactorOverBase(interval, interval.X + k);
            if (cofactor == 0)
                return;
            if (cofactor == 1)
            {
                var relation = CreateRelation(
                    EvaluateMapping(interval.Polynomial, interval.X + k),
                    interval.Polynomial,
                    GetEntries(interval.Exponents),
                    1, 0);
                ++interval.RelationsFound;
                AddRelation(relation);
                return;
            }
            if (cofactor < maximumCofactorSize)
            {
                ++interval.PartialRelationsFound;
                ProcessPartialRelation(interval, k, cofactor);
            }
        }

        private long FactorOverBase(Interval interval, long x)
        {
            var y = EvaluatePolynomial(interval.Polynomial, x);
            var exponents = interval.Exponents;
            var siqs = interval.Siqs;
            if (y < 0)
            {
                ++exponents[0];
                y = -y;
            }
            while (y.IsEven)
            {
                ++exponents[1];
                y >>= 1;
            }
            for (int i = 0; i < siqs.S; i++)
            {
                var j = siqs.QMap[i];
                var q = factorBase[j].P;
                ++exponents[j + 1];
                while (y % q == 0)
                {
                    ++exponents[j + 1];
                    y /= q;
                }
            }
            return FactorOverBase(interval, y, (int)(x - siqs.X));
        }

        private long FactorOverBase(Interval interval, BigInteger y, int delta)
        {
            var siqs = interval.Siqs;
            var offsets1 = siqs.Solution1;
            var offsets2 = siqs.Solution2;
            var exponents = interval.Exponents;
            for (int i = 1; i < largePrimeIndex; i++)
            {
                if (siqs.IsQIndex[i])
                    continue;
                var p = primes[i];
                var offset = delta % p;
                if (offset != offsets1[i] && offset != offsets2[i])
                {
                    Debug.Assert(y % p != 0);
                    continue;
                }
                Debug.Assert(y % p == 0);
                while ((y % p).IsZero)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            if (!largePrimeOptimization)
            {
                for (int i = largePrimeIndex; i < factorBaseSize; i++)
                {
                    if (delta != offsets1[i] && delta != offsets2[i])
                    {
                        Debug.Assert(y % primes[i] != 0);
                        continue;
                    }
                    var p = primes[i];
                    Debug.Assert(y % p == 0);
                    while ((y % p).IsZero)
                    {
                        ++exponents[i + 1];
                        y /= p;
                    }
                }
            }
            else
            {
                var l = siqs.LargePrimes;
                for (int i = largePrimeIndex; i < factorBaseSize; i++)
                {
                    if (delta != l[i - largePrimeIndex].Offset1 && delta != l[i - largePrimeIndex].Offset2)
                    {
                        Debug.Assert(y % primes[i] != 0);
                        continue;
                    }
                    var p = primes[i];
                    Debug.Assert(y % p == 0);
                    while ((y % p).IsZero)
                    {
                        ++exponents[i + 1];
                        y /= p;
                    }
                }
            }
            return y < long.MaxValue ? (long)y : 0;
        }

        private long FactorOverBase(byte[] exponents, Polynomial polynomial, long x)
        {
            var y = polynomial.A * EvaluatePolynomial(polynomial, x);
            if (y < 0)
            {
                ++exponents[0];
                y = -y;
            }
            while (y.IsEven)
            {
                ++exponents[1];
                y >>= 1;
            }
            for (int i = 1; i < factorBaseSize; i++)
            {
                var p = primes[i];
                while (y % p == 0)
                {
                    ++exponents[i + 1];
                    y /= p;
                }
            }
            return y < long.MaxValue ? (long)y : 0;
        }


        private void ProcessPartialRelation(Interval interval, int k, long cofactor)
        {
            Interlocked.Increment(ref partialRelationsProcessed);
            var partialRelation = new PartialRelation
            {
                X = interval.X + k,
                Polynomial = interval.Polynomial,
                Entries = GetEntries(interval.Exponents),
            };
            PartialRelation other;
            while (true)
            {
                lock (partialRelations)
                {
                    if (partialRelations.TryGetValue(cofactor, out other))
                        partialRelations.Remove(cofactor);
                    else
                        partialRelations.Add(cofactor, partialRelation);
                }
                if (other.Polynomial == null)
                    return;
                if (!other.Entries.Equals(partialRelation.Entries))
                    break;
                Interlocked.Increment(ref duplicatePartialRelationsFound);
            }
            Interlocked.Increment(ref partialRelationsConverted);
            if (other.Entries != null)
                AddEntries(interval.Exponents, other.Entries);
            else
                FactorOverBase(interval.Exponents, other.Polynomial, other.X);
            var relation = CreateRelation(
                EvaluateMapping(interval.Polynomial, interval.X + k) * EvaluateMapping(other.Polynomial, other.X),
                null,
                GetEntries(interval.Exponents),
                cofactor, 2);
            AddRelation(relation);
        }

        private void AddEntries(byte[] exponents, ExponentEntries entries)
        {
            for (int i = 0; i < entries.Count; i++)
                exponents[entries[i].Row] += (byte)entries[i].Exponent;
        }

        private Relation CreateRelation(BigInteger x, Polynomial polynomial, ExponentEntries entries, long cofactor, int exponent)
        {
            var relation = new Relation
            {
                X = x,
                Polynomial = polynomial,
                Entries = entries,
                Cofactor = cofactor,
                Exponent = exponent,
            };
            Debug.Assert((relation.X * relation.X - MultiplyFactors(relation)) % n == 0);
            return relation;
        }

        private void AddRelation(Relation relation)
        {
            lock (relationBuffer)
            {
                if (relationBuffer.ContainsKey(relation.Entries))
                {
                    Interlocked.Increment(ref duplicateRelationsFound);
                    return;
                }
                relationBuffer.Add(relation.Entries, relation);
            }
        }

        private void ProcessRelations()
        {
            Debug.Assert(relations.GroupBy(relation => relation.X).Count() == relations.Length);
            matrix = new BitMatrix(factorBaseSize + 1, relations.Length);
            for (int j = 0; j < relations.Length; j++)
            {
                var entries = relations[j].Entries;
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    if (entry.Exponent % 2 != 0)
                        matrix[entry.Row, j] = true;
                }
            }
        }

        private void ClearExponents(Interval interval)
        {
            var exponents = interval.Exponents;
            for (int i = 0; i <= factorBaseSize; i++)
                exponents[i] = 0;
        }

        private ExponentEntries GetEntries(byte[] exponents)
        {
            int size = 16;
            var entries = new ExponentEntry[size];
            int k = 0;
            for (int i = 0; i < exponents.Length; i++)
            {
                if (exponents[i] != 0)
                {
                    entries[k++] = new ExponentEntry { Row = i, Exponent = exponents[i] };
                    if (k == size)
                    {
                        size *= 2;
                        Array.Resize(ref entries, size);
                    }
                }
            }
            Array.Resize(ref entries, k);
            return new ExponentEntries(entries);
        }

        private BigInteger EvaluateMapping(Polynomial polynomial, long x)
        {
            return polynomial.A * x + polynomial.B;
        }

        private BigInteger EvaluatePolynomial(Polynomial polynomial, long x)
        {
            var xPrime = EvaluateMapping(polynomial, x);
            var yPrime = xPrime * xPrime - n;
            Debug.Assert(yPrime % polynomial.A == 0);
            return yPrime / polynomial.A;
        }

        private BigInteger MultiplyFactors(Relation relation)
        {
            var result = relation.Entries
                .Select(entry => BigInteger.Pow(entry.Row == 0 ? -1 : factorBase[entry.Row - 1].P, entry.Exponent))
                .ProductModulo(n);
            return result * BigInteger.ModPow(relation.Cofactor, relation.Exponent, n) % n;
        }
    }
}
