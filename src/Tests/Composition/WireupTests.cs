using System;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using Xunit;

namespace Tests.Composition
{
    public class WireupTests
    {
        private readonly SagaMediator sagaMediator;

        public WireupTests()
        {
            sagaMediator = (SagaMediator)Wireup.UseInternalContainer().BuildMediator();
        }

        [Fact]
        public void Default_Provides_InMemoryRepository()
        {
            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaRepository");

            sagaRepository.Should().BeOfType<InMemorySagaRepository>();
        }


        [Fact]
        public void Default_Provides_TinyIocServiceLocator()
        {
            var sagaRepository = Reflection.GetPrivate(sagaMediator, "sagaFactory");

            sagaRepository.Should().BeOfType<TinyIocSagaFactory>();
        }


        [Fact]
        public void Default_Provides_MetadataPipline()
        {
            var sagaRepository = Reflection.GetPrivate(sagaMediator, "pipelineHook");

            sagaRepository.Should().BeOfType<CompositePipelineHook>();
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
