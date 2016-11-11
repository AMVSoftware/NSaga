using System;
using NSaga;

namespace Tests
{
    public class NullSagaFactory : ISagaFactory
    {
        public T ResolveSaga<T>() where T : class, IAccessibleSaga { throw new NotImplementedException(); }
        public IAccessibleSaga ResolveSaga(Type type) { throw new NotImplementedException(); }
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