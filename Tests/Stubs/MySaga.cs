using System;
using NSaga;

namespace Tests.Stubs
{
    public class MySagaData
    {
        public bool IsInitialised { get; set; }
        public bool IsAdditionalInitialiserCalled { get; set; }
        public bool IsConsumingMessageReceived { get; set; }
        public Guid SomeGuid { get; set; }
    }


    public class MySaga : ISaga<MySagaData>, 
                          InitiatedBy<MySagaInitiatingMessage>,
                          InitiatedBy<MultipleSagaInitiator>,
                          InitiatedBy<MySagaAdditionalInitialser>,
                          ConsumerOf<MySagaConsumingMessage>
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

        public OperationResult Initiate(MySagaAdditionalInitialser message)
        {
            SagaData.IsAdditionalInitialiserCalled = true;

            return new OperationResult();
        }

        public OperationResult Consume(MySagaConsumingMessage message)
        {
            SagaData.IsConsumingMessageReceived = true;

            return new OperationResult();
        }
    }


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



    public class MySagaAdditionalInitialser : IInitiatingSagaMessage
    {
        public MySagaAdditionalInitialser(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }


    public class MySagaConsumingMessage : ISagaMessage
    {
        public MySagaConsumingMessage(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public MySagaConsumingMessage()
        {
            // not assigning Guid to the ID - that's deliberate
        }

        public Guid CorrelationId { get; set; }
    }
}
