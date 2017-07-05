using System;

namespace NSaga
{
    /// <summary>
    /// Metadata pipeline hook is inserted into the pipeline by default configurators. 
    /// This hook provides a useful information about all the incoming messages. 
    /// This information is stored in Saga.Headers and can be later queried by <see cref="SagaMetadatExtension"/>
    /// </summary>
    /// <seealso cref="NSaga.IPipelineHook" />
    public sealed class MetadataPipelineHook : BasePipelineHook, IPipelineHook
    {
        private readonly IMessageSerialiser serialiser;
        internal const string MetadataKeyName = "SagaMetadataKey";

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataPipelineHook"/> class.
        /// </summary>
        /// <param name="serialiser">The serialiser.</param>
        public MetadataPipelineHook(IMessageSerialiser serialiser)
        {
            this.serialiser = serialiser;
        }


        /// <summary>
        /// After the saga is initialised but before it is saved to the storage: save the message currently in the pipeline into Saga.Headers
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public override void AfterInitialisation(PipelineContext context)
        {
            var sagaMetadata = new SagaMetadata();
            String serialisedMetadata;
            if (context.AccessibleSaga.Headers.TryGetValue(MetadataKeyName, out serialisedMetadata))
            {
                sagaMetadata = serialiser.Deserialise<SagaMetadata>(serialisedMetadata);
            }

            sagaMetadata.DateCreated = TimeProvider.Current.UtcNow;
            sagaMetadata.DateLastModified = TimeProvider.Current.UtcNow;
            sagaMetadata.ReceivedMessages.Add(new ReceivedMessage(context.Message, context.OperationResult));

            context.AccessibleSaga.Headers[MetadataKeyName] = serialiser.Serialise(sagaMetadata);
        }


        /// <summary>
        /// Hook executed after the message is consimed by the saga, but before saga is saved to the storage: save the message currently in the pipeline into Saga.Headers
        /// </summary>
        /// <param name="context">Context containing objects and data related to the operation</param>
        public override void AfterConsuming(PipelineContext context)
        {
            var sagaMetadata = new SagaMetadata();
            String serialisedMetadata;
            if (context.AccessibleSaga.Headers.TryGetValue(MetadataKeyName, out serialisedMetadata))
            {
                sagaMetadata = serialiser.Deserialise<SagaMetadata>(serialisedMetadata);
            }

            sagaMetadata.DateLastModified = TimeProvider.Current.UtcNow;
            sagaMetadata.ReceivedMessages.Add(new ReceivedMessage(context.Message, context.OperationResult));

            context.AccessibleSaga.Headers[MetadataKeyName] = serialiser.Serialise(sagaMetadata);
        }
    }
}
