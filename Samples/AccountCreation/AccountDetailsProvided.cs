using NSaga;
using System;

namespace Samples
{
    public class AccountDetailsProvided : ISagaMessage
    {
        public Guid CorrelationId { get; set; }

        public String Username { get; set; }
        public String Password { get; set; }
        public String PasswordConfirmation { get; set; }
    }
}