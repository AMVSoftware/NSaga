using System;
using NSaga;
using NSaga.Pipeline;


namespace Samples
{
    static class Program
    {
        private static SagaMediator _sagaMediator;
        private static ISagaRepository _sagaRepository;

        public static void Main(params string[] args)
        {
            var serviceLocator = new DumbSagaFactory();
            var messageSerialiser = new JsonNetSerialiser();
            _sagaRepository = new InMemorySagaRepository(messageSerialiser, serviceLocator);
            _sagaMediator = new SagaMediator(_sagaRepository, serviceLocator, new MetadataPipelineHook(messageSerialiser), typeof(Program).Assembly);

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

            var result = _sagaMediator.Consume(initialMessage);
            if (!result.IsSuccessful)
            {
                Console.WriteLine(result.ToString());
            }
        }


        private static void RequestVerificationCode(Guid correlationId)
        {
            var verificationRequest = new VerificationCodeRequest(correlationId);

            _sagaMediator.Consume(verificationRequest);
        }



        private static void ProvideVerificationCode(Guid correlationId)
        {
            var verificationCode = new VerificationCodeProvided(correlationId)
            {
                VerificationCode = "123456",
            };
            _sagaMediator.Consume(verificationCode);
        }



        private static void CreateAccount(Guid correlationId)
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
