using System;
using SimpleInjector;

namespace NSaga.SimpleInjector
{
    public sealed class SimpleInjectorSagaFactory : ISagaFactory
    {
        private readonly Container container;

        public SimpleInjectorSagaFactory(Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            this.container = container;
        }

        public T ResolveSaga<T>() where T : class, IAccessibleSaga 
        {
            return container.GetInstance<T>();
        }

        public IAccessibleSaga ResolveSaga(Type type)
        {
            return (IAccessibleSaga)container.GetInstance(type);
        }

        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.GetInstance(interfaceType);
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.GetInstance(interfaceType);
        }
    }
}
