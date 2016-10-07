using System;
using NSaga;

namespace Samples
{
    public class VerificationCodeProvided : ISagaMessage
    {
        public VerificationCodeProvided(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }

        public String VerificationCode { get; set; }
    }
}