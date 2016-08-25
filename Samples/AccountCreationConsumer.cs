using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSaga;

namespace Samples
{
    public class AccountCreationConsumer
    {
        public void TrySaga()
        {
            var sagaMediator = new SagaMediator(new StubSagaRepository());

            var initialMessage = new PersonalDetailsVerification();

            var initiatingResult = sagaMediator.Consume(initialMessage);
        }
    }
}
