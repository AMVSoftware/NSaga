using System;
using FluentAssertions;
using Newtonsoft.Json;
using NSaga;
using Tests.Stubs;
using Xunit;

namespace Tests.Implementations
{
    public class InMemorySagaPersisterTests
    {
        [Fact]
        public void Find_NoSaga_ReturnsNull()
        {
            //Arrange
            var sut = CreateSut();

            // Act
            var result = sut.Find<MySaga>(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public void Save_Returns_SavedData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga();
            saga.CorrelationId = correlationId;
            var sut = CreateSut();

            // Act
            sut.Save(saga);

            // Assert
            sut.DataDictionary.Should().NotBeNull()
                                       .And.HaveCount(1)
                                       .And.ContainKey(correlationId);
        }


        [Fact]
        public void Find_Returns_ExistingSaga()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expectedGuid = Guid.NewGuid();
            var sagaData = new MySagaData() { SomeGuid = expectedGuid };
            var sut = CreateSut();
            sut.DataDictionary[correlationId] = JsonConvert.SerializeObject(sagaData);

            // Act
            var saga = sut.Find<MySaga>(correlationId);

            // Assert
            saga.Should().NotBeNull();
            saga.SagaData.SomeGuid.Should().Be(expectedGuid);
        }



        [Fact]
        public void Complete_Removes_SagaFromStorage()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga();
            saga.CorrelationId = correlationId;
            var sut = CreateSut();
            sut.Save(saga);

            // Act
            sut.Complete(saga);

            // Assert
            sut.DataDictionary.Should().NotContainKey(correlationId);
        }


        public InMemorySagaRepository CreateSut()
        {
            var sut = new InMemorySagaRepository(new JsonNetSerialiser(), new StubSagaServiceLocator());
            return sut;
        }
    }
}
