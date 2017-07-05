using NSaga;
using NSaga.StructureMap;
using StructureMap;
using System;

namespace Samples
{
    public class StructureMapSample
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
