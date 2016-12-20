using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using NSaga;
using NSaga.AzureTables;
using Xunit;

namespace Tests.AzureTables
{
    public class AzureTablesRepositorySpecificTests : IDisposable
    {
        private readonly AzureTablesSagaRepository sut;
        private readonly TableClientFactory tableClientFactory;

        public AzureTablesRepositorySpecificTests()
        {
            var connString = AzureTablesHelper.GetConnectionString();

            tableClientFactory = new TableClientFactory(connString);
            sut = new AzureTablesSagaRepository(tableClientFactory, new JsonNetSerialiser(), new DumbSagaFactory());
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
            var restoredSaga = GetStoredModel(correlationId);
            restoredSaga.Should().NotBeNull();
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
            var restoredSaga = GetStoredModel(correlationId);
            var restoredHeaders = JsonConvert.DeserializeObject<Dictionary<String, String>>(restoredSaga.Headers);

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
            SetStoredModel(correlationId, new AzureTablesSagaRepository.StorageModel()
            {
                BlobData = JsonConvert.SerializeObject(saga.SagaData),
            });

            saga.SagaData.SomeGuid = expectedGuid;

            // Act
            sut.Save(saga);

            // Assert
            var updatedData = GetStoredModel(correlationId);
            updatedData.Should().NotBeNull();
            updatedData.BlobData.Should().ContainEquivalentOf(expectedGuid.ToString());
        }


        private AzureTablesSagaRepository.StorageModel GetStoredModel(Guid correlationId)
        {
            var table = GetTable();

            var operation = TableOperation.Retrieve<AzureTablesSagaRepository.StorageModel>("nsaga", correlationId.ToString());
            var result = table.Execute(operation);

            return (AzureTablesSagaRepository.StorageModel)result.Result;
        }

        private void SetStoredModel(Guid correlationId, AzureTablesSagaRepository.StorageModel persistedModel)
        {
            var table = GetTable();
            persistedModel.RowKey = correlationId.ToString();

            var insertOperation = TableOperation.Insert(persistedModel);
            table.Execute(insertOperation);
        }


        private CloudTable GetTable()
        {
            var client = tableClientFactory.CreateTableClient();
            var table = client.GetTableReference("nsaga");
            table.CreateIfNotExists();
            return table;
        }



        public void Dispose()
        {
            var connectionString = AzureTablesHelper.GetConnectionString();

            var tableClient = new TableClientFactory(connectionString).CreateTableClient();

            var table = tableClient.GetTableReference("nsaga");

            table.DeleteIfExists();
        }
    }
}
