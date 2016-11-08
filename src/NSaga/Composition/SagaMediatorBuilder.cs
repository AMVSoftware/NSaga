using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSaga.Composition;
using NSaga.Pipeline;
using TinyIoC;

namespace NSaga
{
    public class SagaMediatorBuilder
    {
        private List<Type> pipelineHooks;
        private IEnumerable<Assembly> assembliesToScan;
        public TinyIoCContainer Container { get; private set; }

        public SagaMediatorBuilder(IEnumerable<Assembly> assembliesToScan, TinyIoCContainer container)
        {
            this.Container = container;
            pipelineHooks = new List<Type>();
            this.assembliesToScan = assembliesToScan;
            RegisterDefaults();
        }


        private void RegisterDefaults()
        {
            UseSagaFactory<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddPiplineHook<MetadataPipelineHook>();

            Container.Register(typeof(ISagaMediator), typeof(SagaMediator));
            Container.RegisterSagas(assembliesToScan);
        }

        public SagaMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            Container.Register(typeof(IMessageSerialiser), typeof(TSerialiser));

            return this;
        }

        public SagaMediatorBuilder UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            Container.Register(typeof(IMessageSerialiser), messageSerialiser);

            return this;
        }


        public SagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            Container.Register(typeof(ISagaRepository), typeof(TRepository));

            return this;
        }

        public SagaMediatorBuilder UseRepository(ISagaRepository sagaRepository)
        {
            Container.Register(typeof(ISagaRepository), sagaRepository);

            return this;
        }


        public SagaMediatorBuilder UseSagaFactory<TSagaFactory>() where TSagaFactory : ISagaFactory
        {
            Container.Register(typeof(ISagaFactory), typeof(TSagaFactory));

            return this;
        }

        public SagaMediatorBuilder UseSagaFactory(ISagaFactory sagaFactory)
        {
            Container.Register(typeof(ISagaFactory), sagaFactory);

            return this;
        }

        
        public SagaMediatorBuilder AddPiplineHook<TPipelineHook>() where TPipelineHook : IPipelineHook
        {
            pipelineHooks.Add(typeof(TPipelineHook));

            return this;
        }


        public SagaMediatorBuilder AddPiplineHook(Type pipelineHookType)
        {
            if (!pipelineHookType.IsClass || !pipelineHookType.GetInterfaces().Contains(typeof(IPipelineHook)))
            {
                throw new ArgumentException("Provided type is not a class or does not implement IPipelineHook");
            }

            pipelineHooks.Add(pipelineHookType);

            return this;
        }


        public ISagaMediator BuildMediator()
        {
            Container.RegisterMultiple(typeof(IPipelineHook), pipelineHooks);

            var mediator = Container.Resolve<ISagaMediator>();
            return mediator;
        }
    }
}