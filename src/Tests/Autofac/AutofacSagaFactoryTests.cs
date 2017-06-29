using Autofac;
using NSaga;
using NSaga.Autofac;
using System.Reflection;
using Tests.Implementations;

namespace Tests.Autofac
{
    public class AutofacSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public AutofacSagaFactoryTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents(Assembly.GetExecutingAssembly());

            var container = builder.Build();
            Sut = container.Resolve<ISagaFactory>();
        }
    }
}
