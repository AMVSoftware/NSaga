using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSaga;
using NSaga.Pipeline;
using NSaga.SimpleInjector;
using NSaga.SqlServer;
using SimpleInjector;
using Tests.Stubs;
using Xunit;


namespace Tests.SimpleInjector
{
    public class SimpleInjectorNSagaIntegrationTests
    {
        [Fact]
        public void Basic_Registration_ValidContainer()
        {
            //Arrange
            var container = new Container();

            // Act
            container.RegisterNSagaComponents();

            // Assert
            container.Verify();
        }


        [Fact]
        public void Basic_Registration_CanResolveByInitiatedMessage()
        {
            //Arrange
            var container = new Container();

            // Act
            container.RegisterNSagaComponents();

            // Assert
            var mediator = container.GetInstance<InitiatedBy<MySagaInitiatingMessage>>();
            mediator.Should().NotBeNull()
                .And.BeOfType<MySaga>();
        }

        [Fact]
        public void Basic_Registration_CanResolveConsuming_Messages()
        {
            //Arrange
            var container = new Container();

            // Act
            container.RegisterNSagaComponents();

            // Assert
            var mediator = container.GetInstance<ConsumerOf<MySagaConsumingMessage>>();
            mediator.Should().NotBeNull()
                .And.BeOfType<MySaga>();
        }

        [Fact]
        public void Basic_Registration_Can_Resolve()
        {
            //Arrange
            var container = new Container();

            // Act
            container.RegisterNSagaComponents();

            // Assert
            var mediator = container.GetInstance<ISagaMediator>();
            mediator.Should().NotBeNull()
                .And.BeOfType<SagaMediator>();
        }

        [Fact]
        public void OverrideGeneric_Repository_Complies()
        {
            //Arrange
            var container = new Container();
            container.RegisterNSagaComponents().UseSagaRepository<NullSagaRepository>();

            // Act
            var repository = container.GetInstance<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull()
                .And.BeOfType<NullSagaRepository>();
        }


        [Fact]
        public void OverrideByType_Repository_Complies()
        {
            //Arrange
            var container = new Container();
            container.RegisterNSagaComponents()
                     .UseSagaRepository(() => new SqlSagaRepository("TestingConnectionString", container.GetInstance<ISagaFactory>(), container.GetInstance<IMessageSerialiser>()));

            // Act
            var repository = container.GetInstance<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull()
                .And.BeOfType<SqlSagaRepository>();
        }


        [Fact]
        public void AddPipline_Adds_ToCollection()
        {
            //Arrange
            var container = new Container();
            container.RegisterNSagaComponents().AddSagaPipelineHook<NullPipelineHook>();

            // Act
            var collection = container.GetInstance<IEnumerable<IPipelineHook>>();

            // Assert
            collection.Should().HaveCount(2);
            collection.Should().Contain(h => h.GetType() == typeof(NullPipelineHook));
            collection.Should().Contain(h => h.GetType() == typeof(MetadataPipelineHook));
        }


        [Fact]
        public void UseMessageSerialiser_Overrides_Default()
        {
            //Arrange
            var container = new Container();
            container.RegisterNSagaComponents().UseMessageSerialiser<NullMessageSerialiser>();

            // Act
            var result = container.GetInstance<IMessageSerialiser>();

            // Assert
            result.Should().NotBeNull().And.BeOfType<NullMessageSerialiser>();
        }
    }
}
