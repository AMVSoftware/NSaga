using TinyIoC;

namespace NSaga
{
    public class TinyIocMediatorBuilder : AbstractSagaMediatorBuilder<TinyIocMediatorBuilder>
    {
        private readonly TinyIoCContainer container;
        private bool registrationsDone = false;

        public TinyIocMediatorBuilder(TinyIoCContainer container)
        {
            this.container = container;
            sagaFactory = new Registration(typeof(TinyIocSagaFactory));
        }

        public TinyIocMediatorBuilder()
        {
            this.container = TinyIoCContainer.Current;
            sagaFactory = new Registration(typeof(TinyIocSagaFactory));
        }

        public override TinyIocMediatorBuilder GetThis()
        {
            return this;
        }

        public override void RegisterComponents()
        {
            ProcessRegistration(messageSerialiser);
            ProcessRegistration(repository);
            ProcessRegistration(sagaFactory);

            foreach (var hookRegistration in base.pipelineHooks)
            {
                ProcessRegistration(hookRegistration);
            }

            container.Register(base.assembliesToScan);

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