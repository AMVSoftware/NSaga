using System;
using System.Collections.Generic;
using System.Linq;
using PetaPoco;

namespace NSaga.SqlServer
{
    public class SqlSagaRepository : ISagaRepository
    {
        private readonly IServiceLocator serviceLocator;
        private readonly String connectionString;

        private readonly Database database;
        private readonly IMessageSerialiser messageSerialiser;

        public SqlSagaRepository(IServiceLocator serviceLocator, string connectionString, IMessageSerialiser messageSerialiser)
        {
            this.connectionString = connectionString;
            this.messageSerialiser = messageSerialiser;
            this.serviceLocator = serviceLocator;
            this.database = new Database(connectionString);
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
            throw new NotImplementedException();
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }



    [TableName("NSaga.Sagas")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaData
    {
        public Guid CorrelationId { get; set; }
        public String BlobData { get; set; }
    }


    [TableName("NSaga.Headers")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaHeaders
    {
        public Guid CorrelationId { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
    }
}
