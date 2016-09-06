using System;
using System.Collections.Generic;


namespace NSaga
{
    public class InMemorySagaRepository : ISagaRepository
    {
        private readonly IMessageSerialiser messageSerialiser;
        private readonly IServiceLocator serviceLocator;
        public Dictionary<Guid, String> DataDictionary { get; }
        public Dictionary<Guid, String> HeadersDictionary { get; }

        public InMemorySagaRepository(IMessageSerialiser messageSerialiser, IServiceLocator serviceLocator)
        {
            this.messageSerialiser = messageSerialiser;
            this.serviceLocator = serviceLocator;
            DataDictionary = new Dictionary<Guid, string>();
            HeadersDictionary = new Dictionary<Guid, string>();
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
        {
            string dataSerialised;

            if (!DataDictionary.TryGetValue(correlationId, out dataSerialised))
            {
                return null;
            }

            string headersSerialised;
            var headers = new Dictionary<String, String>();
            if (HeadersDictionary.TryGetValue(correlationId, out headersSerialised))
            {
                headers = messageSerialiser.Deserialise<Dictionary<String, String>>(headersSerialised);
            }

            var sagaDataType = Reflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));

            var dataObject = messageSerialiser.Deserialise(dataSerialised, sagaDataType);

            var saga = serviceLocator.Resolve<TSaga>();
            Reflection.Set(saga, "SagaData", dataObject);
            Reflection.Set(saga, "CorrelationId", correlationId);
            Reflection.Set(saga, "Headers", headers);

            return saga;
        }


        public void Save<TSaga>(TSaga saga) where TSaga : class
        {
            var sagaData = Reflection.Get(saga, "SagaData");
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");
            var headers = (Dictionary<String, String>) Reflection.Get(saga, "Headers");

            var serialisedData = messageSerialiser.Serialise(sagaData);
            var serialisedHeaders = messageSerialiser.Serialise(headers);

            DataDictionary[correlationId] = serialisedData;
            HeadersDictionary[correlationId] = serialisedHeaders;
        }


        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            var correlationId = (Guid)Reflection.Get(saga, "CorrelationId");

            Complete(correlationId);
        }


        public void Complete(Guid correlationId)
        {
            DataDictionary.Remove(correlationId);
            HeadersDictionary.Remove(correlationId);
        }
    }
}
