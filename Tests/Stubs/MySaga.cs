using System;
using NSaga;

namespace Tests.Stubs
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
        public bool IsInitialised { get; set; }
    }


    public class MySaga : ISaga<MySagaData>, 
                          InitiatedBy<MySagaInitiatingMessage>,
                          InitiatedBy<MultipleSagaInitiator>
    {
        public MySagaData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public bool IsCompleted { get; set; }

        public MySaga()
        {
            SagaData = new MySagaData();
        }


        public OperationResult Initiate(MySagaInitiatingMessage message)
        {
            SagaData.IsInitialised = true;

            return new OperationResult();
        }


        public OperationResult Initiate(MultipleSagaInitiator message)
        {
            SagaData.IsInitialised = true;

            return new OperationResult();
        }
    }
}
