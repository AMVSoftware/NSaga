using Autofac;
using NSaga;
using NSaga.Autofac;
using Tests.Implementations;

namespace Tests.Autofac
{
    public class AutofacSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public AutofacSagaFactoryTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents();

            var container = builder.Build();
            Sut = container.Resolve<ISagaFactory>();
        }
    }
}
