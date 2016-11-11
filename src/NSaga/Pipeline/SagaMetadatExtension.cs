using System;

namespace NSaga
{
    public static class SagaMetadatExtension
    {
        public static SagaMetadata GetSagaMetadata(this IAccessibleSaga accessibleSaga, IMessageSerialiser messageSerialiser)
        {
            String serialisedMetadata;
            if (!accessibleSaga.Headers.TryGetValue(MetadataPipelineHook.MetadataKeyName, out serialisedMetadata))
            {
                return null;
            }

            var metadata = messageSerialiser.Deserialise<SagaMetadata>(serialisedMetadata);
            return metadata;
        }
    }
}
