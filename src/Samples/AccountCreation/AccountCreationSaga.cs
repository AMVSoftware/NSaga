using System;
using System.Collections.Generic;
using NSaga;

namespace Samples
{
    public class AccountCreationSaga : ISaga<AccountCreationSagaData>,
                                       InitiatedBy<PersonalDetailsVerification>,
                                       ConsumerOf<VerificationCodeRequest>,
                                       ConsumerOf<VerificationCodeProvided>,
                                       ConsumerOf<AccountDetailsProvided>
    {
        public AccountCreationSagaData SagaData { get; set; }
        public Guid CorrelationId { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public AccountCreationSaga(/*Some dependencies*/)
        {
            SagaData = new AccountCreationSagaData();
            Headers = new Dictionary<string, string>();
        }


        public OperationResult Initiate(PersonalDetailsVerification message)
        {
            if (message.FirstName != "James" || message.LastName != "Bond")
            {
                return new OperationResult("This Saga only works for James Bond");
            }

            var person = new Person()
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                ReferenceNumber = message.PayrollNumber,
                Postcode = message.HomePostcode,
                PrivateEmail = "bondy1920@hotmail.com",
                WorkEmail = "James.Bond@mi6.org.uk",
                WorkMobile = "07007007007",
                PrivateMobile = "07123456789",
            };
            SagaData.Person = person;
            Console.WriteLine($"Saga initiated. Person in question is {person.FullName}");
            return new OperationResult().AddPayload(person);
        }


        public OperationResult Consume(VerificationCodeRequest message)
        {
            if (!SagaData.IsPersonalDetailsVerified)
            {
                throw new Exception("Person details are not verified. How did you get here?");
            }

            SagaData.VerificationCode = "123456";
            SagaData.VerificationCodeSentDate = DateTime.UtcNow;

            if (message.RequestCarrier == "Mobile")
            {
                Console.WriteLine($"sending SMS: Please enter verification code {SagaData.VerificationCode} into the form");
            }
            else
            {
                Console.WriteLine($"sending Email: Please enter verification code {SagaData.VerificationCode} into the form");
            }

            return new OperationResult();
        }


        public OperationResult Consume(VerificationCodeProvided message)
        {
            if (!SagaData.IsVerificationCodeSent)
            {
                throw new Exception("Verification code is not sent. How did you get here?");
            }

            if (SagaData.VerificationCode != message.VerificationCode)
            {
                return new OperationResult("Verification Code is not set");
            }

            if (SagaData.VerificationCodeSentDate.Value.AddDays(1) < DateTime.UtcNow)
            {
                return new OperationResult("Verification code has expired. Please request a new one");
            }

            if (message.VerificationCode != SagaData.VerificationCode)
            {
                return new OperationResult("Verification code does not match. Please try again");
            }

            Console.WriteLine("Verificaton code provided matches!");
            SagaData.IsVerificationCodeMatched = true;
            return new OperationResult();
        }



        public OperationResult Consume(AccountDetailsProvided message)
        {
            if (!SagaData.IsVerificationCodeMatched)
            {
                throw new Exception("Verification Code has not been matched. How did you get here?");
            }

            if (message.Password != message.PasswordConfirmation)
            {
                return new OperationResult("Password confirmation does not match the password");
            }

            //TODO create user, saga has finished.

            return new OperationResult();
        }
    }
}
