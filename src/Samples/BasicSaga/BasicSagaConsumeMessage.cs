using System;
using NSaga;

namespace Samples.BasicSaga
{
    public class BasicSagaConsumeMessage : ISagaMessage
    {
        public BasicSagaConsumeMessage(Guid correlationid, String value)
        {
            CorrelationId = correlationid;
            Value = value;
        }

        public Guid CorrelationId { get; set; }
        public String Value { get; set; }
    }
}