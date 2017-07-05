using System;
using SimpleInjector;

namespace NSaga.SimpleInjector
{
    /// <summary>
    /// Implementation of <see cref="ISagaFactory"/> that uses SimpleInjector to resolve sagas.
    /// </summary>
    /// <seealso cref="NSaga.ISagaFactory" />
    public sealed class SimpleInjectorSagaFactory : ISagaFactory
    {
        private readonly Container container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInjectorSagaFactory"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public SimpleInjectorSagaFactory(Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            this.container = container;
        }

        /// <summary>
        /// Resolves the saga instance.
        /// </summary>
        /// <typeparam name="T">Type of Saga to be resolved</typeparam>
        /// <returns>
        /// An instance of saga
        /// </returns>
        public T ResolveSaga<T>() where T : class, IAccessibleSaga 
        {
            return container.GetInstance<T>();
        }

        /// <summary>
        /// Resolves the saga instance.
        /// </summary>
        /// <param name="type">The type of saga to be resolved</param>
        /// <returns>
        /// An instance of <see cref="T:NSaga.ISaga`1" />
        /// </returns>
        public IAccessibleSaga ResolveSaga(Type type)
        {
            return (IAccessibleSaga)container.GetInstance(type);
        }

        /// <summary>
        /// Resolves the saga inititated by a given message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An instance of <see cref="T:NSaga.ISaga`1" />
        /// </returns>
        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.GetInstance(interfaceType);
        }

        /// <summary>
        /// Resolves the saga consumed by a given message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// An instance of <see cref="T:NSaga.ISaga`1" />
        /// </returns>
        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.GetInstance(interfaceType);
        }
    }
}
