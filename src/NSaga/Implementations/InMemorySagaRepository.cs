using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace NSaga
{
    public class InMemorySagaRepository : ISagaRepository
    {
        private readonly IMessageSerialiser messageSerialiser;
        private readonly ISagaFactory sagaFactory;
        private static readonly ConcurrentDictionary<Guid, String> DataDictionary = new ConcurrentDictionary<Guid, string>();
        private static readonly ConcurrentDictionary<Guid, String> HeadersDictionary = new ConcurrentDictionary<Guid, string>();

        public InMemorySagaRepository(IMessageSerialiser messageSerialiser, ISagaFactory sagaFactory)
        {
            this.messageSerialiser = messageSerialiser;
            this.sagaFactory = sagaFactory;
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
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

            var sagaDataType = NSagaReflection.GetInterfaceGenericType<TSaga>(typeof(ISaga<>));

            var dataObject = messageSerialiser.Deserialise(dataSerialised, sagaDataType);

            var saga = sagaFactory.ResolveSaga<TSaga>();
            NSagaReflection.Set(saga, "SagaData", dataObject);
            NSagaReflection.Set(saga, "CorrelationId", correlationId);
            NSagaReflection.Set(saga, "Headers", headers);

            return saga;
        }


        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var sagaData = NSagaReflection.Get(saga, "SagaData");
            var correlationId = (Guid)NSagaReflection.Get(saga, "CorrelationId");
            var headers = (Dictionary<String, String>) NSagaReflection.Get(saga, "Headers");

            var serialisedData = messageSerialiser.Serialise(sagaData);
            var serialisedHeaders = messageSerialiser.Serialise(headers);

            DataDictionary[correlationId] = serialisedData;
            HeadersDictionary[correlationId] = serialisedHeaders;
        }


        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var correlationId = (Guid)NSagaReflection.Get(saga, "CorrelationId");

            Complete(correlationId);
        }


        public void Complete(Guid correlationId)
        {
            String value;
            DataDictionary.TryRemove(correlationId, out value);
            HeadersDictionary.TryRemove(correlationId, out value);
        }

        public static void ResetStorage()
        {
            DataDictionary.Clear();
            HeadersDictionary.Clear();
        }
    }
}
