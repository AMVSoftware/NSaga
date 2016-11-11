using System;
using System.Reflection;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests
{
    public class SagaMediatorConsumingTests
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;

        public SagaMediatorConsumingTests()
        {
            var container = TinyIoCContainer.Current;
            container.RegisterSagas(typeof(SagaMediatorConsumingTests).Assembly);
            var serviceLocator = new TinyIocSagaFactory(container);
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, new[] { new NullPipelineHook() });
        }



        [Fact]
        public void Consume_MessageWithoutId_ThrowsException()
        {
            //Arrange
            var message = new MySagaConsumingMessage();

            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("CorrelationId was not provided in the message");
        }


        [Fact(Skip = "Saga diagnostics should be done separately")]
        public void Consume_MessageWithoutSaga_Throws()
        {
            //Arrange
            var message = new MyFakeInitiatingMessage(Guid.NewGuid());

            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("is not consumed by any Sagas");
        }


        [Fact]
        public void Consume_NoSagaExists_Throws()
        {
            //Arrange
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
            sut.Consume(new MySagaInitiatingMessage(correlationId));
            var message = new MySagaConsumingMessage(correlationId);

            // Act
            sut.Consume(message);

            // Assert
            var saga = repository.Find<MySaga>(correlationId);
            saga.SagaData.IsConsumingMessageReceived.Should().BeTrue();
        }


        [Fact]
        public void Consume_MessageWithErrors_ErrorsReturned()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            sut.Consume(new ActuallyInitiatingSagaWithErrors(correlationId));
            var message = new GetSomeConsumedErrorsForSagaWithErrors(correlationId);

            // Act
            var result = sut.Consume(message);

            // Assert
            result.Errors.Should().HaveCount(1).And.Contain("This is not right!");
        }


        [Fact]
        public void Consume_MessageWithErrors_SagaDataNotPersisted()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            sut.Consume(new ActuallyInitiatingSagaWithErrors(correlationId));
            var message = new GetSomeConsumedErrorsForSagaWithErrors(correlationId);

            // Act
            sut.Consume(message);

            // Assert
            var saga = repository.Find<SagaWithErrors>(correlationId);
            saga.SagaData.IsConsumed.Should().BeFalse();

        }
    }
}
