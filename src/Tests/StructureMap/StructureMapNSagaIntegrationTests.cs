using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSaga;
using NSaga.StructureMap;
using StructureMap;
using Xunit;

namespace Tests.StructureMap
{
    public class StructureMapNSagaIntegrationTests
    {
        [Theory]
        [InlineData(typeof(ISagaMediator), typeof(SagaMediator))]
        [InlineData(typeof(ISagaRepository), typeof(InMemorySagaRepository))]
        [InlineData(typeof(ISagaFactory), typeof(StructureMapSagaFactory))]
        [InlineData(typeof(IMessageSerialiser), typeof(JsonNetSerialiser))]
        [InlineData(typeof(ISaga<MySagaData>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaInitiatingMessage>), typeof(MySaga))]
        [InlineData(typeof(ConsumerOf<MySagaConsumingMessage>), typeof(MySaga))]
        [InlineData(typeof(InitiatedBy<MySagaAdditionalInitialser>), typeof(MySaga))]
        public void DefaultRegistration_Resolves_DefaultComponents(Type requestedType, Type expectedImplementation)
        {
            //Arrange
            var container = new Container().RegisterNSagaComponents();

            // Act
            var result = container.GetInstance(requestedType);

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType(expectedImplementation);
        }
    }
}
