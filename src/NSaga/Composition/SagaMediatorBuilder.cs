using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSaga
{
    public class SagaMediatorBuilder
    {
        private readonly List<Type> pipelineHooks;
        private List<Assembly> assembliesToScan;
        private bool areComponentsRegistered;
        private TinyIoCContainer Container { get; set; }

        public SagaMediatorBuilder()
        {
            Container = new TinyIoCContainer();
            pipelineHooks = new List<Type>();
            this.assembliesToScan = AppDomain.CurrentDomain.GetAssemblies().ToList();
            areComponentsRegistered = false;

            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            UseSagaFactory<TinyIocSagaFactory>();
            UseMessageSerialiser<JsonNetSerialiser>();
            UseRepository<InMemorySagaRepository>();
            AddPiplineHook<MetadataPipelineHook>();
        }


        /// <summary>
        /// Replaces all previosly reigstered assemblies with the provided ones
        /// </summary>
        /// <param name="assemblies">List of assemblies to scan for sagas</param>
        /// <returns>Current Builder</returns>
        public SagaMediatorBuilder ReplaceAssembliesToScan(IEnumerable<Assembly> assemblies)
        {
            this.assembliesToScan = assembliesToScan.ToList();
            return this;
        }

        public SagaMediatorBuilder AddAssemblyToScan(Assembly assembly)
        {
            assembliesToScan.Add(assembly);
            return this;
        }

        public SagaMediatorBuilder AddAssembliesToScan(IEnumerable<Assembly> assemblies)
        {
            assembliesToScan.AddRange(assemblies);
            return this;
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

        public SagaMediatorBuilder Register(Type @interface, Type implementation)
        {
            Container.Register(@interface, implementation);

            return this;
        }


        public SagaMediatorBuilder Register(Type registerType, object instance)
        {
            Container.Register(registerType, instance);

            return this;
        }


        public SagaMediatorBuilder RegisterComponents()
        {
            if (areComponentsRegistered)
            {
                throw new Exception("Components have already been registered. Can't register second time");
            }

            Container.RegisterMultiple(typeof(IPipelineHook), pipelineHooks);
            Container.Register(typeof(ISagaMediator), typeof(SagaMediator));
            Container.RegisterSagas(assembliesToScan);
            areComponentsRegistered = true;

            return this;
        }

        public ISagaMediator ResolveMediator()
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            var mediator = Container.Resolve<ISagaMediator>();
            return mediator;
        }

        public ISagaRepository ResolveRepository()
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve<ISagaRepository>();
        }

        public T Resolve<T>() where T : class
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve<T>();
        }

        public object Resolve(Type objectType)
        {
            if (!areComponentsRegistered)
            {
                RegisterComponents();
            }

            return Container.Resolve(objectType);
        }

    }
}