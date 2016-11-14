using System;
using System.Collections.Concurrent;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using Xunit;


namespace Tests
{
    [Collection("InMemorySagaRepository")]
    public class SagaMediatorInitiationsTests : IDisposable
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;

        public SagaMediatorInitiationsTests()
        {
            var container = new TinyIoCContainer();
            container.RegisterSagas(typeof(SagaMediatorInitiationsTests).Assembly);

            var serviceLocator = new TinyIocSagaFactory(container);
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, new[] { new NullPipelineHook() });
        }


        [Fact]
        public void Initiate_NoCorrelationId_Throws()
        {
            // Arrange
            var initiatingMessage = new MySagaInitiatingMessage();

            // Act
            Action act = () => sut.Consume(initiatingMessage);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("CorrelationId was not provided in the message");
        }


        [Fact]
        public void Initiate_SagaAlreadyExists_Throws()
        {
            // Arrange
            var correlationId = Guid.NewGuid();
            repository.Save(new MySaga() { CorrelationId = correlationId });
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            Action act = () => sut.Consume(initiatingMessage);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("Trying to initiate the same saga twice");
        }


        [Fact(Skip = "Check for sagas should be done separately")]
        public void Initiate_NoSagaTypes_Throws()
        {
            // Arrange
            var initiatingMessage = new MyFakeInitiatingMessage(Guid.NewGuid());

            // Act
            Action act = () => sut.Consume(initiatingMessage);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("is not initiating any Sagas");
        }


        [Fact]
        public void Initiate_Saves_IntoRepository()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var data = InMemorySagaRepository.DataDictionary;
            data.Should().HaveCount(1);
            var saga = repository.Find<MySaga>(correlationId);
            saga.CorrelationId.Should().Be(correlationId);
        }


        [Fact]
        public void Initiate_InititatesSaga_AssignsCorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = repository.Find<MySaga>(correlationId);
            saga.CorrelationId.Should().Be(correlationId);
            saga.SagaData.IsInitialised.Should().BeTrue();
        }


        [Fact]
        public void Initiate_CalledInitiate_CheckData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = repository.Find<MySaga>(correlationId);
            saga.SagaData.IsInitialised.Should().BeTrue();
        }


        [Fact]
        public void Initiate_SagaWithErrors_SagaIsNotCreated()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new InitiatingSagaWithErrors(correlationId);

            // Act
            var operationResult = sut.Consume(initiatingMessage);

            // Assert
            operationResult.HasErrors.Should().BeTrue();
            var saga = repository.Find<SagaWithErrors>(correlationId);
            saga.Should().BeNull();
        }


        [Fact(Skip = "Check for sagas should be done separately")]
        public void Initiate_MultipleInitiator_Throws()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var message = new MultipleSagaInitiator(correlationId);

            // Act
            Action act = () => sut.Consume(message);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("initiating more than one saga");
        }




        [Fact]
        public void Initiate_AdditionalInitialser_CorrectlyCalled()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaAdditionalInitialser(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = repository.Find<MySaga>(correlationId);
            saga.SagaData.IsInitialised.Should().BeFalse();
            saga.SagaData.IsAdditionalInitialiserCalled.Should().BeTrue();
            saga.CorrelationId.Should().Be(correlationId);
        }

        public void Dispose()
        {
            InMemorySagaRepository.ResetStorage();
        }
    }
}
