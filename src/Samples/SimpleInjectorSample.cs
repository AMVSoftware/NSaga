using System;
using NSaga;
using NSaga.SimpleInjector;
using NSaga.SqlServer;
using SimpleInjector;

namespace Samples
{
    public class SimpleInjectorSample
    {
        public void RunSample()
        {
            var simpleInjectorContainer = new Container();
            var mediator = Wireup.Init()
                                 .UseSimpleInjector(simpleInjectorContainer)
                                 .UseSqlServerStorage("NSagaDatabase")
                                 .BuildMediator();

            var correlationId = Guid.NewGuid();
            var initMessage = new PersonalDetailsVerification(correlationId);

            mediator.Consume(initMessage);
        }
    }
}
