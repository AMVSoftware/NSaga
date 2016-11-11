using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmarking.Sagas;
using NSaga;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MediatorBenchmarking>();
            //var bench = new MediatorBenchmarking();
            //bench.InitiateSaga();
        }
    }

    public class MediatorBenchmarking
    {
        private ISagaMediator mediator;
        private Guid correlationId;

        public MediatorBenchmarking()
        {
            var builder = Wireup.UseInternalContainer();
            mediator = builder.ResolveMediator();
            correlationId = Guid.NewGuid();
            mediator.Consume(new FirstMessage(correlationId));
        }


        [Benchmark]
        public void InitiateSaga()
        {
            mediator.Consume(new FirstMessage(Guid.NewGuid()));
        }

        [Benchmark]
        public void ConsumeMessage()
        {
            mediator.Consume(new SecondMessage(correlationId));
        }
    }
}
