using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSaga.Pipeline;
using TinyIoC;


namespace NSaga
{
    public class Wireup
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


        public static Wireup Init()
        {
            return new Wireup();
        }
    }


    public class SagaMediatorBuilder
    {
        private readonly IConformingContainer container;
        private Assembly[] assembliesToScan;
        private List<Type> pipelineHooks;

        public SagaMediatorBuilder(IConformingContainer container)
        {
            this.container = container;
            pipelineHooks = new List<Type>();
            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            UseSagaFactory<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddPiplineHook<MetadataPipelineHook>();
            AddAssembliesToScan(AppDomain.CurrentDomain.GetAssemblies());

            container.Register(typeof(ISagaMediator), typeof(SagaMediator));
        }

        public IConformingContainer Container => container;

        public SagaMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            container.Register(typeof(IMessageSerialiser), typeof(TSerialiser));

            return this;
        }

        public SagaMediatorBuilder UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            container.Register(typeof(IMessageSerialiser), messageSerialiser);

            return this;
        }



        public SagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            container.Register(typeof(ISagaRepository), typeof(TRepository));

            return this;
        }

        public SagaMediatorBuilder UseRepository(ISagaRepository sagaRepository)
        {
            container.Register(typeof(ISagaRepository), sagaRepository);

            return this;
        }


        public SagaMediatorBuilder UseSagaFactory<TSagaFactory>() where TSagaFactory : ISagaFactory
        {
            container.Register(typeof(ISagaFactory), typeof(TSagaFactory));

            return this;
        }

        public SagaMediatorBuilder UseSagaFactory(ISagaFactory sagaFactory)
        {
            container.Register(typeof(ISagaFactory), sagaFactory);

            return this;
        }

        
        public SagaMediatorBuilder AddAssembliesToScan(Assembly[] assemblies)
        {
            assembliesToScan = assemblies;

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
            container.Register(typeof(Assembly[]), assembliesToScan);
            container.RegisterMultiple(typeof(IPipelineHook), pipelineHooks);

            //TODO register all the sagas from 
            //container.RegisterMultiple(typeof(ISaga<>), assembliesToScan.Select(a => a.GetTypes().));

            var mediator = container.Resolve<ISagaMediator>();
            return mediator;
        }
    }
}
