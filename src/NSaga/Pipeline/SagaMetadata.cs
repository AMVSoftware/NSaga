using System;
using System.Collections.Generic;

namespace NSaga
{
    public class SagaMetadata
    {
        public SagaMetadata()
        {
            ReceivedMessages = new List<ReceivedMessage>();
        }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastModified { get; set; }
        public List<ReceivedMessage> ReceivedMessages { get; set; }
    }


    public class ReceivedMessage
    {
        public ReceivedMessage(ISagaMessage sagaMessage)
        {
            Timestamp = DateTime.UtcNow;
            SagaMessage = sagaMessage;
        }

        public ReceivedMessage(ISagaMessage sagaMessage, OperationResult operationResult)
        {
            Timestamp = DateTime.UtcNow;
            SagaMessage = sagaMessage;
            OperationResult = operationResult;
        }

        public DateTime Timestamp { get; set; }
        public ISagaMessage SagaMessage { get; set; }
        public OperationResult OperationResult { get; set; }
    }
}
