using System;
using System.Collections.Generic;
using System.Reflection;
using NSaga.Composition;
using TinyIoC;

namespace NSaga
{
    public class InternalMediatorBuilder : AbstractSagaMediatorBuilder<InternalMediatorBuilder>
    {
        private readonly TinyIoCContainer container;
        private readonly IEnumerable<Assembly> assembliesToScan;
        private bool registrationsDone = false;


        public InternalMediatorBuilder(TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            this.container = container;
            sagaFactory = new Registration(typeof(TinyIocSagaFactory));
            this.assembliesToScan = assembliesToScan;
        }

        public InternalMediatorBuilder(IEnumerable<Assembly> assembliesToScan)
        {
            this.container = TinyIoCContainer.Current;
            sagaFactory = new Registration(typeof(TinyIocSagaFactory));
            this.assembliesToScan = assembliesToScan;
        }

        public override InternalMediatorBuilder GetThis()
        {
            return this;
        }

        public override void RegisterComponents()
        {
            if (registrationsDone)
            {
                throw new Exception("Registration is already done. Can't register components second time.");
            }

            ProcessRegistration(messageSerialiser);
            ProcessRegistration(repository);
            ProcessRegistration(sagaFactory);

            foreach (var hookRegistration in base.pipelineHooks)
            {
                ProcessRegistration(hookRegistration);
            }

            container.RegisterSagas(assembliesToScan);

            registrationsDone = true;
        }

        public override ISagaMediator ResolveMediator()
        {
            if (!registrationsDone)
            {
                RegisterComponents();
            }

            return container.Resolve<ISagaMediator>();
        }

        private void ProcessRegistration(Registration registration)
        {
            if (registration.RegisterByType)
            {
                container.Register(registration.Type);
            }
            else
            {
                container.Register(registration.Instance);
            }
        }
    }
}