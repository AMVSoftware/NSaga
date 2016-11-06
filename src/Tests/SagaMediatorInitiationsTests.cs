using System;
using System.Linq;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using TinyIoC;
using Xunit;


namespace Tests
{
    public class SagaMediatorInitiationsTests
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;

        public SagaMediatorInitiationsTests()
        {
            var container = TinyIoCContainer.Current;
            //container.Register()

            var assembliesToScan = typeof(SagaMediatorInitiationsTests).Assembly;

            container.RegisterMultiple(typeof(ISaga<>), assembliesToScan.GetTypes().Where(t => typeof(ISaga<>).IsAssignableFrom(t)).ToList());
            container.RegisterMultiple(typeof(InitiatedBy<>), assembliesToScan.GetTypes().Where(t => typeof(InitiatedBy<>).IsAssignableFrom(t)).ToList());
            container.RegisterMultiple(typeof(ConsumerOf<>), assembliesToScan.GetTypes().Where(t => typeof(ConsumerOf<>).IsAssignableFrom(t)).ToList());

            var serviceLocator = new TinyIocSagaFactory(container);
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, new [] { new NullPipelineHook()}, assembliesToScan);
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
    }
}
