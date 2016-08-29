using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    public class SagaMediator
    {
        private readonly ISagaRepository sagaRepository;
        private readonly IServiceLocator serviceLocator;
        private readonly Assembly[] assembliesToScan;

        public SagaMediator(ISagaRepository sagaRepository, IServiceLocator serviceLocator, params Assembly[] assembliesToScan)
        {
            this.sagaRepository = sagaRepository;
            this.serviceLocator = serviceLocator;

            if (assembliesToScan.Length == 0)
            {
                this.assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
            }
            else
            {
                this.assembliesToScan = assembliesToScan;
            }
        }


        public OperationResult Consume(ISagaMessage sagaMessage)
        {
            // try to find the existing saga with this CorrelationID

            //var saga = sagaRepository.Find(sagaMessage.CorrelationId);

            //var initiatingMessage = sagaMessage as IInitiatingSagaMessage;
            //if (initiatingMessage != null && saga != null)
            //{
            //    // saga with this CorrelationID already exists, can't initiate it again
            //    throw new Exception("Saga have already been initialised");
            //}

            //if (initiatingMessage == null && saga == null)
            //{
            //    throw new Exception("Saga does not exist in the storage, but consumed message is not an initiating message. Please start Saga from IInitiatingMessage");
            //}

            //if (initiatingMessage != null)
            //{
            //    //var sagaType = sagaMessage.GetType(); //TODO find saga type that implements InitiatedBy<TSagaMessage>

            //    //saga = sagaRepository.InitiateSaga(sagaType);
            //    //var operationResult = saga.Initiate(sagaMessage);
            //    //sagaRepository.Save(saga);
            //    //return operationResult;
            //}

            //// saga should 

            throw new NotImplementedException();
        }


        public OperationResult Consume(IInitiatingSagaMessage initiatingMessage)
        {
            if (initiatingMessage.CorrelationId == default(Guid))
            {
                throw new ArgumentException("CorrelationId was not provided in the message. Please make sure you assign CorrelationId before initiating your Saga");
            }

            // find all sagas that are initiated by this message
            var sagaTypes = Reflection.GetSagaTypesInitiatedBy(initiatingMessage, assembliesToScan);

            if (!sagaTypes.Any())
            {
                throw new ArgumentException($"Message with type {initiatingMessage.GetType().Name} is not initiating any Sagas. Please add InitiatedBy<{initiatingMessage.GetType().Name}> to your Saga type");
            }

            // try to find sagas that already exist
            foreach (var sagaType in sagaTypes)
            {
                var existingSaga = Reflection.InvokeGenericMethod(sagaRepository, "Find", sagaType, initiatingMessage.CorrelationId);

                if (existingSaga != null)
                {
                    throw new ArgumentException($"Trying to initiate the same saga twice. {initiatingMessage.GetType().Name} is Initiating Message, but saga of type {sagaType.Name} with CorrelationId {initiatingMessage.CorrelationId} already exists");
                }
            }

            var operationResult = new OperationResult();
            // now create an instance of each saga and persist the data
            foreach (var sagaType in sagaTypes)
            {
                dynamic saga = serviceLocator.Resolve(sagaType);
                saga.CorrelationId = initiatingMessage.CorrelationId;

                var errors = (OperationResult)Reflection.InvokeMethod(saga, "Initiate", initiatingMessage);
                operationResult.Merge(errors);
                sagaRepository.Save(saga);
            }

            return operationResult;
        }
    }
}
