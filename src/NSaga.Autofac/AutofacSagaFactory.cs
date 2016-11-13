using System;
using Autofac;

namespace NSaga.Autofac
{
    public class AutofacSagaFactory : ISagaFactory
    {
        private readonly ILifetimeScope container;

        public AutofacSagaFactory(ILifetimeScope container)
        {
            this.container = container;
        }

        public T ResolveSaga<T>() where T : class, IAccessibleSaga
        {
            return container.Resolve<T>();
        }

        public IAccessibleSaga ResolveSaga(Type type)
        {
            return (IAccessibleSaga)container.Resolve(type);
        }

        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga) container.Resolve(interfaceType);
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            return (IAccessibleSaga)container.Resolve(interfaceType);
        }
    }
}
