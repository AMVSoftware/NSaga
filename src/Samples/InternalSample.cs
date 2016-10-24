using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSaga;

namespace Samples
{
    public class InternalSample
    {
        private  ISagaMediator _sagaMediator;
        private  ISagaRepository _sagaRepository;

        public  void Run()
        {
            var builder = Wireup.UseInternalContainer();
            _sagaMediator = builder.BuildMediator();

            _sagaRepository = builder.Container.Resolve<ISagaRepository>();


            var correlationId = Guid.NewGuid();

            StartSaga(correlationId);

            RequestVerificationCode(correlationId);

            ProvideVerificationCode(correlationId);

            CreateAccount(correlationId);

            var saga = _sagaRepository.Find<AccountCreationSaga>(correlationId);
            var jamesName = saga.SagaData.Person.FullName;
            Console.WriteLine($"Taking information from SagaData; Person.FullName='{jamesName}'");

            // and time to remove saga from the storage
            _sagaRepository.Complete(correlationId);

            Console.WriteLine("Press Any Key");
            Console.ReadKey();
        }



        private  void StartSaga(Guid correlationId)
        {
            var initialMessage = new PersonalDetailsVerification(correlationId)
            {
                DateOfBirth = new DateTime(1920, 11, 11),
                FirstName = "James",
                LastName = "Bond",
                HomePostcode = "MI6 HQ",
                PayrollNumber = "007",
            };

            var result = _sagaMediator.Consume(initialMessage);
            if (!result.IsSuccessful)
            {
                Console.WriteLine(result.ToString());
            }
        }


        private  void RequestVerificationCode(Guid correlationId)
        {
            var verificationRequest = new VerificationCodeRequest(correlationId);

            _sagaMediator.Consume(verificationRequest);
        }



        private  void ProvideVerificationCode(Guid correlationId)
        {
            var verificationCode = new VerificationCodeProvided(correlationId)
            {
                VerificationCode = "123456",
            };
            _sagaMediator.Consume(verificationCode);
        }



        private  void CreateAccount(Guid correlationId)
        {
            var accountCreation = new AccountDetailsProvided(correlationId)
            {
                Username = "James.Bond",
                Password = "James Is Awesome!",
                PasswordConfirmation = "james is awesome",
            };

            var excutionResult = _sagaMediator.Consume(accountCreation);

            if (!excutionResult.IsSuccessful)
            {
                Console.WriteLine(excutionResult.ToString());
            }
        }

    }
}
