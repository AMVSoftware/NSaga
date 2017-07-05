using System;
using System.Collections.Generic;

namespace NSaga
{
    /// <summary>
    /// Interface that does not contain generic TSagaData part.
    /// This is for ease of accessing of saga's headers and CorrelationId
    /// <para><b>If you are creating new Sagas, please use <see cref="ISaga{TSagaData}"/> interface.</b></para>
    /// </summary>
    public interface IAccessibleSaga
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        Guid CorrelationId { get; set; }


        /// <summary>
        /// Metadata information
        /// </summary>
        Dictionary<String, String> Headers { get; set; }
    }


    /// <summary>
    /// General Saga interface that describes what every saga should have.
    /// </summary>
    /// <typeparam name="TSagaData">Type of data that will be preserved</typeparam>
    public interface ISaga<TSagaData> : IAccessibleSaga
    {
        /// <summary>
        /// Gets or sets the saga data.
        /// </summary>
        /// <value>
        /// The saga data - payload
        /// </value>
        TSagaData SagaData { get; set; }
    }
    

    /// <summary>
    /// Defines message that is consumed by a Saga
    /// </summary>
    public interface ISagaMessage
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        Guid CorrelationId { get; set; }
    }


    /// <summary>
    /// Defines a message that initiates a Saga
    /// </summary>
    public interface IInitiatingSagaMessage : ISagaMessage
    {
        // marker interface
    }


    /// <summary>
    /// Interface for a Saga that defines what type of message initiates the Saga
    /// </summary>
    /// <typeparam name="TMsg">What type of message to initiate the saga</typeparam>
    public interface InitiatedBy<TMsg> where TMsg : IInitiatingSagaMessage
    {
        /// <summary>
        /// Initiates the saga by a specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Result of the operation</returns>
        OperationResult Initiate(TMsg message);
    }

    /// <summary>
    /// Interface for a Saga that defines that the Saga can consume particular type of message
    /// </summary>
    /// <typeparam name="TMsg">What type of message to be consumed</typeparam>
    public interface ConsumerOf<TMsg> where TMsg : ISagaMessage
    {
        /// <summary>
        /// Consumes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Result of the operation</returns>
        OperationResult Consume(TMsg message);
    }
}
