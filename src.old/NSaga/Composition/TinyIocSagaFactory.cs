using System;


namespace NSaga
{
    internal class TinyIocSagaFactory : ISagaFactory
    {
        private readonly TinyIoCContainer container;

        public TinyIocSagaFactory(TinyIoCContainer container)
        {
            this.container = container;
        }

        public T ResolveSaga<T>() where T : class, IAccessibleSaga
        {
            var result = container.Resolve<T>();

            return result;
        }


        public IAccessibleSaga ResolveSaga(Type type)
        {
            var result = container.Resolve(type);

            return (IAccessibleSaga)result;
        }

        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            var interfaceType = typeof(InitiatedBy<>).MakeGenericType(message.GetType());
            var saga = container.Resolve(interfaceType);
            return (IAccessibleSaga)saga;
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            var interfaceType = typeof(ConsumerOf<>).MakeGenericType(message.GetType());
            var saga = container.Resolve(interfaceType);
            return (IAccessibleSaga)saga;
        }
    }
}
