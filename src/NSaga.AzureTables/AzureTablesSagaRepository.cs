using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace NSaga.AzureTables
{
    /// <summary>
    /// Implementation of <see cref="ISagaRepository"/> that uses Azure Tables to store Saga data.
    /// </summary>
    /// <seealso cref="NSaga.ISagaRepository" />
    public class AzureTablesSagaRepository : ISagaRepository
    {
        private readonly ITableClientFactory tableClientFactory;
        private readonly IMessageSerialiser messageSerialiser;
        private readonly ISagaFactory sagaFactory;
        private const string PartitionKey = "nsaga";
        private const string TableName = "nsaga";

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTablesSagaRepository"/> class.
        /// </summary>
        /// <param name="tableClientFactory">The table client factory.</param>
        /// <param name="messageSerialiser">The message serialiser.</param>
        /// <param name="sagaFactory">The saga factory.</param>
        public AzureTablesSagaRepository(ITableClientFactory tableClientFactory, IMessageSerialiser messageSerialiser, ISagaFactory sagaFactory)
        {
            Guard.ArgumentIsNotNull(tableClientFactory, nameof(tableClientFactory));
            Guard.ArgumentIsNotNull(sagaFactory, nameof(sagaFactory));
            Guard.ArgumentIsNotNull(messageSerialiser, nameof(messageSerialiser));


            this.tableClientFactory = tableClientFactory;
            this.messageSerialiser = messageSerialiser;
            this.sagaFactory = sagaFactory;
        }


        /// <summary>
        /// Finds and returns saga instance with the given correlation ID.
        /// Actually creates an instance of saga from Saga factory, retrieves SagaData and Headers from the storage and populates the instance with these.
        /// </summary>
        /// <typeparam name="TSaga">Type of saga we are looking for</typeparam>
        /// <param name="correlationId">CorrelationId to identify the saga</param>
        /// <returns>
        /// An instance of the saga. Or Null if there is no saga with this ID.
        /// </returns>
        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            var sagasTable = GetSagaTable();

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

        /// <summary>
        /// Persists the instance of saga into the database storage.
        /// Actually stores SagaData and Headers. All other variables in saga are not persisted
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga instance</param>
        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var sagasTable = GetSagaTable();

            var sagaData = NSagaReflection.Get(saga, "SagaData");
            var serialisedData = messageSerialiser.Serialise(sagaData);

            var storageModel = new StorageModel()
            {
                RowKey = saga.CorrelationId.ToString(),
                Headers = messageSerialiser.Serialise(saga.Headers),
                BlobData = serialisedData,
            };

            var insertOperation = TableOperation.InsertOrReplace(storageModel);

            sagasTable.Execute(insertOperation);
        }

        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga to be deleted</param>
        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            Complete(saga.CorrelationId);
        }

        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <param name="correlationId">Correlation Id for the saga</param>
        /// <exception cref="System.Exception"></exception>
        public void Complete(Guid correlationId)
        {
            var sagasTable = GetSagaTable();
            var retrieveOperation = TableOperation.Retrieve<StorageModel>(PartitionKey, correlationId.ToString());
            var storedSaga = sagasTable.Execute(retrieveOperation);

            if (storedSaga.Result == null)
            {
                throw new Exception($"Unable to find saga with correlationId {correlationId}");
            }

            var deleteOperation = TableOperation.Delete((StorageModel)storedSaga.Result);
            sagasTable.Execute(deleteOperation);
        }



        private CloudTable GetSagaTable()
        {
            var client = tableClientFactory.CreateTableClient();
            var table = client.GetTableReference(TableName);
            table.CreateIfNotExists();

            return table;
        }

        internal class StorageModel : TableEntity
        {
            public StorageModel()
            {
                base.PartitionKey = AzureTablesSagaRepository.PartitionKey;
            }

            public String Headers { get; set; }

            public String BlobData { get; set; }
        }
    }
}
