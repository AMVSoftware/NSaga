//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

//namespace NSaga
//{
//    public class SagaRepository : ISagaRepository
//    {
//        private readonly SelfServiceDatabase database;
//        private readonly Container container;

//        public SagaRepository(SelfServiceDatabase database, Container container)
//        {
//            this.database = database;
//            this.container = container;
//        }

//        public TSaga StartSaga<TSaga>() where TSaga : class
//        {
//            var correlationId = Guid.NewGuid();

//            var sagaInstance = container.GetInstance<TSaga>();

//            var sagaDataType = Reflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));
//            var sagaData = Activator.CreateInstance(sagaDataType);
//            Reflection.Set(sagaInstance, "SagaData", sagaData);
//            Reflection.Set(sagaInstance, "CorrelationId", correlationId);

//            var persistedData = new SagaPersistedData()
//            {
//                CorrelationId = correlationId,
//                BlobData = JsonConvert.SerializeObject(sagaData),
//                DateCreated = TimeProvider.Current.UtcNow,
//                DateLastModified = TimeProvider.Current.UtcNow,
//            };

//            database.Insert(persistedData);

//            return sagaInstance;
//        }

//        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
//        {
//            // get saga data deserialised from DB
//            var sql = Sql.Builder.Where("correlationId = @0", correlationId);
//            var persistedData = database.SingleOrDefault<SagaPersistedData>(sql);

//            if (persistedData == null)
//            {
//                return null;
//            }

//            var sagaInstance = container.GetInstance<TSaga>();

//            var sagaDataType = Reflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));
//            var deserialisedSagaData = JsonConvert.DeserializeObject(persistedData.BlobData, sagaDataType);

//            // need reflection here. Otherwise will need to specify <TSaga, TSagaData> all over the shop
//            Reflection.Set(sagaInstance, "CorrelationId", correlationId);
//            Reflection.Set(sagaInstance, "SagaData", deserialisedSagaData);

//            return sagaInstance;
//        }


//        /// <summary>
//        /// Save Saga to database
//        /// </summary>
//        /// <param name="saga">Saga to be saved into DB</param>
//        public void Save<TSaga>(TSaga saga) where TSaga : class
//        {
//            var sagaData = Reflection.Get(saga, "SagaData");
//            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

//            var serialisedData = JsonConvert.SerializeObject(sagaData);

//            var persistedData = new SagaPersistedDataUpdateModel()
//            {
//                CorrelationId = correlationId,
//                BlobData = serialisedData,
//                DateLastModified = TimeProvider.Current.UtcNow,
//            };

//            database.Update(persistedData);
//        }


//        /// <summary>
//        ///  Remove saga data from DB
//        /// </summary>
//        public void Complete<TSaga>(TSaga saga) where TSaga : class
//        {
//            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

//            var data = new SagaPersistedData() { CorrelationId = correlationId };
//            database.Delete(data);
//        }


//        [TableName("Sagas")]
//        [PrimaryKey("CorrelationId", AutoIncrement = false)]
//        private class SagaPersistedDataUpdateModel
//        {
//            public Guid CorrelationId { get; set; }
//            public String BlobData { get; set; }
//            public DateTime? DateLastModified { get; set; }
//        }



//        [TableName("Sagas")]
//        [PrimaryKey("CorrelationId", AutoIncrement = false)]
//        public class SagaPersistedData
//        {
//            public Guid CorrelationId { get; set; }
//            public String BlobData { get; set; }
//            public DateTime? DateCreated { get; set; }
//            public DateTime? DateLastModified { get; set; }
//        }
//    }
//}
