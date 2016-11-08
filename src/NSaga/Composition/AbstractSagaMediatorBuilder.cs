using System;
using System.Collections.Generic;
using System.Linq;
using NSaga.Pipeline;

namespace NSaga
{
    public interface IPersistenceMediatorBuilder
    {
        AbstractSagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository;
        AbstractSagaMediatorBuilder UseRepository(ISagaRepository sagaRepository);
    }

    public abstract class AbstractSagaMediatorBuilder : IPersistenceMediatorBuilder
    {
        protected List<Registration> allRegistrations;

        protected AbstractSagaMediatorBuilder()
        {
            allRegistrations = new List<Registration>()
            {
                new Registration(typeof(IPipelineHook), typeof(MetadataPipelineHook)),
                new Registration(typeof(IMessageSerialiser), typeof(JsonNetSerialiser)),
                new Registration(typeof(ISagaRepository), typeof(InMemorySagaRepository)),
            };
        }

        public abstract AbstractSagaMediatorBuilder GetThis();


        public virtual AbstractSagaMediatorBuilder UseMessageSerialiser<TSerialiser>() where TSerialiser : IMessageSerialiser
        {
            var existingRegistration = allRegistrations.FirstOrDefault(r => r.Interface == typeof(IMessageSerialiser));
            if (existingRegistration != null)
            {
                existingRegistration = new Registration(typeof(IMessageSerialiser), typeof(TSerialiser));
            }
            else
            {
                allRegistrations.Add(new Registration(typeof(IMessageSerialiser), typeof(TSerialiser)));
            }
            
            return GetThis();
        }

        public virtual AbstractSagaMediatorBuilder UseMessageSerialiser(IMessageSerialiser messageSerialiser)
        {
            this.messageSerialiser = new Registration(typeof(IMessageSerialiser), messageSerialiser);
            return GetThis();
        }

        public virtual AbstractSagaMediatorBuilder UseRepository<TRepository>() where TRepository : ISagaRepository
        {
            this.repository = new Registration(typeof(ISagaRepository), typeof(TRepository));
            return GetThis();
        }

        public virtual AbstractSagaMediatorBuilder UseRepository(ISagaRepository sagaRepository)
        {
            this.repository = new Registration(typeof(ISagaRepository), sagaRepository);
            return GetThis();
        }

        public virtual AbstractSagaMediatorBuilder AddPiplineHook<TPipelineHook>() where TPipelineHook : IPipelineHook
        {
            pipelineHooks.Add(new Registration(typeof(IPipelineHook), typeof(TPipelineHook)));

            return GetThis();
        }


        public virtual AbstractSagaMediatorBuilder AddPiplineHook(Type pipelineHookType)
        {
            if (!pipelineHookType.IsClass || !pipelineHookType.GetInterfaces().Contains(typeof(IPipelineHook)))
            {
                throw new ArgumentException("Provided type is not a class or does not implement IPipelineHook");
            }

            pipelineHooks.Add(new Registration(typeof(IPipelineHook), pipelineHookType));

            return GetThis();
        }

        public virtual AbstractSagaMediatorBuilder AddPipelineHook(IPipelineHook pipelineHook)
        {
            pipelineHooks.Add(new Registration(typeof(IPipelineHook), pipelineHook));
            return GetThis();
        }

        //public abstract AbstractSagaMediatorBuilder RegisterComponents();

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