using System;
using NSaga;

namespace Tests.Stubs
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

        public Guid CorrelationId { get;  }
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

        public Guid CorrelationId { get; }
    }
}