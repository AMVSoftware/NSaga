using System;
using NSaga;
using Tests.Implementations;

namespace Tests.Composition
{
    public class TinyIocSagaFactoryTests : SagaFactoryTestsTemplate, IDisposable
    {
        public TinyIocSagaFactoryTests()
        {
            var builder = Wireup.UseInternalContainer();
            Sut = builder.Resolve<ISagaFactory>();
        }

        public void Dispose()
        {
            TinyIoCContainer.Current.Dispose();
        }
    }
}
