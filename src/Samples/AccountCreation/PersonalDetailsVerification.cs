using System;
using NSaga;

namespace Samples
{
    public class PersonalDetailsVerification : IInitiatingSagaMessage
    {
        public PersonalDetailsVerification(Guid correlationId)
        {
            this.CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String PayrollNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public String HomePostcode { get; set; }
    }
}