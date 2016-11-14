using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NSaga;
using Xunit;


namespace Tests.Implementations
{
    [Collection("InMemorySagaRepository")]
    public class InMemorySagaRepositorySpecificTests : IDisposable
    {
        private readonly InMemorySagaRepository sut;

        public InMemorySagaRepositorySpecificTests()
        {
            sut = new InMemorySagaRepository(new JsonNetSerialiser(), new DumbSagaFactory());
        }


        [Fact]
        public void Find_NoSaga_ReturnsNull()
        {
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
            var saga = new MySaga { CorrelationId = correlationId };

            // Act
            sut.Save(saga);

            // Assert
            var dataDictionary = GetDataDictionary();
            dataDictionary.Should().NotBeNull()
                                       .And.HaveCount(1)
                                       .And.ContainKey(correlationId);
        }

        [Fact]
        public void Save_Persists_Headers()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };

            // Act
            sut.Save(saga);

            // Assert
            var headersDictionary = GetHeadersDictionary();
            headersDictionary.Should().NotBeNull()
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
            GetDataDictionary()[correlationId] = JsonConvert.SerializeObject(sagaData);

            // Act
            var saga = sut.Find<MySaga>(correlationId);

            // Assert
            saga.Should().NotBeNull();
            saga.SagaData.SomeGuid.Should().Be(expectedGuid);
        }


        [Fact]
        public void Find_Returns_PersistedHeaders()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expectedGuid = Guid.NewGuid();
            var sagaData = new MySagaData() { SomeGuid = expectedGuid };

            GetDataDictionary()[correlationId] = JsonConvert.SerializeObject(sagaData);
            GetHeadersDictionary()[correlationId] = JsonConvert.SerializeObject(new Dictionary<String, String>() { { expectedGuid.ToString(), expectedGuid.ToString() } });

            // Act
            var saga = sut.Find<MySaga>(correlationId);

            // Assert
            saga.Should().NotBeNull();
            saga.Headers.Should().HaveCount(1).And.ContainKey(expectedGuid.ToString());
            saga.Headers[expectedGuid.ToString()].Should().Be(expectedGuid.ToString());
        }



        [Fact]
        public void Complete_Removes_SagaFromStorage()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga();
            saga.CorrelationId = correlationId;
            sut.Save(saga);

            // Act
            sut.Complete(saga);

            // Assert
            GetDataDictionary().Should().NotContainKey(correlationId);
        }


        [Fact]
        public void Complete_Removes_HeadersFromStorage()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga();
            saga.CorrelationId = correlationId;
            sut.Save(saga);

            // Act
            sut.Complete(saga);

            // Assert
            var concurrentDictionary = GetHeadersDictionary();
            concurrentDictionary.Should().NotContainKey(correlationId);
        }


        [Fact]
        public void MultipleRepo_Instances_ShareDataStorage()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expected = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId, SagaData = new MySagaData() { SomeGuid = expected } };
            sut.Save(saga);

            // Act
            var anotherInstance = new InMemorySagaRepository(new JsonNetSerialiser(), new DumbSagaFactory());
            var restoredSaga = anotherInstance.Find<MySaga>(correlationId);

            // Assert
            restoredSaga.SagaData.SomeGuid.Should().Be(expected);
        }


        [Fact]
        public void ClearData_Removes_Data()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expected = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            sut.Save(saga);

            // Act
            InMemorySagaRepository.ResetStorage();

            // Assert
            var deletedSaga = sut.Find<MySaga>(correlationId);
            deletedSaga.Should().BeNull();
        }



        private ConcurrentDictionary<Guid, string> GetDataDictionary()
        {
            return (ConcurrentDictionary<Guid, String>)NSagaReflection.GetPrivate(sut, "DataDictionary");
        }

        private ConcurrentDictionary<Guid, string> GetHeadersDictionary()
        {
            return (ConcurrentDictionary<Guid, String>)NSagaReflection.GetPrivate(sut, "HeadersDictionary");
        }

        public void Dispose()
        {
            InMemorySagaRepository.ResetStorage();
        }
    }
}
