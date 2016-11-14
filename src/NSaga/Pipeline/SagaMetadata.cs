using System;
using System.Collections.Generic;

namespace NSaga
{
    public sealed class SagaMetadata
    {
        public SagaMetadata()
        {
            ReceivedMessages = new List<ReceivedMessage>();
        }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastModified { get; set; }
        public List<ReceivedMessage> ReceivedMessages { get; set; }
    }


    public sealed class ReceivedMessage
    {
        internal ReceivedMessage()
        {
            // constructor for serialisation
        }

        public ReceivedMessage(ISagaMessage sagaMessage)
        {
            Timestamp = TimeProvider.Current.UtcNow;
            SagaMessage = sagaMessage;
        }

        public ReceivedMessage(ISagaMessage sagaMessage, OperationResult operationResult)
        {
            Timestamp = TimeProvider.Current.UtcNow;
            SagaMessage = sagaMessage;
            OperationResult = operationResult;
        }

        public DateTime Timestamp { get; set; }
        public ISagaMessage SagaMessage { get; set; }
        public OperationResult OperationResult { get; set; }
    }
}
