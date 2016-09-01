using System;
using FluentAssertions;
using NSaga;
using NSaga.Implementations;
using Tests.Stubs;
using Xunit;


namespace Tests
{
    public class SagaMediatorInitiationsTests
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;

        public SagaMediatorInitiationsTests()
        {
            var serviceLocator = new DumbServiceLocator();
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorInitiationsTests).Assembly);
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


        [Fact]
        public void Initiate_NoSagaTypes_Throws()
        {
            // Arrange
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
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);
            
            // Act
            sut.Consume(initiatingMessage);

            // Assert
            repository.DataDictionary.Should().HaveCount(1);
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
        public void Initiate_SagaWithErrors_ReturnsErrors()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new InitiatingSagaWithErrors(correlationId);

            // Act
            var errors = sut.Consume(initiatingMessage);

            // Assert
            errors.Should().HaveCount(1).And.Contain("This is not right!");
            var saga = repository.Find<SagaWithErrors>(correlationId);
            saga.SagaData.IsInitiated.Should().BeTrue();
        }


        [Fact]
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


        [Fact]
        public void Initiate_SagaDoesNotInitiateDate_InitiatesSagaDataObject()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var initiatingMessage = new InitiatingSagaWithErrors(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = repository.Find<SagaWithErrors>(correlationId);
            saga.SagaData.Should().NotBeNull();

        }




        //private static SagaMediator CreateSut(ISagaRepository repository = null, IServiceLocator serviceLocator = null)
        //{
        //    var sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorInitiationsTests).Assembly);
        //    return sut;
        //}
    }
}
