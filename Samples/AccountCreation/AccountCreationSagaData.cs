using System;

namespace Samples
{
    public class AccountCreationSagaData
    {
        public bool IsPersonalDetailsVerified => Person != null;
        public Person Person { get; set; }

        public String VerificationCode { get; set; }
        public DateTime? VerificationCodeSentDate { get; set; }

        public bool IsVerificationCodeSent => !String.IsNullOrWhiteSpace(VerificationCode);
        public bool IsVerificationCodeMatched { get; set; }
    }
}