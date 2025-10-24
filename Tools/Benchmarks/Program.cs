#region
using BenchmarkDotNet.Running;
#endregion

namespace Benchmarks;

public static class Program
{
    public static void Main()
        =>

            // Uncomment the benchmark you want to run:
            BenchmarkRunner.Run<PointExtensionsBenchmarks>();

    //BenchmarkRunner.Run<QuadTreeBenchmarks>();
    //BenchmarkRunner.Run<GeometryBenchmarks>();
    //BenchmarkRunner.Run<DictionaryEnumerationBenchmarks>();
    //BenchmarkRunner.Run<DictionaryLookupBenchmarks>();
    //BenchmarkRunner.Run<ForEachBenchmark>();
    //BenchmarkRunner.Run<FuzzySearchBenchmarks>();
    //BenchmarkRunner.Run<LocalFunctionBenchmarks>();
    //BenchmarkRunner.Run<LockBenchmarks>();
    //BenchmarkRunner.Run<ShuffleBenchmarks>();
    //BenchmarkRunner.Run<SpanBenchmarks>();
    //BenchmarkRunner.Run<StringProcessorBenchmarks>();
}