using System;
using System.Collections.Generic;
using NSaga;

namespace Tests.Stubs
{
    public class SagaWithErrorsData
    {
        public bool IsInitiated { get; set; }
        public bool IsConsumed { get; set; }
    }

    class SagaWithErrors : ISaga<SagaWithErrorsData>,
                           InitiatedBy<InitiatingSagaWithErrors>,
                           InitiatedBy<MultipleSagaInitiator>,
                           InitiatedBy<ActuallyInitiatingSagaWithErrors>,
                           ConsumerOf<GetSomeConsumedErrorsForSagaWithErrors>
    {
        public SagaWithErrors()
        {
        }

        public SagaWithErrorsData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Headers { get; set; }


        public OperationResult Initiate(InitiatingSagaWithErrors message)
        {
            this.SagaData.IsInitiated = true;
            var errors = new OperationResult("This is not right!");

            return errors;
        }

        public OperationResult Initiate(MultipleSagaInitiator message)
        {
            this.SagaData.IsInitiated = true;
            var errors = new OperationResult("This is not right!");

            return errors;
        }

        public OperationResult Consume(GetSomeConsumedErrorsForSagaWithErrors message)
        {
            this.SagaData.IsConsumed = true;
            var errors = new OperationResult("This is not right!");

            return errors;
        }

        public OperationResult Initiate(ActuallyInitiatingSagaWithErrors message)
        {
            this.SagaData.IsInitiated = true;

            return new OperationResult();
        }
    }


    public class InitiatingSagaWithErrors : IInitiatingSagaMessage
    {
        public InitiatingSagaWithErrors(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }

    public class ActuallyInitiatingSagaWithErrors : IInitiatingSagaMessage
    {
        public ActuallyInitiatingSagaWithErrors(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }

    public class GetSomeConsumedErrorsForSagaWithErrors : ISagaMessage
    {
        public GetSomeConsumedErrorsForSagaWithErrors(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}
