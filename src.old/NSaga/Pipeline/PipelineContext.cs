using System;

namespace NSaga
{
    /// <summary>
    /// Context of currently executing saga and a message. Contains references to useful objects
    /// </summary>
    public sealed class PipelineContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineContext"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="saga">The saga.</param>
        /// <param name="operationResult">The operation result. Null of operation have not finished</param>
        public PipelineContext(ISagaMessage message, IAccessibleSaga saga, OperationResult operationResult = null)
        {
            this.Message = message;
            this.AccessibleSaga = saga;
            SagaData = NSagaReflection.Get(saga, "SagaData");
            OperationResult = operationResult;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message that is currently in the pipeline. Can be <see cref="IInitiatingSagaMessage"/> as well.
        /// </value>
        public ISagaMessage Message { get; }

        /// <summary>
        /// Gets the operation result.
        /// </summary>
        /// <value>
        /// The operation result. Will be null if context is called before the operation (Initialise or Consume) is executed
        /// </value>
        public OperationResult OperationResult { get; }

        /// <summary>
        /// Gets the accessible saga.
        /// </summary>
        /// <value>
        /// The accessible saga that is currently getting worked on.
        /// </value>
        public IAccessibleSaga AccessibleSaga { get; }

        /// <summary>
        /// Gets the saga data.
        /// </summary>
        /// <value>
        /// The object containing saga data - supplement to <see cref="AccessibleSaga"/>
        /// </value>
        public Object SagaData { get; }
    }
}