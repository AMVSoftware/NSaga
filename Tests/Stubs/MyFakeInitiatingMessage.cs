using System;
using NSaga;

namespace Tests.Stubs
{
    /// <summary>
    /// Message that does not inititate anything
    /// </summary>
    public class MyFakeInitiatingMessage : IInitiatingSagaMessage
    {
        public Guid CorrelationId { get; set; }
    }


    /// <summary>
    /// Message that is not consumed by anything
    /// </summary>
    class MyFakeSagaMessage : ISagaMessage
    {
        public Guid CorrelationId { get; set; }
    }
}