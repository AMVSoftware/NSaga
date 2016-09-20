using System;
using System.Collections.Generic;

namespace NSaga
{
    public interface IAccessibleSaga
    {
        Guid CorrelationId { get; set; }


        /// <summary>
        /// Metadata information
        /// </summary>
        Dictionary<String, String> Headers { get; set; }
    }

    public interface ISaga<TSagaData> : IAccessibleSaga
    {
        TSagaData SagaData { get; set; }
    }



    public interface ISagaMessage
    {
        Guid CorrelationId { get; }
    }


    public interface IInitiatingSagaMessage : ISagaMessage
    {
        // marker interface
    }

    public interface InitiatedBy<TMsg> where TMsg : IInitiatingSagaMessage
    {
        OperationResult Initiate(TMsg message);
    }

    public interface ConsumerOf<TMsg> where TMsg : ISagaMessage
    {
        OperationResult Consume(TMsg message);
    }
}
