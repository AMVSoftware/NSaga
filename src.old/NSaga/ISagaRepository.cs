using System;

namespace NSaga
{
    /// <summary>
    /// Repository to preserve sagas into storage
    /// </summary>
    public interface ISagaRepository
    {
        /// <summary>
        /// Finds and returns saga instance with the given correlation ID.
        /// Actually creates an instance of saga from Saga factory, retrieves SagaData and Headers from the storage and populates the instance with these.
        /// </summary>
        /// <typeparam name="TSaga">Type of saga we are looking for</typeparam>
        /// <param name="correlationId">CorrelationId to identify the saga</param>
        /// <returns>An instance of the saga. Or Null if there is no saga with this ID.</returns>
        TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga;

        /// <summary>
        /// Persists the instance of saga into the database storage.
        /// Actually stores SagaData and Headers. All other variables in saga are not persisted
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga instance</param>
        void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga;

        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <typeparam name="TSaga">Type of saga</typeparam>
        /// <param name="saga">Saga to be deleted</param>
        void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga;

        /// <summary>
        /// Deletes the saga instance from the storage
        /// </summary>
        /// <param name="correlationId">Correlation Id for the saga</param>
        void Complete(Guid correlationId);
    }
}
