using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace NSaga.AzureTables
{
    public class AzureTablesSagaRepository : ISagaRepository
    {
        private readonly ITableClientFactory tableClientFactory;
        private readonly IMessageSerialiser messageSerialiser;

        public AzureTablesSagaRepository(ITableClientFactory tableClientFactory, IMessageSerialiser messageSerialiser)
        {
            this.tableClientFactory = tableClientFactory;
            this.messageSerialiser = messageSerialiser;
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            var client = tableClientFactory.CreateTableClient();

            throw new NotImplementedException();
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var client = tableClientFactory.CreateTableClient();
            var sagasTable = GetSagaTable(client);

            var sagaData = NSagaReflection.Get(saga, "SagaData");
            var serialisedData = messageSerialiser.Serialise(sagaData);

            var storageModel = new StorageModel()
            {
                Headers = saga.Headers,
                CorrelationId = saga.CorrelationId,
                JsonData = serialisedData,
                RowKey = saga.CorrelationId.ToString(),
                PartitionKey = "nsaga",
            };

            var insertOperation = TableOperation.Insert(storageModel);

            throw new NotImplementedException();
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid correlationId)
        {
            throw new NotImplementedException();
        }



        private CloudTable GetSagaTable(CloudTableClient client)
        {
            var table = client.GetTableReference("nsaga");
            table.CreateIfNotExists();

            return table;
        }

        private class StorageModel : TableEntity
        {
            public Dictionary<String, String> Headers { get; set; }

            public String JsonData { get; set; }

            public Guid CorrelationId { get; set; }
        }
    }
}
