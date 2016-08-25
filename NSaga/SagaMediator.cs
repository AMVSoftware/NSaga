using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public class SagaMediator
    {
        private readonly ISagaRepository sagaRepository;

        public SagaMediator(ISagaRepository sagaRepository)
        {
            this.sagaRepository = sagaRepository;
        }


        public OperationResult Consume(ISagaMessage sagaMessage)
        {
            var saga = sagaRepository.Find(sagaMessage.CorrelationId);

            var initiatingMessage = sagaMessage as IInitiatingSagaMessage;
            if (initiatingMessage != null && saga != null)
            {
                // saga with this CorrelationID already exists, can't initiate it again
                throw new Exception("Saga have already been initialised");
            }

            if (initiatingMessage == null && saga == null)
            {
                throw new Exception("Saga does not exist in the storage, but consumed message is not an initiating message. Please start Saga from IInitiatingMessage");
            }

            if (initiatingMessage != null)
            {
                var sagaType = sagaMessage.GetType(); //TODO find saga type that implements InitiatedBy<TSagaMessage>

                saga = sagaRepository.InitiateSaga(sagaType);
                var operationResult = saga.Initiate(sagaMessage);
                sagaRepository.Save(saga);
                return operationResult;
            }



            // saga should 

            throw new NotImplementedException();
        }

    }
}
