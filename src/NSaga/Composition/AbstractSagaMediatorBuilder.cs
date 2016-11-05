using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSaga.Pipeline;

namespace NSaga
{
    public abstract class AbstractSagaMediatorBuilder<TChild> where TChild : AbstractSagaMediatorBuilder<TChild>
    {
        protected List<Assembly> assembliesToScan;
        protected List<Registration> pipelineHooks;
        protected Registration messageSerialiser;
        protected Registration repository;
        protected Registration sagaFactory;

        protected AbstractSagaMediatorBuilder()
        {
            pipelineHooks = new List<Registration>()
            {
                new Registration(typeof(MetadataPipelineHook)),
            };
            assembliesToScan = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());

            messageSerialiser = new Registration(typeof(JsonNetSerialiser));
            repository = new Registration(typeof(InMemorySagaRepository));
        }

        public abstract TChild GetThis();


        public virtual TChild UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            this.messageSerialiser = new Registration(typeof(TSerialiser));
            return GetThis();
        }

        public virtual TChild UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            this.messageSerialiser = new Registration(messageSerialiser);
            return GetThis();
        }

        public virtual TChild UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            this.repository = new Registration(typeof(TRepository));
            return GetThis();
        }

        public virtual TChild UseRepository(ISagaRepository sagaRepository)
        {
            this.repository = new Registration(sagaRepository);
            return GetThis();
        }

        //public virtual TChild UseSagaFactory<TSagaFactory>() where TSagaFactory : ISagaFactory
        //{
        //    this.sagaFactory = new Registration(typeof(TSagaFactory));
        //    return GetThis();
        //}

        //public virtual TChild UseSagaFactory(ISagaFactory sagaFactory)
        //{
        //    this.sagaFactory = new Registration(sagaFactory);
        //    return GetThis();
        //}

        public virtual TChild AddAssemblyToScan(Assembly assembly)
        {
            assembliesToScan.Add(assembly);
            return GetThis();
        }


        public virtual TChild AddAssembliesToScan(Assembly[] assemblies)
        {
            assembliesToScan.AddRange(assemblies);
            return GetThis();
        }

        public virtual TChild AddPiplineHook<TPipelineHook>() where TPipelineHook : IPipelineHook
        {
            pipelineHooks.Add(new Registration(typeof(TPipelineHook)));

            return GetThis();
        }


        public virtual TChild AddPiplineHook(Type pipelineHookType)
        {
            if (!pipelineHookType.IsClass || !pipelineHookType.GetInterfaces().Contains(typeof(IPipelineHook)))
            {
                throw new ArgumentException("Provided type is not a class or does not implement IPipelineHook");
            }

            pipelineHooks.Add(new Registration(pipelineHookType));

            return GetThis();
        }

        public virtual TChild AddPipelineHook(IPipelineHook pipelineHook)
        {
            pipelineHooks.Add(new Registration(pipelineHook));
            return GetThis();
        }

        public abstract void RegisterComponents();

        public abstract ISagaMediator ResolveMediator();
    }


    public class Registration
    {
        public Registration(Type type)
        {
            Type = type;
        }

        public Registration(object instance)
        {
            Instance = instance;
        }
        public Type Type { get; private set; }
        public object Instance { get; private set; }

        public bool RegisterByType => Type != null;
    }
}