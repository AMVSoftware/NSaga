using System;
using NSaga;

namespace Tests
{
    /// <summary>
    /// Saga Message that initiates multiple sagas of different type
    /// </summary>
    public class MultipleSagaInitiator : IInitiatingSagaMessage
    {
        public MultipleSagaInitiator(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; set; }
    }
}
