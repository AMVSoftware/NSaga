using System;
using FluentAssertions;
using NSaga;
using NSaga.SimpleInjector;
using SimpleInjector;
using Xunit;


namespace Tests.SimpleInjector
{
    public class SimpleInjectorNSagaIntegrationTests
    {
        [Fact]
        public void Basic_Registration_Can_Resolve()
        {
            //Arrange
            var container = new Container();

            // Act
            container.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());

            // Assert
            var mediator = container.GetInstance<ISagaMediator>();
            mediator.Should().NotBeNull()
                .And.BeOfType<SagaMediator>();
        }
    }
}
