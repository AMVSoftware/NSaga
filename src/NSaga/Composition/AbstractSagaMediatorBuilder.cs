using System;
using System.Collections.Generic;
using System.Linq;
using NSaga.Pipeline;

namespace NSaga
{
    public abstract class AbstractSagaMediatorBuilder<TChild> where TChild : AbstractSagaMediatorBuilder<TChild>
    {
        protected List<Registration> pipelineHooks;
        protected Registration messageSerialiser;
        protected Registration repository;
        protected Registration sagaFactory;

        protected AbstractSagaMediatorBuilder()
        {
            pipelineHooks = new List<Registration>()
            {
                new Registration(typeof(IPipelineHook), typeof(MetadataPipelineHook)),
            };

            messageSerialiser = new Registration(typeof(IMessageSerialiser), typeof(JsonNetSerialiser));
            repository = new Registration(typeof(ISagaRepository), typeof(InMemorySagaRepository));
        }

        public abstract TChild GetThis();


        public virtual TChild UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            this.messageSerialiser = new Registration(typeof(IMessageSerialiser), typeof(TSerialiser));
            return GetThis();
        }

        public virtual TChild UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            this.messageSerialiser = new Registration(typeof(IMessageSerialiser), messageSerialiser);
            return GetThis();
        }

        public virtual TChild UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            this.repository = new Registration(typeof(ISagaRepository), typeof(TRepository));
            return GetThis();
        }

        public virtual TChild UseRepository(ISagaRepository sagaRepository)
        {
            this.repository = new Registration(typeof(ISagaRepository), sagaRepository);
            return GetThis();
        }

        public virtual TChild AddPiplineHook<TPipelineHook>() where TPipelineHook : IPipelineHook
        {
            pipelineHooks.Add(new Registration(typeof(IPipelineHook), typeof(TPipelineHook)));

            return GetThis();
        }


        public virtual TChild AddPiplineHook(Type pipelineHookType)
        {
            if (!pipelineHookType.IsClass || !pipelineHookType.GetInterfaces().Contains(typeof(IPipelineHook)))
            {
                throw new ArgumentException("Provided type is not a class or does not implement IPipelineHook");
            }

            pipelineHooks.Add(new Registration(typeof(IPipelineHook), pipelineHookType));

            return GetThis();
        }

        public virtual TChild AddPipelineHook(IPipelineHook pipelineHook)
        {
            pipelineHooks.Add(new Registration(typeof(IPipelineHook), pipelineHook));
            return GetThis();
        }

        public abstract TChild RegisterComponents();

        public abstract ISagaMediator ResolveMediator();
    }


    public class Registration
    {
        public Registration(Type @interface, Type implementationType)
        {
            Interface = @interface;
            ImplementationType = implementationType;
        }

        public Registration(Type @interface, object instance)
        {
            Interface = @interface;
            Instance = instance;
        }


        public Type Interface { get; private set; }
        public Type ImplementationType { get; private set; }
        public object Instance { get; private set; }

        public bool RegisterByType => ImplementationType != null;
    }
}