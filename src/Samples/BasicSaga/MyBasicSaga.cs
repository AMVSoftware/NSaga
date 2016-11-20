using System;
using System.Collections.Generic;
using NSaga;

namespace Samples.BasicSaga
{
    public class BasicSagaData
    {
        public String Name { get; set; }
        public String Value { get; set; }
    }
    public class MyBasicSaga : ISaga<BasicSagaData>, InitiatedBy<BasicSagaStart>, ConsumerOf<BasicSagaConsumeMessage>
    {
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public BasicSagaData SagaData { get; set; }


        public OperationResult Initiate(BasicSagaStart message)
        {
            SagaData.Name = message.Name;
            Console.WriteLine($"Name is {SagaData.Name}");
            return new OperationResult();
        }

        public OperationResult Consume(BasicSagaConsumeMessage message)
        {
            SagaData.Value = message.Value;
            Console.WriteLine($"Value is {SagaData.Value}");
            return new OperationResult();
        }
    }
}
