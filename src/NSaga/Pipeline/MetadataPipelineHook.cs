using System;
using NSaga.Implementations;

namespace NSaga.Pipeline
{
    public class MetadataPipelineHook : BasePipelineHook, IPipelineHook
    {
        private readonly IMessageSerialiser serialiser;
        public const string MetadataKeyName = "SagaMetadataKey";

        public MetadataPipelineHook(IMessageSerialiser serialiser)
        {
            this.serialiser = serialiser;
        }

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
