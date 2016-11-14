using System;
using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using NSaga;
using NSaga.Autofac;
using Xunit;

namespace Tests.Autofac
{
    public class AutofacNSagaIntegrationTests
    {
        [Theory]
        [InlineData(typeof(ISagaMediator), typeof(SagaMediator))]
        [InlineData(typeof(ISagaRepository), typeof(InMemorySagaRepository))]
        [InlineData(typeof(ISagaFactory), typeof(AutofacSagaFactory))]
        [InlineData(typeof(IMessageSerialiser), typeof(JsonNetSerialiser))]
        [InlineData(typeof(ISaga<MySagaData>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaInitiatingMessage>), typeof(MySaga))]
        [InlineData(typeof(ConsumerOf<MySagaConsumingMessage>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaAdditionalInitialser>), typeof(MySaga))]
        public void DefaultRegistration_Resolves_DefaultComponents(Type requestedType, Type expectedImplementation)
        {
            //Arrange
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents();
            var container = builder.Build();

            // Act
            var result = container.Resolve(requestedType);

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType(expectedImplementation);
        }


        [Fact]
        public void DefaultRegistration_ResolvePipline_ResolvesMetadataHook()
        {
            //Arrange
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents();
            var container = builder.Build();
            
            // Act
            var result = container.Resolve<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(1)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook));
        }


        [Fact]
        public void AddPipeline_Resolves_AdditionalHooks()
        {
            //Arrange
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents()
                .AddSagaPipelineHook<NullPipelineHook>();
            var container = builder.Build();

            // Act
            var result = container.Resolve<IEnumerable<IPipelineHook>>();

            // Assert
            result.Should().NotBeNull()
                       .And.HaveCount(2)
                       .And.Contain(h => h.GetType() == typeof(MetadataPipelineHook))
                       .And.Contain(h => h.GetType() == typeof(NullPipelineHook));
        }

        [Fact]
        public void OverrideRepository_Resolves_OverridenRepository()
        {
            //Arrange
            var builder = new ContainerBuilder();
            builder.RegisterNSagaComponents()
                   .UseSagaRepository<NullSagaRepository>();
            var container = builder.Build();

            // Act
            var result = container.Resolve<ISagaRepository>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullSagaRepository>();
        }
    }
}
