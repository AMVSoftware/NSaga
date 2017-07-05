using System;

namespace NSaga
{
    /// <summary>
    /// Factory that creates saga instances. This does not deal with SagaData, but deals with saga dependencies.
    /// <para>This abastraction to hide a Dependency Injection container that does the class resolution.</para>
    /// </summary>
    public interface ISagaFactory
    {
        /// <summary>
        /// Resolves the saga instance.
        /// </summary>
        /// <typeparam name="T">Type of Saga to be resolved</typeparam>
        /// <returns>An instance of saga</returns>
        T ResolveSaga<T>() where T : class, IAccessibleSaga;

        /// <summary>
        /// Resolves the saga instance.
        /// </summary>
        /// <param name="type">The type of saga to be resolved</param>
        /// <returns>An instance of <see cref="ISaga{TSagaData}"/></returns>
        IAccessibleSaga ResolveSaga(Type type);


        /// <summary>
        /// Resolves the saga inititated by a given message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An instance of <see cref="ISaga{TSagaData}"/></returns>
        IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message);


        /// <summary>
        /// Resolves the saga consumed by a given message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An instance of <see cref="ISaga{TSagaData}"/></returns>
        IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message);
    }
}
