using System;

namespace NSaga
{
    public class StubSagaRepository : ISagaRepository
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
}
