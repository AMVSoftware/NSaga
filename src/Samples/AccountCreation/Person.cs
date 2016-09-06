using System;

namespace Samples
{
    public class Person
    {
        public Guid PersonId { get; set; }
        public String ReferenceNumber { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Postcode { get; set; }

        public String FullName => $"{ReferenceNumber} - {FirstName} {LastName}";

        public String WorkEmail { get; set; }
        public String WorkMobile { get; set; }
        public String PrivateEmail { get; set; }
        public String PrivateMobile { get; set; }
        public String DomainLogin { get; set; }
    }
}