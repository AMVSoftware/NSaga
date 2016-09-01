using System;
using NSaga;
using NSaga.Implementations;

namespace Samples
{
    static class Program
    {
        private static SagaMediator sagaMediator;
        private static ISagaRepository sagaRepository;

        public static void Main(params string[] args)
        {
            var serviceLocator = new DumbServiceLocator();
            sagaRepository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sagaMediator = new SagaMediator(sagaRepository, serviceLocator, typeof(Program).Assembly);

            var correlationId = Guid.NewGuid();

            StartSaga(correlationId);

            RequestVerificationCode(correlationId);

            ProvideVerificationCode(correlationId);

            CreateAccount(correlationId);

            var saga = sagaRepository.Find<AccountCreationSaga>(correlationId);
            var jamesName = saga.SagaData.Person.FullName;
            Console.WriteLine($"Taking information from SagaData; Person.FullName='{jamesName}'");

            // and time to remove saga from the storage
            sagaRepository.Complete(correlationId);

            Console.WriteLine("Press Any Key");
            Console.ReadKey();
        }



        private static void StartSaga(Guid correlationId)
        {
            var initialMessage = new PersonalDetailsVerification(correlationId)
            {
                DateOfBirth = new DateTime(1920, 11, 11),
                FirstName = "James",
                LastName = "Bond",
                HomePostcode = "MI6 HQ",
                PayrollNumber = "007",
            };

            var result = sagaMediator.Consume(initialMessage);
            if (!result.IsSuccessful)
            {
                Console.WriteLine(result.ToString());
            }
        }


        private static void RequestVerificationCode(Guid correlationId)
        {
            var verificationRequest = new VerificationCodeRequest(correlationId);

            sagaMediator.Consume(verificationRequest);
        }



        private static void ProvideVerificationCode(Guid correlationId)
        {
            var verificationCode = new VerificationCodeProvided(correlationId)
            {
                VerificationCode = "123456",
            };
            sagaMediator.Consume(verificationCode);
        }



        private static void CreateAccount(Guid correlationId)
        {
            var accountCreation = new AccountDetailsProvided(correlationId)
            {
                Username = "James.Bond",
                Password = "James Is Awesome!",
                PasswordConfirmation = "james is awesome",
            };

            var excutionResult = sagaMediator.Consume(accountCreation);

            if (!excutionResult.IsSuccessful)
            {
                Console.WriteLine(excutionResult.ToString());
            }
        }
    }
}
