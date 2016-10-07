using System;
using NSaga;

namespace Samples
{
    public class VerificationCodeRequest : ISagaMessage
    {
        public Guid CorrelationId { get; set; }

        public VerificationCodeRequest(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public String RequestCarrier { get; set; }
        public String CallbackUrl { get; set; }
    }
}