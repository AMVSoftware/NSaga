using System;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using Xunit;


namespace Tests
{
    public class SagaMediatorTests
    {
        [Fact]
        public void Initiate_NoCorrelationId_Throws()
        {
            // Arrange
            var sut = CreateSut();
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
            var repository = new SagaRepositoryInMemoryStub();
            repository.Sagas.Add(correlationId, new MySaga() { CorrelationId = correlationId });
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);
            var sut = CreateSut(repository);

            // Act
            Action act = () => sut.Consume(initiatingMessage);

            // Assert
            act.ShouldThrow<ArgumentException>().Which.Message.Contains("Trying to initiate the same saga twice");
        }


        [Fact]
        public void Initiate_NoSagaTypes_Throws()
        {
            // Arrange
            var sut = CreateSut();
            var initiatingMessage = new MyFakeInitiatingMessage() {CorrelationId = Guid.NewGuid()};

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
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);
            
            // Act
            sut.Consume(initiatingMessage);

            // Assert
            sagaRepository.Sagas.Should().HaveCount(1);
            var saga = (MySaga)sagaRepository.Sagas[correlationId];
            saga.CorrelationId.Should().Be(correlationId);
        }


        [Fact]
        public void Initiate_InititatesSaga_AssignsCorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = (MySaga)sagaRepository.Sagas[correlationId];
            saga.CorrelationId.Should().Be(correlationId);
            saga.SagaData.IsInitialised.Should().BeTrue();
        }


        [Fact]
        public void Initiate_CalledInitiate_CheckData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = (MySaga)sagaRepository.Sagas[correlationId];
            saga.SagaData.IsInitialised.Should().BeTrue();
        }


        [Fact]
        public void Initiate_SagaWithErrors_ReturnsErrors()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new InitiatingSagaWithErrors(correlationId);

            // Act
            var errors = sut.Consume(initiatingMessage);

            // Assert
            errors.Should().HaveCount(1).And.Contain("This is not right!");
            var saga = (SagaWithErrors)sagaRepository.Sagas[correlationId];
            saga.SagaData.IsInitiated.Should().BeTrue();
        }


        [Fact]
        public void Initiate_MultipleInitiator_Throws()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);

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
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new MySagaAdditionalInitialser(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = (MySaga)sagaRepository.Sagas[correlationId];
            saga.SagaData.IsInitialised.Should().BeFalse();
            saga.SagaData.IsAdditionalInitialiserCalled.Should().BeTrue();
            saga.CorrelationId.Should().Be(correlationId);
        }


        [Fact]
        public void Initiate_SageDoesNotInitiateDate_InitiatesSagaDataObject()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository);
            var initiatingMessage = new InitiatingSagaWithErrors(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = (SagaWithErrors)sagaRepository.Sagas[correlationId];
            saga.SagaData.Should().NotBeNull();
        }




        private static SagaMediator CreateSut(ISagaRepository repository = null, IServiceLocator serviceLocator = null)
        {
            repository = repository ?? new SagaRepositoryInMemoryStub();
            serviceLocator = serviceLocator ?? new StubSagaServiceLocator();
            var sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorTests).Assembly);
            return sut;
        }
    }
}
