using System;
using NSaga;

namespace Benchmarking.Sagas
{
    public class FirstMessage : IInitiatingSagaMessage
    {
        public Guid CorrelationId { get; set; }

        public Guid MessageId { get; set; }

        public FirstMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
            MessageId = Guid.NewGuid();
        }
    }

    public class SecondMessage : ISagaMessage
    {
        public Guid CorrelationId { get; set; }

        public Guid MessageId { get; set; }

        public SecondMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
            MessageId = Guid.NewGuid();
        }
    }

    public class ThirdMessage : ISagaMessage
    {
        public Guid CorrelationId { get; set; }

        public Guid MessageId { get; set; }

        public ThirdMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
            MessageId = Guid.NewGuid();
        }
    }
}
