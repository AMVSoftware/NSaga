using System;
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


        [Fact]
        public void Consume_MessageWithoutSaga_Throws()
        {
            //Arrange
            var sut = CreateSut();
            var message = new MyFakeInitiatingMessage();

            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("is not consumed by any Sagas");
        }


        [Fact]
        public void Consume_NoSagaExists_Throws()
        {
            //Arrange
            var sut = CreateSut();
            var message = new MySagaConsumingMessage(Guid.NewGuid());

            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("Saga with this CorrelationId does not exist");
        }


        [Fact]
        public void Consume_MessageConsumed_SagaPersisted()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            sut.Consume(new MySagaInitiatingMessage(correlationId));
            var message = new MySagaConsumingMessage(correlationId);

            // Act
            sut.Consume(message);

            // Assert
            var saga = (MySaga)sagaRepository.Sagas[correlationId];
            saga.SagaData.IsConsumingMessageReceived.Should().BeTrue();
        }


        [Fact]
        public void Consume_MessageWithErrors_ErrorsReturned()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            sut.Consume(new InitiatingSagaWithErrors(correlationId));
            var message = new GetSomeConsumedErrorsForSagaWithErrors(correlationId);

            // Act
            var result = sut.Consume(message);

            // Assert
            result.Should().HaveCount(1).And.Contain("This is not right!");
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
