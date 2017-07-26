using NSaga;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Tests.Stubs
{
    public class MyAsyncSagaData
    {
        public bool IsInitialised { get; set; }
        public bool IsAdditionalInitialiserCalled { get; set; }
        public bool IsConsumingMessageReceived { get; set; }
        public Guid SomeGuid { get; set; }
    }

    public class MyAsyncSaga : ISaga<MyAsyncSagaData>,
                               InitiatedBy<MyAsyncSagaInitialser>,
                               AsyncConsumerOf<MyAsyncSagaInitialser>
    {
        public MyAsyncSagaData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public MyAsyncSaga()
        {
            Headers = new Dictionary<string, string>();
            SagaData = new MyAsyncSagaData();
        }

        public OperationResult Initiate(MyAsyncSagaInitialser message)
        {
            SagaData.IsInitialised = true;

            return new OperationResult();
        }

        public Task<OperationResult> ConsumeAsync(MyAsyncSagaInitialser message)
        {
            SagaData.IsConsumingMessageReceived = true;

            return Task.FromResult(new OperationResult());
        }
    }


    public class MyAsyncSagaInitialser : IInitiatingSagaMessage
    {
        public MyAsyncSagaInitialser(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }

    public class MyAsyncMessage : ISagaMessage
    {
        public MyAsyncMessage(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}
