using NSaga;
using NSaga.SimpleInjector;
using SimpleInjector;
using System.Reflection;
using Tests.Implementations;

namespace Tests.SimpleInjector
{
    public class SimpleInjectorSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public SimpleInjectorSagaFactoryTests()
        {
            var container = new Container();
            container.RegisterNSagaComponents(Assembly.GetExecutingAssembly());
            Sut = container.GetInstance<ISagaFactory>();
        }
    }
}
