using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace NSaga
{
    /// <summary>
    /// Basic implementation of in-memory storage for ISagaRepository.
    /// <para><b>This implementation is not recommended for any serious production use, as the data will be erased on app restart</b></para>
    /// </summary>
    /// <seealso cref="NSaga.ISagaRepository" />
    public sealed class InMemorySagaRepository : ISagaRepository
    {
        private readonly IMessageSerialiser messageSerialiser;
        private readonly ISagaFactory sagaFactory;
        internal static readonly ConcurrentDictionary<Guid, String> DataDictionary = new ConcurrentDictionary<Guid, string>();
        internal static readonly ConcurrentDictionary<Guid, String> HeadersDictionary = new ConcurrentDictionary<Guid, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemorySagaRepository"/> class.
        /// </summary>
        /// <param name="messageSerialiser">The message serialiser.</param>
        /// <param name="sagaFactory">The saga factory.</param>
        public InMemorySagaRepository(IMessageSerialiser messageSerialiser, ISagaFactory sagaFactory)
        {
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


        /// <summary>
        /// Persists the instance of saga into the database storage.
        /// Actually stores SagaData and Headers. All other variables in saga are not persisted
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga instance</param>
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


        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga to be deleted</param>
        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            var correlationId = (Guid)NSagaReflection.Get(saga, "CorrelationId");

            Complete(correlationId);
        }


        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <param name="correlationId">Correlation Id for the saga</param>
        public void Complete(Guid correlationId)
        {
            String value;
            DataDictionary.TryRemove(correlationId, out value);
            HeadersDictionary.TryRemove(correlationId, out value);
        }

        /// <summary>
        /// Resets the memory storage - clears the dictionaries with data.
        /// </summary>
        public static void ResetStorage()
        {
            DataDictionary.Clear();
            HeadersDictionary.Clear();
        }
    }
}
