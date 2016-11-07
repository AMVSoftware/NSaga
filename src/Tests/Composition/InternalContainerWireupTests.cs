using System;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using Xunit;

namespace Tests.Composition
{
    public class InternalContainerWireupTests
    {
        private readonly ISagaMediator sagaMediator;

        public InternalContainerWireupTests()
        {
            sagaMediator = Wireup.UseInternalContainer().ResolveMediator();
        }

        [Fact]
        public void Default_Provides_InMemoryRepository()
        {
            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaRepository");

            sagaRepository.Should().BeOfType<InMemorySagaRepository>();
        }


        [Fact]
        public void Default_Provides_TinyIocSagaFactory()
        {
            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaFactory");

            sagaRepository.Should().BeOfType<TinyIocSagaFactory>();
        }


        [Fact]
        public void Default_Can_Initialise_Saga()
        {
            //Arrange
            var correlationId = Guid.NewGuid();

            // Act
            var result = sagaMediator.Consume(new MySagaInitiatingMessage(correlationId));

            // Assert
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
