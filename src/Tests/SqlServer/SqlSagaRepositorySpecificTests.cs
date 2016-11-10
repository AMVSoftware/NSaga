using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NSaga;
using NSaga.SqlServer;
using PetaPoco;
using Xunit;


namespace Tests.SqlServer
{
    [AutoRollback]
    public class SqlSagaRepositorySpecificTests
    {
        private readonly SqlSagaRepository sut;
        private readonly Database database;

        public SqlSagaRepositorySpecificTests()
        {
            sut = new SqlSagaRepository("TestingConnectionString", new DumbSagaFactory(), new JsonNetSerialiser());
            database = new Database("TestingConnectionString");
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
        public void Save_Persists_SagaData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            var expectedGuid = Guid.NewGuid();
            saga.SagaData.SomeGuid = expectedGuid;

            // Act
            sut.Save(saga);

            // Assert
            var restoredSaga = DatabaseHelpers.GetSagaData(database, correlationId);
            restoredSaga.Should().NotBeNull();
            restoredSaga.CorrelationId.Should().Be(correlationId);
            restoredSaga.BlobData.Should().ContainEquivalentOf(expectedGuid.ToString());
        }


        [Fact]
        public void Save_Persists_Headers()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            var expectedValue = Guid.NewGuid().ToString();
            saga.Headers = new Dictionary<string, string>() { { "key", expectedValue } };

            // Act
            sut.Save(saga);

            // Assert
            var restoredHeaders = DatabaseHelpers.GetSagaHeaders(database, correlationId);
            restoredHeaders.Should().HaveCount(1);
            restoredHeaders.First().Value.Should().Be(expectedValue);
        }


        [Fact]
        public void Save_Updates_ExistingSagaData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expectedGuid = Guid.NewGuid();
            var saga = new MySaga
            {
                CorrelationId = correlationId,
                SagaData = { SomeGuid = Guid.NewGuid(), },
            };
            database.Insert(new SagaData() { CorrelationId = correlationId, BlobData = JsonConvert.SerializeObject(saga.SagaData) });

            saga.SagaData.SomeGuid = expectedGuid;

            // Act
            sut.Save(saga);

            // Assert
            var updatedData = DatabaseHelpers.GetSagaData(database, correlationId);
            updatedData.Should().NotBeNull();
            updatedData.BlobData.Should().ContainEquivalentOf(expectedGuid.ToString());
        }


        [Fact]
        public void Save_Updates_ExistingHeaders()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var expectedValue = Guid.NewGuid().ToString();
            var saga = new MySaga
            {
                CorrelationId = correlationId,
                Headers = new Dictionary<string, string>() { { "SomeKey", Guid.NewGuid().ToString() } },
            };
            database.Insert(new SagaData() { CorrelationId = correlationId, BlobData = JsonConvert.SerializeObject(saga.SagaData) });
            database.Insert(new SagaHeaders() { CorrelationId = correlationId, Key = saga.Headers.First().Key, Value = saga.Headers.First().Value });

            saga.Headers["SomeKey"] = expectedValue;

            // Act
            sut.Save(saga);

            // Assert
            var updatedHeaders = DatabaseHelpers.GetSagaHeaders(database, correlationId);
            updatedHeaders.Should().HaveCount(1);
            updatedHeaders.First().Value.Should().Be(expectedValue);
        }



        [Fact]
        public void Complete_Removes_Data()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga
            {
                CorrelationId = correlationId,
            };
            database.Insert(new SagaData() { CorrelationId = correlationId, BlobData = JsonConvert.SerializeObject(saga.SagaData) });

            // Act
            sut.Complete(saga);

            // Assert
            var updatedData = DatabaseHelpers.GetSagaData(database, correlationId);
            updatedData.Should().BeNull();
        }

        [Fact]
        public void Complete_Removes_Headers()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga
            {
                CorrelationId = correlationId,
                Headers = new Dictionary<string, string>() { { "SomeKey", Guid.NewGuid().ToString() } },
            };
            database.Insert(new SagaData() { CorrelationId = correlationId, BlobData = JsonConvert.SerializeObject(saga.SagaData) });
            database.Insert(new SagaHeaders() { CorrelationId = correlationId, Key = saga.Headers.First().Key, Value = saga.Headers.First().Value });


            // Act
            sut.Complete(saga);

            // Assert
            var updatedHeaders = DatabaseHelpers.GetSagaHeaders(database, correlationId);
            updatedHeaders.Should().HaveCount(0);
        }
    }
}