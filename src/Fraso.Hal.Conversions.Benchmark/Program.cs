using BenchmarkDotNet.Running;
using System;

namespace Fraso.Hal.Conversions.Benchmark
{
    internal class Program
    {
        public static void Main(string[] args)
            => BenchmarkRunner.Run<BasicWrapping>();
    }
}
