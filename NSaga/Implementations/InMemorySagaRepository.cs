using System;
using System.Collections.Generic;


namespace NSaga
{
    public class InMemorySagaRepository : ISagaRepository
    {
        private readonly IMessageSerialiser messageSerialiser;
        private readonly IServiceLocator serviceLocator;
        public Dictionary<Guid, String> DataDictionary { get; }

        public InMemorySagaRepository(IMessageSerialiser messageSerialiser, IServiceLocator serviceLocator)
        {
            this.messageSerialiser = messageSerialiser;
            this.serviceLocator = serviceLocator;
            DataDictionary = new Dictionary<Guid, string>();
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
        {
            string dataSerialised;

            if (!DataDictionary.TryGetValue(correlationId, out dataSerialised))
            {
                return null;
            }

            var sagaDataType = Reflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));

            var dataObject = messageSerialiser.Deserialise(dataSerialised, sagaDataType);

            var saga = serviceLocator.Resolve<TSaga>();
            Reflection.Set(saga, "SagaData", dataObject);
            Reflection.Set(saga, "CorrelationId", correlationId);

            return saga;
        }


        public void Save<TSaga>(TSaga saga) where TSaga : class
        {
            var sagaData = Reflection.Get(saga, "SagaData");
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

            var serialisedData = messageSerialiser.Serialise(sagaData);

            DataDictionary[correlationId] = serialisedData;
        }


        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

            DataDictionary.Remove(correlationId);
        }
    }
}
