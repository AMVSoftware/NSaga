using FluentAssertions;
using NSaga.SimpleInjector;
using SimpleInjector;
using Xunit;

namespace Tests.SimpleInjector
{
    public class SimpleInjectorSagaFactoryTests
    {
        private readonly SimpleInjectorSagaFactory sut;

        public SimpleInjectorSagaFactoryTests()
        {
            var container = new Container();
            container.RegisterNSagaComponents();
            sut = new SimpleInjectorSagaFactory(container);
        }

        [Fact]
        public void Resolve_Saga_Resolved()
        {
            //Arrange


            // Act

            // Assert
        }
    }
}
