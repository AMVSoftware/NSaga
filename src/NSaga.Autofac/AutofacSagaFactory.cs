using System;
using Autofac;

namespace NSaga.Autofac
{
    public class AutofacSagaFactory : ISagaFactory
    {
        private readonly IContainer container;

        public AutofacSagaFactory(IContainer container)
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
            throw new NotImplementedException();
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
