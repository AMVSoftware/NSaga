using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSaga;
using NSaga.Pipeline;
using Xunit;


namespace Tests.Composition
{
    public class InternalContainerWireupTests
    {
        [Theory]
        [InlineData("sagaRepository", typeof(InMemorySagaRepository))]
        [InlineData("sagaFactory", typeof(TinyIocSagaFactory))]
        [InlineData("pipelineHook", typeof(CompositePipelineHook))]
        public void Default_Provides_InMemoryRepository(string propertyName, Type expectedType)
        {
            var sagaMediator = Wireup.UseInternalContainer().ResolveMediator();

            ValidatePrivateProperty(sagaMediator, propertyName, expectedType);
        }

        [Fact]
        public void NullObjects_Can_Be_Resolved()
        {
            var mediator = Wireup.UseInternalContainer()
                                 .UseSagaFactory<NullSagaFactory>()
                                 .UseRepository<NullSagaRepository>()
                                 .ResolveMediator();

            mediator.Should().NotBeNull();
            ValidatePrivateProperty(mediator, "sagaRepository", typeof(NullSagaRepository));
            ValidatePrivateProperty(mediator, "sagaFactory", typeof(NullSagaFactory));
        }

        [Fact]
        public void AddingPipeline_Adds_ToCollection()
        {
            //Arrange
            var mediator = Wireup.UseInternalContainer()
                                 .AddPiplineHook<NullPipelineHook>()
                                 .ResolveMediator();
            var composite = Reflection.GetPrivate(mediator, "pipelineHook");
            var hooks = (List<IPipelineHook>)Reflection.GetPrivate(composite, "hooks");

            // Assert
            hooks.Should().HaveCount(2);
            hooks.FirstOrDefault(h => h.GetType() == typeof(NullPipelineHook)).Should().NotBeNull();
            hooks.FirstOrDefault(h => h.GetType() == typeof(MetadataPipelineHook)).Should().NotBeNull();
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



        private static void ValidatePrivateProperty(ISagaMediator sagaMediator, string propertyName, Type expectedType)
        {
            var propertyValue = Reflection.GetPrivate(sagaMediator, propertyName);

            propertyValue.Should().BeOfType(expectedType);
        }
    }
}
