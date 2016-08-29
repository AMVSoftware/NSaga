using System;
using NSaga;

namespace Tests
{
    public class MySagaInitiatingMessage : IInitiatingSagaMessage
    {
        public MySagaInitiatingMessage()
        {
        }

        public MySagaInitiatingMessage(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }


    public class MySagaData
    {
        public String Name { get; set; }
    }

    public class MySaga : ISaga<MySagaData>, InitiatedBy<MySagaInitiatingMessage>
    {
        public MySagaData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public bool IsCompleted { get; set; }

        public OperationResult Initiate(MySagaInitiatingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
