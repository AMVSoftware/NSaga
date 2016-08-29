using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using Xunit;

namespace Tests
{
    public class SagaMediatorConsumingTests
    {
        [Fact]
        public void Consume_MessageWithoutId_ThrowsException()
        {
            //Arrange
            var sut = CreateSut();
            var message = new MySagaConsumingMessage();
            
            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("CorrelationId was not provided in the message");
        }


        private static SagaMediator CreateSut(ISagaRepository repository = null, IServiceLocator serviceLocator = null)
        {
            repository = repository ?? new SagaRepositoryInMemoryStub();
            serviceLocator = serviceLocator ?? new StubSagaServiceLocator();
            var sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorInitiationsTests).Assembly);
            return sut;
        }
    }
}
