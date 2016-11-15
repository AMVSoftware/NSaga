using System;
using Autofac;

namespace NSaga.Autofac
{
    /// <summary>
    /// Implementation of <see cref="ISagaFactory"/> that uses Autofac container to resolve intances of Sagas
    /// </summary>
    public sealed class AutofacSagaFactory : ISagaFactory
    {
        private readonly ILifetimeScope container;

        /// <summary>
        /// Creates an instance of <see cref="AutofacSagaFactory"/> 
        /// </summary>
        /// <param name="container">Reference to Autofac Container</param>
        public AutofacSagaFactory(ILifetimeScope container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            this.container = container;
        }

        /// <summary>
        /// Resolves instance of Saga of generic type T
        /// </summary>
        /// <typeparam name="T">Type of saga to resolve</typeparam>
        /// <returns>An instance of Saga of the requested type</returns>
        public T ResolveSaga<T>() where T : class, IAccessibleSaga
        {
            return container.Resolve<T>();
        }

        /// <summary>
        /// Resolves intance of Saga of the provided type
        /// </summary>
        /// <param name="type">Type of Saga to resolve</param>
        /// <returns>An instance of a Saga</returns>
        public IAccessibleSaga ResolveSaga(Type type)
        {
            return (IAccessibleSaga)container.Resolve(type);
        }

        /// <summary>
        /// Resolves an instance of Saga that is intiated by a given message
        /// </summary>
        /// <param name="message">An instance of a message</param>
        /// <returns>An instance of a Saga</returns>
        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga) container.Resolve(interfaceType);
        }

        /// <summary>
        /// Resolves an instance of Saga that can consume the given message
        /// </summary>
        /// <param name="message">An instance of a message</param>
        /// <returns>An instance of a Saga</returns>
        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.Resolve(interfaceType);
        }
    }
}
