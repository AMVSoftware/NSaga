using System;
using System.Collections.Generic;
using FluentAssertions;
using NSaga;
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
            var correlationId = Guid.NewGuid();
            var repository = new SagaRepositoryInMemoryStub();

            //TODO
        }



        private static SagaMediator CreateSut(ISagaRepository repository = null, IServiceLocator serviceLocator = null)
        {
            repository = repository ?? NSubstitute.Substitute.For<ISagaRepository>();
            serviceLocator = serviceLocator ?? NSubstitute.Substitute.For<IServiceLocator>();
            var sut = new SagaMediator(repository, serviceLocator, typeof(SagaMediatorTests).Assembly);
            return sut;
        }




        public class SagaRepositoryInMemoryStub : ISagaRepository
        {
            public SagaRepositoryInMemoryStub()
            {
                Sagas = new Dictionary<Guid, ISaga<MySagaData>>();
            }

            public Dictionary<Guid, ISaga<MySagaData>> Sagas { get; set; }


            public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
            {
                return (TSaga)Sagas[correlationId];
            }

            public void Save<TSaga>(TSaga saga) where TSaga : class
            {
                throw new NotImplementedException();
            }

            public void Complete<TSaga>(TSaga saga) where TSaga : class
            {
                throw new NotImplementedException();
            }
        }
    }
}
