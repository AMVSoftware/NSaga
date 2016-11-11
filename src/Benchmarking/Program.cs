using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Benchmarking.Sagas;
using NSaga;

namespace Benchmarking
{
    class Program
    {
        public static Guid FirstGuid = new Guid("85B8ED38-D0BA-4CD3-BBB3-15952B76E774");

        static void Main(string[] args)
        {
#if DEBUG
            var bench = new MediatorBenchmarking();
            bench.InitiateSaga();
            bench.ConsumeMessage();
#else
            var summary = BenchmarkRunner.Run<MediatorBenchmarking>();
#endif
        }
    }

    public class MediatorBenchmarking
    {
        private ISagaMediator mediator;
        private Guid correlationId;

        public MediatorBenchmarking()
        {
            mediator = Wireup.UseInternalContainer().UseRepository<FastSagaRepository>().ResolveMediator();

            correlationId = Guid.NewGuid();
            //mediator.Consume(new FirstMessage(correlationId));
            mediator.Consume(new FirstMessage(Program.FirstGuid));
        }


        [Benchmark]
        public void InitiateSaga()
        {
            //mediator.Consume(new FirstMessage(Guid.NewGuid()));
            mediator.Consume(new FirstMessage(Program.FirstGuid));
        }

        [Benchmark]
        public void ConsumeMessage()
        {
            mediator.Consume(new SecondMessage(correlationId));
        }
    }
}
