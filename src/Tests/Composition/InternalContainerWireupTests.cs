using System;
using System.Collections.Generic;
using FluentAssertions;
using NSaga;
using Xunit;


namespace Tests.Composition
{
    public class InternalContainerWireupTests
    {
        [Theory]
        [InlineData(typeof(ISagaMediator), typeof(SagaMediator))]
        [InlineData(typeof(ISagaRepository), typeof(InMemorySagaRepository))]
        [InlineData(typeof(ISagaFactory), typeof(TinyIocSagaFactory))]
        [InlineData(typeof(IMessageSerialiser), typeof(JsonNetSerialiser))]
        [InlineData(typeof(ISaga<MySagaData>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaInitiatingMessage>), typeof(MySaga))]
        [InlineData(typeof(ConsumerOf<MySagaConsumingMessage>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaAdditionalInitialser>), typeof(MySaga))]
        public void DefaultRegistration_Resolves_DefaultComponents(Type requestedType, Type expectedImplementation)
        {
            //Arrange
            var builder = Wireup.UseInternalContainer();

            // Act
            var result = builder.Resolve(requestedType);

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType(expectedImplementation);
        }

        [Fact]
        public void OverrideRepository_Resolves_OverridenRepository()
        {
            //Arrange
            var builder = Wireup.UseInternalContainer()
                                .UseRepository<NullSagaRepository>();

            // Act
            var result = builder.Resolve<ISagaRepository>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullSagaRepository>();
        }


        [Fact]
        public void OverrideSagaFactory_Resolves_OverridenFactory()
        {
            // Arrange
            var builder = Wireup.UseInternalContainer()
                                .UseSagaFactory<NullSagaFactory>();

            // Act
            var result = builder.Resolve<ISagaFactory>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullSagaFactory>();
        }


        [Fact]
        public void DefaultRegistration_ResolvePipline_ResolvesMetadataHook()
        {
            //Arrange
            var builder = Wireup.UseInternalContainer();

            // Act
            var result = builder.Resolve<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(1)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook));
        }


        [Fact]
        public void AddingPipeline_Adds_ToCollection()
        {
            //Arrange
            var builder = Wireup.UseInternalContainer()
                                .AddPiplineHook<NullPipelineHook>();

            // Act
            var result = builder.Resolve<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(2)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook))
                       .And.Contain(h => h.GetType() == typeof(NullPipelineHook));
        }

        [Fact]
        public void Default_Can_Initialise_Saga()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaMediator = Wireup.UseInternalContainer().ResolveMediator();

            // Act
            var result = sagaMediator.Consume(new MySagaInitiatingMessage(correlationId));

            // Assert
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
