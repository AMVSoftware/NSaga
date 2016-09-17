using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco;

namespace NSaga.SqlServer
{
    public class SqlSagaRepository : ISagaRepository
    {
        public const string SagaDataTableName = "NSaga.Sagas";
        public const string HeadersTableName = "NSaga.Headers";

        private readonly IServiceLocator serviceLocator;
        private readonly Database database;
        private readonly IMessageSerialiser messageSerialiser;

        public SqlSagaRepository(string connectionStringName, IServiceLocator serviceLocator,
            IMessageSerialiser messageSerialiser)
        {
            this.messageSerialiser = messageSerialiser;
            this.serviceLocator = serviceLocator;
            this.database = new Database(connectionStringName);
        }

        public SqlSagaRepository(string connectionString, string providerName, IServiceLocator serviceLocator,
            IMessageSerialiser messageSerialiser)
        {
            this.messageSerialiser = messageSerialiser;
            this.serviceLocator = serviceLocator;
            this.database = new Database(connectionString, providerName);
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
        {
            var sql = Sql.Builder.Where("correlationId = @0", correlationId);
            var persistedData = database.SingleOrDefault<SagaData>(sql);

            if (persistedData == null)
            {
                return null;
            }

            var sagaInstance = serviceLocator.Resolve<TSaga>();
            var sagaDataType = Reflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));
            var sagaData = messageSerialiser.Deserialise(persistedData.BlobData, sagaDataType);

            var headersSql = Sql.Builder.Where("correlationId = @0", correlationId);
            var headersPersisted = database.Query<SagaHeaders>(headersSql);
            var headers = headersPersisted.ToDictionary(k => k.Key, v => v.Value);

            Reflection.Set(sagaInstance, "CorrelationId", correlationId);
            Reflection.Set(sagaInstance, "SagaData", sagaData);
            Reflection.Set(sagaInstance, "Headers", headers);

            return sagaInstance;
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class
        {
            var sagaData = Reflection.Get(saga, "SagaData");
            var sagaHeaders = (Dictionary<String, String>) Reflection.Get(saga, "Headers");
            var correlationId = (Guid) Reflection.Get(saga, "CorrelationId");

            var serialisedData = messageSerialiser.Serialise(sagaData);

            var dataModel = new SagaData()
            {
                CorrelationId = correlationId,
                BlobData = serialisedData,
            };


            using (var transaction = database.GetTransaction())
            {
                try
                {
                    int updatedRaws = database.Update(dataModel);

                    if (updatedRaws == 0)
                    {
                        // no records were updated - this means no records already exist - need to insert new record
                        database.Insert(dataModel);
                    }

                    // delete all existing headers
                    database.Delete<SagaHeaders>("WHERE CorrelationId=@0", correlationId);

                    // and insert updated ones
                    foreach (var header in sagaHeaders)
                    {
                        var storedHeader = new SagaHeaders()
                        {
                            CorrelationId = correlationId,
                            Key = header.Key,
                            Value = header.Value,
                        };

                        database.Insert(storedHeader);
                    }
                    transaction.Complete();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }


        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");
            Complete(correlationId);
        }

        public void Complete(Guid correlationId)
        {
            using (var transaction = database.GetTransaction())
            {
                try
                {
                    database.Delete<SagaHeaders>("WHERE CorrelationId=@0", correlationId);
                    database.Delete<SagaData>("WHERE CorrelationId=@0", correlationId);
                    transaction.Complete();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }
    }



    [TableName(SqlSagaRepository.SagaDataTableName)]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaData
    {
        public Guid CorrelationId { get; set; }
        public String BlobData { get; set; }
    }


    [TableName(SqlSagaRepository.HeadersTableName)]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaHeaders
    {
        public Guid CorrelationId { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
    }
}
