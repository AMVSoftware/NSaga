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
            act.ShouldThrow<ArgumentException>();
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
            act.ShouldThrow<ArgumentException>();
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
            act.ShouldThrow<ArgumentException>();
        }


        [Fact]
        public void Initiate_Saves_IntoRepository()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository, new StubSagaServiceLocator());
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);
            
            // Act
            sut.Consume(initiatingMessage);

            // Assert
            sagaRepository.Sagas.Should().HaveCount(1);
            var saga = sagaRepository.Sagas[correlationId];
            saga.CorrelationId.Should().Be(correlationId);
        }


        [Fact]
        public void Initiate_InititatesSaga_AssignsCorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository, new StubSagaServiceLocator());
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = sagaRepository.Sagas[correlationId];
            saga.CorrelationId.Should().Be(correlationId);
            saga.SagaData.IsInitialised.Should().BeTrue();
        }


        [Fact]
        public void Initiate_CalledInitiate_CheckData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var sagaRepository = new SagaRepositoryInMemoryStub();
            var sut = CreateSut(sagaRepository, new StubSagaServiceLocator());
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            var saga = sagaRepository.Sagas[correlationId];
            saga.SagaData.IsInitialised.Should().BeTrue();
        }



        private static SagaMediator CreateSut(ISagaRepository repository = null, IServiceLocator serviceLocator = null)
        {
            repository = repository ?? new SagaRepositoryInMemoryStub();
            serviceLocator = serviceLocator ?? new StubSagaServiceLocator();
            var sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorTests).Assembly);
            return sut;
        }







        // message that does not inititate anything
        public class MyFakeInitiatingMessage : IInitiatingSagaMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
