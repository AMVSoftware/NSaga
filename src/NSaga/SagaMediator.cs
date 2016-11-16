using System;
using System.Collections.Generic;


namespace NSaga
{
    /// <summary>
    /// Core of the system - routes messages to a correct Saga, preserves saga data, resolves instances of sagas. Does all the magic
    /// </summary>
    /// <seealso cref="NSaga.ISagaMediator" />
    public sealed class SagaMediator : ISagaMediator
    {
        private readonly ISagaRepository sagaRepository;
        private readonly ISagaFactory sagaFactory;
        private readonly IPipelineHook pipelineHook;

        /// <summary>
        /// Initializes a new instance of the <see cref="SagaMediator"/> class.
        /// </summary>
        /// <param name="sagaRepository">The saga repository.</param>
        /// <param name="sagaFactory">The saga factory.</param>
        /// <param name="pipelineHooks">The pipeline hooks.</param>
        public SagaMediator(ISagaRepository sagaRepository, ISagaFactory sagaFactory, IEnumerable<IPipelineHook> pipelineHooks)
        {
            Guard.ArgumentIsNotNull(sagaRepository, nameof(sagaRepository));
            Guard.ArgumentIsNotNull(sagaFactory, nameof(sagaFactory));
            Guard.ArgumentIsNotNull(pipelineHooks, nameof(pipelineHooks));

            this.sagaRepository = sagaRepository;
            this.sagaFactory = sagaFactory;
            this.pipelineHook = new CompositePipelineHook(pipelineHooks);
        }


        /// <summary>
        /// Consumes the specified initiating message and creates a new instance of the correlating saga.
        /// <para>Saga is not persisted if operation have failed</para>
        /// </summary>
        /// <param name="initiatingMessage">The initiating message.</param>
        /// <returns>
        /// Result of the operation
        /// </returns>
        /// <exception cref="System.ArgumentException"></exception>
        public OperationResult Consume(IInitiatingSagaMessage initiatingMessage)
        {
            Guard.CheckSagaMessage(initiatingMessage, nameof(initiatingMessage));

            var resolvedSaga = sagaFactory.ResolveSagaInititatedBy(initiatingMessage);
            var sagaType = resolvedSaga.GetType();

            // try to find sagas that already exist
            var existingSaga = NSagaReflection.InvokeGenericMethod(sagaRepository, "Find", sagaType, initiatingMessage.CorrelationId);
            if (existingSaga != null)
            {
                throw new ArgumentException($"Trying to initiate the same saga twice. {initiatingMessage.GetType().Name} is Initiating Message, but saga of type {sagaType.Name} with CorrelationId {initiatingMessage.CorrelationId} already exists");
            }

            // now create an instance of saga and persist the data
            var saga = sagaFactory.ResolveSaga(sagaType);
            NSagaReflection.Set(saga, "CorrelationId", initiatingMessage.CorrelationId);

            // if SagaData is null - create an instance of the object and assign to saga
            var sagaData = NSagaReflection.Get(saga, "SagaData");
            if (sagaData == null)
            {
                var sagaDataType = NSagaReflection.GetInterfaceGenericType(saga, typeof(ISaga<>));
                var newSagaData = Activator.CreateInstance(sagaDataType);
                NSagaReflection.Set(saga, "SagaData", newSagaData);
            }

            var sagaHeaders = NSagaReflection.Get(saga, "Headers");
            if (sagaHeaders == null)
            {
                NSagaReflection.Set(saga, "Headers", new Dictionary<String, String>());
            }

            pipelineHook.BeforeInitialisation(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga));

            var errors = (OperationResult)NSagaReflection.InvokeMethod(saga, "Initiate", initiatingMessage);

            pipelineHook.AfterInitialisation(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga, errors));

            if (errors.IsSuccessful)
            {
                sagaRepository.Save(saga);
                pipelineHook.AfterSave(new PipelineContext(initiatingMessage, (IAccessibleSaga)saga, errors));
            }

            return errors;
        }

        /// <summary>
        /// Consumes the specified saga message - finds the existing saga that can consume given message type and with matching CorrelationId.
        /// </summary>
        /// <param name="sagaMessage">The saga message.</param>
        /// <returns>
        /// Result of the operation
        /// </returns>
        /// <exception cref="System.ArgumentException"></exception>
        public OperationResult Consume(ISagaMessage sagaMessage)
        {
            Guard.CheckSagaMessage(sagaMessage, nameof(sagaMessage));

            var resolvedSaga = sagaFactory.ResolveSagaConsumedBy(sagaMessage);
            var sagaType = resolvedSaga.GetType();

            var saga = NSagaReflection.InvokeGenericMethod(sagaRepository, "Find", sagaType, sagaMessage.CorrelationId);
            if (saga == null)
            {
                throw new ArgumentException($"Saga with this CorrelationId does not exist. Please initiate a saga with IInitiatingMessage.");
            }

            pipelineHook.BeforeConsuming(new PipelineContext(sagaMessage, (IAccessibleSaga)saga));

            var errors = (OperationResult)NSagaReflection.InvokeMethod(saga, "Consume", sagaMessage);

            pipelineHook.AfterConsuming(new PipelineContext(sagaMessage, (IAccessibleSaga)saga, errors));

            if (errors.IsSuccessful)
            {
                sagaRepository.Save((IAccessibleSaga)saga);
                pipelineHook.AfterSave(new PipelineContext(sagaMessage, (IAccessibleSaga)saga, errors));
            }

            return errors;
        }
    }
}
