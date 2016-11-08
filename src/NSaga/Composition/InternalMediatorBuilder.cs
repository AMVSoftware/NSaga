using System;
using System.Collections.Generic;
using System.Reflection;
using NSaga.Composition;
using TinyIoC;

namespace NSaga
{
    public class InternalMediatorBuilder : AbstractSagaMediatorBuilder
    {
        public readonly TinyIoCContainer Container;
        private readonly IEnumerable<Assembly> assembliesToScan;
        private bool registrationsDone = false;


        public InternalMediatorBuilder(TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            this.Container = container;
            sagaFactory = new Registration(typeof(ISagaFactory), typeof(TinyIocSagaFactory));
            this.assembliesToScan = assembliesToScan;
        }

        public override AbstractSagaMediatorBuilder GetThis()
        {
            return this;
        }

        public InternalMediatorBuilder RegisterComponents()
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

            Container.RegisterSagas(assembliesToScan);
            Container.Register<ISagaMediator, SagaMediator>();

            registrationsDone = true;
            return this;
        }

        public override ISagaMediator ResolveMediator()
        {
            if (!registrationsDone)
            {
                RegisterComponents();
            }

            return Container.Resolve<ISagaMediator>();
        }

        private void ProcessRegistration(Registration registration)
        {
            if (registration.RegisterByType)
            {
                Container.Register(registration.Interface, registration.ImplementationType);
            }
            else
            {
                Container.Register(registration.Interface, registration.Instance);
            }
        }
    }
}