using NSaga;

namespace Samples
{
    public class AccountCreationConsumer
    {
        public void TrySaga()
        {
            var sagaMediator = new SagaMediator(new StubSagaRepository(), new StubServiceLocator());

            var initialMessage = new PersonalDetailsVerification();

            var initiatingResult = sagaMediator.Consume(initialMessage);
        }
    }
}
