using System;
using NSaga;

namespace Tests
{
    public class NullSagaRepository : ISagaRepository
    {
        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class { throw new NotImplementedException(); }
        public void Save<TSaga>(TSaga saga) where TSaga : class { throw new NotImplementedException(); }
        public void Complete<TSaga>(TSaga saga) where TSaga : class { throw new NotImplementedException(); }
        public void Complete(Guid correlationId) { throw new NotImplementedException(); }
    }
}