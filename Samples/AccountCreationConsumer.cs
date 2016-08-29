using NSaga;

namespace Samples
{
    public class AccountCreationConsumer
    {
        public void TrySaga()
        {
            var sagaMediator = new SagaMediator(new NullSagaRepository(), new NullServiceLocator());

            var initialMessage = new PersonalDetailsVerification();

            var initiatingResult = sagaMediator.Consume(initialMessage);
        }
    }
}
