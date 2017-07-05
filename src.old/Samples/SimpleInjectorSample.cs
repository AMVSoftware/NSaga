using System;
using NSaga;
using NSaga.SimpleInjector;
using SimpleInjector;

namespace Samples
{
    public class SimpleInjectorSample
    {
        public void RunSample()
        {
            var container = new Container();
            container.RegisterNSagaComponents();

            var mediator = container.GetInstance<ISagaMediator>();

            var correlationId = Guid.NewGuid();
            var initMessage = new PersonalDetailsVerification(correlationId);

            mediator.Consume(initMessage);
        }
    }
}
