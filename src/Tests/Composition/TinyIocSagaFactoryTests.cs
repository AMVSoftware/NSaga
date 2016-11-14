using System;
using NSaga;
using Tests.Implementations;

namespace Tests.Composition
{
    public class TinyIocSagaFactoryTests : SagaFactoryTestsTemplate
    {
        public TinyIocSagaFactoryTests()
        {
            var builder = Wireup.UseInternalContainer();
            Sut = builder.Resolve<ISagaFactory>();
        }
    }
}
