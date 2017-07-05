using System;
using NSaga;

namespace Tests
{
    /// <summary>
    /// Message that does not inititate anything
    /// </summary>
    public class MyFakeInitiatingMessage : IInitiatingSagaMessage
    {
        public MyFakeInitiatingMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }


    /// <summary>
    /// Message that is not consumed by anything
    /// </summary>
    class MyFakeSagaMessage : ISagaMessage
    {
        public MyFakeSagaMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}