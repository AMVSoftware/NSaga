using System;
using System.Reflection;
using NSaga.Pipeline;
using TinyIoC;


namespace NSaga
{
    public static class Wireup
    {
        public static SagaMediatorBuilder UseInternalContainer()
        {
            var builder = new SagaMediatorBuilder(new TinyIocConformingContainer());

            return builder;
        }

        public static SagaMediatorBuilder UseInternalContainer(TinyIoCContainer container)
        {
            var builder = new SagaMediatorBuilder(new TinyIocConformingContainer(container));

            return builder;
        }

        public static SagaMediatorBuilder UseContainer(IConformingContainer conformingContainer)
        {
            var builder = new SagaMediatorBuilder(conformingContainer);

            return builder;
        }
    }


    public class SagaMediatorBuilder
    {
        private readonly IConformingContainer container;
        private Assembly[] assembliesToScan;
        private readonly CompositePipelineHook compositePipeline;

        public SagaMediatorBuilder(IConformingContainer container)
        {
            this.container = container;
            this.compositePipeline = new CompositePipelineHook();
            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            UseSagaFactory<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddAssembliesToScan(AppDomain.CurrentDomain.GetAssemblies());
            AddPiplineHook(new MetadataPipelineHook(container.Resolve<IMessageSerialiser>()));
            AddAssembliesToScan(AppDomain.CurrentDomain.GetAssemblies());

            container.Register(typeof(ISagaMediator), typeof(SagaMediator));
        }

        public SagaMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            container.Register(typeof(IMessageSerialiser), typeof(TSerialiser));

            return this;
        }

        public SagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            container.Register(typeof(ISagaRepository), typeof(TRepository));

            return this;
        }

        public SagaMediatorBuilder UseSagaFactory<TSagaFactory>() where TSagaFactory : ISagaFactory
        {
            container.Register(typeof(ISagaFactory), typeof(TSagaFactory));

            return this;
        }

        public SagaMediatorBuilder AddAssembliesToScan(Assembly[] assemblies)
        {
            assembliesToScan = assemblies;

            return this;
        }

        public SagaMediatorBuilder AddPiplineHook(IPipelineHook pipelineHook)
        {
            compositePipeline.AddHook(pipelineHook);

            return this;
        }

        public ISagaMediator Build()
        {
            container.Register(typeof(IPipelineHook), compositePipeline);

            container.Register(typeof(Assembly[]), assembliesToScan);


            var mediator = container.Resolve<ISagaMediator>();
            return mediator;
        }
    }
}
