using System;

namespace NSaga
{
    public class NullSagaRepository : ISagaRepository
    {
        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
        {
            throw new NotImplementedException();
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class
        {
            throw new NotImplementedException();
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            throw new NotImplementedException();
        }
    }

    public class NullServiceLocator : IServiceLocator
    {
        public T Resolve<T>()
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
