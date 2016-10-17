using System;
using System.Reflection;
using NSaga.Pipeline;
using TinyIoC;


namespace NSaga
{
    public static class Wireup
    {
        public static InternalMediatorBuilder UseInternalContainer()
        {
            var builder = new InternalMediatorBuilder(TinyIoCContainer.Current);

            return builder;
        }

        public static InternalMediatorBuilder UseInternalContainer(TinyIoCContainer container)
        {
            var builder = new InternalMediatorBuilder(container);

            return builder;
        }
    }


    public class InternalMediatorBuilder
    {
        private readonly TinyIoC.TinyIoCContainer container;
        private Assembly[] assembliesToScan;
        private readonly CompositePipelineHook compositePipeline;

        public InternalMediatorBuilder(TinyIoCContainer container)
        {
            this.container = container;
            this.compositePipeline = new CompositePipelineHook();
            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            UseServiceLocator<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddAssembliesToScan(AppDomain.CurrentDomain.GetAssemblies());
            AddPiplineHook(new MetadataPipelineHook(container.Resolve<IMessageSerialiser>()));
            AddAssembliesToScan(AppDomain.CurrentDomain.GetAssemblies());

            container.Register(typeof(ISagaMediator), typeof(SagaMediator));
        }

        public InternalMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            container.Register(typeof(IMessageSerialiser), typeof(TSerialiser));

            return this;
        }

        public InternalMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            container.Register(typeof(ISagaRepository), typeof(TRepository));

            return this;
        }

        public InternalMediatorBuilder UseServiceLocator<TServiceLocator>() where TServiceLocator : ISagaFactory
        {
            container.Register(typeof(ISagaFactory), typeof(TServiceLocator));

            return this;
        }

        public InternalMediatorBuilder AddAssembliesToScan(Assembly[] assemblies)
        {
            assembliesToScan = assemblies;

            return this;
        }

        public InternalMediatorBuilder AddPiplineHook(IPipelineHook pipelineHook)
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
