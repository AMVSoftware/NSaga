using System;
using NSaga;

namespace Samples.BasicSaga
{
    public class BasicSagaStart : IInitiatingSagaMessage
    {
        public BasicSagaStart(Guid correlationId, String name)
        {
            CorrelationId = correlationId;
            Name = name;
        }

        public Guid CorrelationId { get; set; }
        public String Name { get; set; }
    }
}