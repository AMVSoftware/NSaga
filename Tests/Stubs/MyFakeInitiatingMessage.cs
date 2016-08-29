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
}