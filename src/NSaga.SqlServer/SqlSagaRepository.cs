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

        private readonly ISagaFactory sagaFactory;
        private readonly IMessageSerialiser messageSerialiser;
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        /// Initiates an instance of <see cref="SqlSagaRepository"/> with a connection string name.
        /// Actual connection string is taken from your app.config or web.config
        /// </summary>
        /// <param name="connectionFactory">An insantance implementing <see cref="IConnectionFactory"/></param>
        /// <param name="sagaFactory">An instance implementing <see cref="ISagaFactory"/></param>
        /// <param name="messageSerialiser">An instance implementing <see cref="IMessageSerialiser"/></param>
        public SqlSagaRepository(IConnectionFactory connectionFactory, ISagaFactory sagaFactory, IMessageSerialiser messageSerialiser)
        {
            Guard.ArgumentIsNotNull(connectionFactory, nameof(connectionFactory));
            Guard.ArgumentIsNotNull(sagaFactory, nameof(sagaFactory));
            Guard.ArgumentIsNotNull(messageSerialiser, nameof(messageSerialiser));

            this.messageSerialiser = messageSerialiser;
            this.sagaFactory = sagaFactory;
            this.connectionFactory = connectionFactory;
        }


        /// <summary>
        /// Finds and returns saga instance with the given correlation ID.
        /// You will get exceptions if TSaga does not match the actual saga data with the provided exception.
        /// 
        /// Actually creates an instance of saga from service locator, retrieves SagaData and Headers from the storage and populates the instance with these.
        /// </summary>
        /// <typeparam name="TSaga">Type of saga we are looking for</typeparam>
        /// <param name="correlationId">CorrelationId to identify the saga</param>
        /// <returns>An instance of the saga. Or Null if there is no saga with this ID.</returns>
        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            Guard.ArgumentIsNotNull(correlationId, nameof(correlationId));

            using(var connection = connectionFactory.GetConnection())
            using (var database = new Database(connection))
            {
                var sql = Sql.Builder.Where("correlationId = @0", correlationId);
                var persistedData = database.SingleOrDefault<SagaData>(sql);

                if (persistedData == null)
                {
                    return null;
                }

                var sagaInstance = sagaFactory.ResolveSaga<TSaga>();
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
        }


        /// <summary>
        /// Persists the instance of saga into the database storage.
        /// 
        /// Actually stores SagaData and Headers. All other variables in saga are not persisted
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga instance</param>
        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            Guard.ArgumentIsNotNull(saga, nameof(saga));

            var sagaData = Reflection.Get(saga, "SagaData");
            var sagaHeaders = (Dictionary<String, String>)Reflection.Get(saga, "Headers");
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

            var serialisedData = messageSerialiser.Serialise(sagaData);

            var dataModel = new SagaData()
            {
                CorrelationId = correlationId,
                BlobData = serialisedData,
            };

            using (var connection = connectionFactory.GetConnection())
            using (var database = new Database(connection))
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


        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga to be deleted</param>
        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            Guard.ArgumentIsNotNull(saga, nameof(saga));

            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");
            Complete(correlationId);
        }

        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <param name="correlationId">Correlation Id for the saga</param>
        public void Complete(Guid correlationId)
        {
            Guard.ArgumentIsNotNull(correlationId, nameof(correlationId));

            using (var connection = connectionFactory.GetConnection())
            using (var database = new Database(connection))
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
