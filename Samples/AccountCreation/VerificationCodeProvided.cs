using System;
using NSaga;

namespace Samples
{
    public class VerificationCodeProvided : ISagaMessage
    {
        public Guid CorrelationId { get; set; }

        public String VerificationCode { get; set; }
    }
}