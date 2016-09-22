using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    public class SagaMediator : ISagaMediator
    {
        private readonly ISagaRepository sagaRepository;
        private readonly IServiceLocator serviceLocator;
        private readonly IPipelineHook pipelineHook;
        private readonly Assembly[] assembliesToScan;

        public SagaMediator(ISagaRepository sagaRepository, IServiceLocator serviceLocator, IPipelineHook pipelineHook, params Assembly[] assembliesToScan)
        {
            Guard.ArgumentIsNotNull(sagaRepository, nameof(sagaRepository));
            Guard.ArgumentIsNotNull(serviceLocator, nameof(serviceLocator));
            Guard.ArgumentIsNotNull(pipelineHook, nameof(pipelineHook));

            this.sagaRepository = sagaRepository;
            this.serviceLocator = serviceLocator;
            this.pipelineHook = pipelineHook;

            if (assembliesToScan.Length == 0)
            {
                this.assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
            }
            else
            {
                this.assembliesToScan = assembliesToScan;
            }
        }


        public OperationResult Consume(IInitiatingSagaMessage initiatingMessage)
        {
            Guard.CheckSagaMessage(initiatingMessage, nameof(initiatingMessage));

            // find all sagas that can be initiated by this message
            var sagaTypes = Reflection.GetSagaTypesInitiatedBy(initiatingMessage, assembliesToScan);
            var sagaType = GetSingleSagaType(initiatingMessage, sagaTypes);

            // try to find sagas that already exist
            var existingSaga = Reflection.InvokeGenericMethod(sagaRepository, "Find", sagaType, initiatingMessage.CorrelationId);
            if (existingSaga != null)
            {
                throw new ArgumentException($"Trying to initiate the same saga twice. {initiatingMessage.GetType().Name} is Initiating Message, but saga of type {sagaType.Name} with CorrelationId {initiatingMessage.CorrelationId} already exists");
            }

            // now create an instance of saga and persist the data
            var saga = serviceLocator.Resolve(sagaType);
            Reflection.Set(saga, "CorrelationId", initiatingMessage.CorrelationId);

            // if SagaData is null - create an instance of the object and assign to saga
            var sagaData = Reflection.Get(saga, "SagaData");
            if (sagaData == null)
            {
                var sagaDataType = Reflection.GetInterfaceGenericType(saga, typeof(ISaga<>));
                var newSagaData = Activator.CreateInstance(sagaDataType);
                Reflection.Set(saga, "SagaData", newSagaData);
            }

            var sagaHeaders = Reflection.Get(saga, "Headers");
            if (sagaHeaders == null)
            {
                Reflection.Set(saga, "Headers", new Dictionary<String, String>());
            }

            pipelineHook.BeforeInitialisation(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga));

            var errors = (OperationResult)Reflection.InvokeMethod(saga, "Initiate", initiatingMessage);

            pipelineHook.AfterInitialisation(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga, errors));

            if (errors.IsSuccessful)
            {
                sagaRepository.Save(saga);
                pipelineHook.AfterSave(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga, errors));
            }

            return errors;
        }

        public OperationResult Consume(ISagaMessage sagaMessage)
        {
            Guard.CheckSagaMessage(sagaMessage, nameof(sagaMessage));

            var sagaTypes = Reflection.GetSagaTypesConsuming(sagaMessage, assembliesToScan);
            var sagaType = GetSingleSagaType(sagaMessage, sagaTypes);

            var saga = Reflection.InvokeGenericMethod(sagaRepository, "Find", sagaType, sagaMessage.CorrelationId);
            if (saga == null)
            {
                throw new ArgumentException($"Saga with this CorrelationId does not exist. Please initiate a saga with IInitiatingMessage.");
            }

            pipelineHook.BeforeConsuming(new PipelineContext(sagaMessage, (IAccessibleSaga)saga));

            var errors = (OperationResult)Reflection.InvokeMethod(saga, "Consume", sagaMessage);

            pipelineHook.AfterConsuming(new PipelineContext(sagaMessage, (IAccessibleSaga)saga, errors));

            if (errors.IsSuccessful)
            {
                sagaRepository.Save(saga);
                pipelineHook.AfterSave(new PipelineContext(sagaMessage, (IAccessibleSaga)saga, errors));
            }

            return errors;
        }


        private Type GetSingleSagaType(ISagaMessage sagaMessage, IEnumerable<Type> sagaTypes)
        {
            if (!sagaTypes.Any())
            {
                throw new ArgumentException(
                    $"Message of type {sagaMessage.GetType().Name} is not initiating or consumed by any Sagas. Please add InitiatedBy<{sagaMessage.GetType().Name}> or ConsumedBy<{sagaMessage.GetType().Name}> to your Saga type");
            }
            if (sagaTypes.Count() > 1)
            {
                // can't have multiple sagas initiated by the same message - can't have 2 sagas of different types with the same CorrelationId
                var sagaNames = String.Join(", ", sagaTypes.Select(t => t.Name));
                throw new ArgumentException(
                    $"Message of type {sagaMessage.GetType().Name} is initiating or consumed by more than one saga. Please make sure any single message is initiating only one saga. Affected sagas: {sagaNames}");
            }

            return sagaTypes.Single();
        }
    }
}
