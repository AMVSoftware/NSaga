using System;
using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using NSaga;
using NSaga.Autofac;
using NSaga.SqlServer;
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
            var container = new ContainerBuilder().RegisterNSagaComponents().Build();

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
            var container = new ContainerBuilder()
                                .RegisterNSagaComponents()
                                .Build();
            
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
            var container = new ContainerBuilder()
                                .RegisterNSagaComponents()
                                .AddSagaPipelineHook<NullPipelineHook>().Build();

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
            var container = new ContainerBuilder()
                                .RegisterNSagaComponents()
                                .UseSagaRepository<NullSagaRepository>()
                                .Build();

            // Act
            var result = container.Resolve<ISagaRepository>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullSagaRepository>();
        }

        [Fact]
        public void Default_Can_Initialise_Saga()
        {
            //Arrange
            var container = new ContainerBuilder().RegisterNSagaComponents().Build();
            var sagaMediator = container.Resolve<ISagaMediator>();

            // Act
            var result = sagaMediator.Consume(new MySagaInitiatingMessage(Guid.NewGuid()));

            // Assert
            result.IsSuccessful.Should().BeTrue();
        }


        [Fact]
        public void UseSqlServerRepository_Registers_AndResolves()
        {
            //Arrange
            var builder = new ContainerBuilder().RegisterNSagaComponents();
            builder.RegisterType<SqlSagaRepository>().As<ISagaRepository>();
            builder.Register(c => ConnectionFactory.FromConnectionStringName("TestingConnectionString")).As<IConnectionFactory>();
            var container = builder.Build();

            // Act
            var repository = container.Resolve<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull().And.BeOfType<SqlSagaRepository>();
        }


        [Fact]
        public void UseSqlServerRepository_RegistersByConnectionString_AndResolves()
        {
            //Arrange
            var builder = new ContainerBuilder().RegisterNSagaComponents();
            builder.RegisterType<SqlSagaRepository>().As<ISagaRepository>();
            builder.Register(c => new ConnectionFactory(@"Server=(localdb)\v12.0;Database=NSaga-Testing")).As<IConnectionFactory>();
            var container = builder.Build();

            // Act
            var repository = container.Resolve<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull().And.BeOfType<SqlSagaRepository>();
        }
    }
}
