using BenchmarkDotNet.Attributes;
using Fraso.Hal.Primitives;
using System.Linq;

namespace Fraso.Hal.Conversions.Benchmark
{
    public class BasicWrapping
    {
        #region Fields
        private ToWrap[] Data;

        private readonly WrapPolicy<ToWrap> Policy
            = WrapPolicy
            .For<ToWrap>()
            .Property(i => i.Text);
        #endregion //Fields

        [Params(1, 10, 100, 1000, 10000, 100000)]
        public int Size;

        [GlobalSetup]
        public void Setup()
        {
            Data = new ToWrap[Size];

            for (int i = 0; i < Size; i++)
            {
                Data[i]
                    = new ToWrap()
                    {
                        Text = "foobar"
                    };
            }
        }

        [Benchmark(Baseline = true)]
        public Resource[] SimpleAssign()
        {
            var arr = new Resource[Size];

            for (int i = 0; i < Size; i++)
            {
                var resource = new Resource();
                resource["Text"] = Data[i].Text;

                arr[i] = resource;
            }

            return arr;
        }

        [Benchmark]
        public Resource[] Wrapping()
        {
            WrapPolicy<ToWrap> policy =
                WrapPolicy
                .For<ToWrap>()
                .Property(i => i.Text);

            return Data
                .WrapUsing(policy)
                .ToArray();
        }

        [Benchmark]
        public Resource[] CachedPolicyWrapping()
            => Data
                .WrapUsing(Policy)
                .ToArray();
    }
}
