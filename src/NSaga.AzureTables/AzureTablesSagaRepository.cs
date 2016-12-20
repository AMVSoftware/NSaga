using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace NSaga.AzureTables
{
    public class AzureTablesSagaRepository : ISagaRepository
    {
        private readonly ITableClientFactory tableClientFactory;
        private readonly IMessageSerialiser messageSerialiser;
        private readonly ISagaFactory sagaFactory;
        private const string PartitionKey = "nsaga";
        private const string TableName = "nsaga";

        public AzureTablesSagaRepository(ITableClientFactory tableClientFactory, IMessageSerialiser messageSerialiser, ISagaFactory sagaFactory)
        {
            Guard.ArgumentIsNotNull(tableClientFactory, nameof(tableClientFactory));
            Guard.ArgumentIsNotNull(sagaFactory, nameof(sagaFactory));
            Guard.ArgumentIsNotNull(messageSerialiser, nameof(messageSerialiser));


            this.tableClientFactory = tableClientFactory;
            this.messageSerialiser = messageSerialiser;
            this.sagaFactory = sagaFactory;
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            var client = tableClientFactory.CreateTableClient();
            var sagasTable = GetSagaTable(client);

            var retrieveOperation = TableOperation.Retrieve<StorageModel>(PartitionKey, correlationId.ToString());

            var retrieveResult = sagasTable.Execute(retrieveOperation);

            if (retrieveResult.Result == null)
            {
                return null;
            }

            var storedModel = (StorageModel)retrieveResult.Result;
            var sagaDataType = NSagaReflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));
            var sagaData = messageSerialiser.Deserialise(storedModel.BlobData, sagaDataType);

            var sagaInstance = sagaFactory.ResolveSaga<TSaga>();
            sagaInstance.CorrelationId = correlationId;
            sagaInstance.Headers = messageSerialiser.Deserialise<Dictionary<String, String>>(storedModel.Headers);
            NSagaReflection.Set(sagaInstance, "SagaData", sagaData);

            return sagaInstance;
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var client = tableClientFactory.CreateTableClient();
            var sagasTable = GetSagaTable(client);

            var sagaData = NSagaReflection.Get(saga, "SagaData");
            var serialisedData = messageSerialiser.Serialise(sagaData);

            var storageModel = new StorageModel()
            {
                Headers = messageSerialiser.Serialise(saga.Headers),
                BlobData = serialisedData,
                RowKey = saga.CorrelationId.ToString(),
                PartitionKey = PartitionKey,
            };

            var insertOperation = TableOperation.Insert(storageModel);

            sagasTable.Execute(insertOperation);
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            Complete(saga.CorrelationId);
        }

        public void Complete(Guid correlationId)
        {
            var client = tableClientFactory.CreateTableClient();
            var sagasTable = GetSagaTable(client);
            var retrieveOperation = TableOperation.Retrieve<StorageModel>(PartitionKey, correlationId.ToString());
            var storedSaga = sagasTable.Execute(retrieveOperation);

            if (storedSaga.Result == null)
            {
                throw new Exception($"Unable to find saga with correlationId {correlationId}");
            }

            var deleteOperation = TableOperation.Delete((StorageModel)storedSaga.Result);
            sagasTable.Execute(deleteOperation);
        }



        private CloudTable GetSagaTable(CloudTableClient client)
        {
            var table = client.GetTableReference(TableName);
            table.CreateIfNotExists();

            return table;
        }

        private class StorageModel : TableEntity
        {
            public String Headers { get; set; }

            public String BlobData { get; set; }
        }
    }
}
