//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NSaga;
//using NSaga.SqlServer;

//namespace Samples
//{
//    public class InternalSample
//    {
//        private  ISagaMediator sagaMediator;
//        private  ISagaRepository sagaRepository;

//        public  void Run()
//        {
//            var builder = Wireup.UseInternalContainer();

//            sagaMediator = builder.BuildMediator();

//            sagaRepository = builder.Container.Resolve<ISagaRepository>();

//            var correlationId = Guid.NewGuid();



//            StartSaga(correlationId);

//            RequestVerificationCode(correlationId);

//            ProvideVerificationCode(correlationId);

//            CreateAccount(correlationId);

//            var saga = sagaRepository.Find<AccountCreationSaga>(correlationId);
//            var jamesName = saga.SagaData.Person.FullName;
//            Console.WriteLine($"Taking information from SagaData; Person.FullName='{jamesName}'");

//            // and time to remove saga from the storage
//            sagaRepository.Complete(correlationId);

//            Console.WriteLine("Press Any Key");
//            Console.ReadKey();
//        }



//        private  void StartSaga(Guid correlationId)
//        {
//            var initialMessage = new PersonalDetailsVerification(correlationId)
//            {
//                DateOfBirth = new DateTime(1920, 11, 11),
//                FirstName = "James",
//                LastName = "Bond",
//                HomePostcode = "MI6 HQ",
//                PayrollNumber = "007",
//            };

//            var result = sagaMediator.Consume(initialMessage);
//            if (!result.IsSuccessful)
//            {
//                Console.WriteLine(result.ToString());
//            }
//        }


//        private  void RequestVerificationCode(Guid correlationId)
//        {
//            var verificationRequest = new VerificationCodeRequest(correlationId);

//            sagaMediator.Consume(verificationRequest);
//        }



//        private  void ProvideVerificationCode(Guid correlationId)
//        {
//            var verificationCode = new VerificationCodeProvided(correlationId)
//            {
//                VerificationCode = "123456",
//            };
//            sagaMediator.Consume(verificationCode);
//        }



//        private  void CreateAccount(Guid correlationId)
//        {
//            var accountCreation = new AccountDetailsProvided(correlationId)
//            {
//                Username = "James.Bond",
//                Password = "James Is Awesome!",
//                PasswordConfirmation = "james is awesome",
//            };

//            var excutionResult = sagaMediator.Consume(accountCreation);

//            if (!excutionResult.IsSuccessful)
//            {
//                Console.WriteLine(excutionResult.ToString());
//            }
//        }
//    }
//}
