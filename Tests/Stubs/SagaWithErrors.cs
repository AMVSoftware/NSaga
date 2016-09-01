using System;
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
                           ConsumerOf<GetSomeConsumedErrorsForSagaWithErrors>
    {
        public SagaWithErrors()
        {
        }

        public SagaWithErrorsData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public bool IsCompleted { get; set; }


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
    }


    public class InitiatingSagaWithErrors : IInitiatingSagaMessage
    {
        public InitiatingSagaWithErrors(Guid correlationId)
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
