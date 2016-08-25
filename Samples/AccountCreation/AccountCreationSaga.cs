using System;
using NSaga;

namespace Samples
{
    public class AccountCreationSaga : ISaga<AccountCreationSagaData>,
                                       InitiatedBy<PersonalDetailsVerification>,
                                       ConsumerOf<VerificationCodeRequest>,
                                       ConsumerOf<VerificationCodeProvided>,
                                       ConcludeBy<AccountDetailsProvided>
    {
        public AccountCreationSagaData SagaData { get; set; }
        public Guid CorrelationId { get; set; }

        public AccountCreationSaga(/*Some dependencies*/)
        {
            
        }


        public OperationResult Initiate(PersonalDetailsVerification message)
        {
            if (false)
            {
                return new OperationResult("Person details don't match. Please try again");
            }

            var person = new Person()
            {
                FirstName = message.FirstName,
                LastName = message.LastName,
                ReferenceNumber = message.PayrollNumber,
            };
            SagaData.Person = person;
            return new OperationResult();
        }


        public OperationResult Consume(VerificationCodeRequest message)
        {
            if (!SagaData.IsPersonalDetailsVerified)
            {
                throw new Exception("Person details are not verified. How did you get here?");
            }


            if (message.RequestCarrier == "Mobile")
            {
                //TODO send SMS to SagaData.Person.Mobile
            }
            else
            {
                //TODO send email to SagaData.Person.PrivateEmail
            }
            SagaData.VerificationCode = "123456";
            SagaData.VerificationCodeSentDate = DateTime.UtcNow;

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

            SagaData.IsVerificationCodeMatched = true;

            return new OperationResult();
        }


        public OperationResult Conclude(AccountDetailsProvided message)
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
