using System;
using NSaga;
using Tests.Implementations;
using System.Reflection;

namespace Tests.Composition
{
    public class TinyIocSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public TinyIocSagaFactoryTests()
        {
            var builder = Wireup.UseInternalContainer(Assembly.GetExecutingAssembly());
            Sut = builder.Resolve<ISagaFactory>();
        }
    }
}
