using System;
using System.Collections.Generic;

namespace NSaga
{
    /// <summary>
    /// Metadata information saved by <see cref="MetadataPipelineHook"/>
    /// </summary>
    public sealed class SagaMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SagaMetadata"/> class.
        /// </summary>
        public SagaMetadata()
        {
            ReceivedMessages = new List<ReceivedMessage>();
        }
        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>
        /// The date when Saga was created. I.e. when <see cref="IInitiatingSagaMessage"/> was received.
        /// </value>
        public DateTime DateCreated { get; set; }


        /// <summary>
        /// Gets or sets the date last modified.
        /// </summary>
        /// <value>
        /// The date when Saga was last modified. i.e. last <see cref="ISagaMessage"/> was received.
        /// </value>
        public DateTime DateLastModified { get; set; }

        /// <summary>
        /// Gets or sets the received messages.
        /// </summary>
        /// <value>
        /// The list of all messages received by this saga
        /// </value>
        public List<ReceivedMessage> ReceivedMessages { get; set; }
    }


    /// <summary>
    /// Description of a message that was received by saga. This is constructed and used by <see cref="MetadataPipelineHook"/>
    /// </summary>
    public sealed class ReceivedMessage
    {
        internal ReceivedMessage()
        {
            // constructor for serialisation
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessage"/> class.
        /// </summary>
        /// <param name="sagaMessage">The saga message.</param>
        public ReceivedMessage(ISagaMessage sagaMessage)
        {
            Timestamp = TimeProvider.Current.UtcNow;
            SagaMessage = sagaMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessage"/> class.
        /// </summary>
        /// <param name="sagaMessage">The saga message.</param>
        /// <param name="operationResult">The operation result.</param>
        public ReceivedMessage(ISagaMessage sagaMessage, OperationResult operationResult)
        {
            Timestamp = TimeProvider.Current.UtcNow;
            SagaMessage = sagaMessage;
            OperationResult = operationResult;
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The UTC timestamp when the message was received
        /// </value>
        public DateTime Timestamp { get; set; }


        /// <summary>
        /// Gets or sets the saga message.
        /// </summary>
        /// <value>
        /// The actual saga message that was received
        /// </value>
        public ISagaMessage SagaMessage { get; set; }

        /// <summary>
        /// Gets or sets the operation result.
        /// </summary>
        /// <value>
        /// The operation result. Will be null if this is saved before the operation was applied.
        /// </value>
        public OperationResult OperationResult { get; set; }
    }
}
