using System;

namespace NSaga
{
    /// <summary>
    /// Extension method to allow retrieving metadata from saga headers. Metadata will be saved by <see cref="MetadataPipelineHook"/>
    /// </summary>
    public static class SagaMetadatExtension
    {
        /// <summary>
        /// Gets the saga metadata from previous operations
        /// </summary>
        /// <param name="accessibleSaga">The accessible saga.</param>
        /// <param name="messageSerialiser">The message serialiser.</param>
        /// <returns></returns>
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
