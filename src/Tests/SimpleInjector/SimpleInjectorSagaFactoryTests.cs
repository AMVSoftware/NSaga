using NSaga;
using NSaga.SimpleInjector;
using SimpleInjector;
using Tests.Implementations;

namespace Tests.SimpleInjector
{
    public class SimpleInjectorSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public SimpleInjectorSagaFactoryTests()
        {
            var container = new Container();
            container.RegisterNSagaComponents();
            Sut = container.GetInstance<ISagaFactory>();
        }
    }
}
