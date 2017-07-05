namespace NSaga
{
    /// <summary>
    /// The core of the system that routes the messages between sagas, retrieves saga data from storage, resolves the saga instances and does all the magic
    /// </summary>
    public interface ISagaMediator
    {
        /// <summary>
        /// Consumes the specified initiating message and creates a new instance of the correlating saga.
        /// <para>Saga is not persisted if operation have failed</para>
        /// </summary>
        /// <param name="initiatingMessage">The initiating message.</param>
        /// <returns>Result of the operation</returns>
        OperationResult Consume(IInitiatingSagaMessage initiatingMessage);


        /// <summary>
        /// Consumes the specified saga message - finds the existing saga that can consume given message type and with matching CorrelationId.
        /// </summary>
        /// <param name="sagaMessage">The saga message.</param>
        /// <returns>Result of the operation</returns>
        OperationResult Consume(ISagaMessage sagaMessage);
    }
}