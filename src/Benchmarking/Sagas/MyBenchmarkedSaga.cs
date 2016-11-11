using System;
using System.Collections.Generic;
using NSaga;

namespace Benchmarking.Sagas
{
    public class MyBenchmarkedSagaData
    {
        public List<Guid> MessageIds { get; set; }

        public MyBenchmarkedSagaData()
        {
            MessageIds = new List<Guid>();
        }
    }

    public class MyBenchmarkedSaga : ISaga<MyBenchmarkedSagaData>,
                                     InitiatedBy<FirstMessage>,
                                     ConsumerOf<SecondMessage>,
                                     ConsumerOf<ThirdMessage>
    {
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public MyBenchmarkedSagaData SagaData { get; set; }

        public MyBenchmarkedSaga()
        {
            // nothing here just for the sake of it
        }


        public OperationResult Initiate(FirstMessage message)
        {
            SagaData.MessageIds.Add(message.MessageId);

            return new OperationResult();
        }

        public OperationResult Consume(SecondMessage message)
        {
            SagaData.MessageIds.Add(message.MessageId);

            return new OperationResult();
        }

        public OperationResult Consume(ThirdMessage message)
        {
            SagaData.MessageIds.Add(message.MessageId);

            return new OperationResult();
        }
    }
}
