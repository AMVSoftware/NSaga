using System;

namespace NSaga
{
    public interface ISagaRepository
    {
        TSaga Find<TSaga>(Guid correlationId) where TSaga : class;
        void Save<TSaga>(TSaga saga) where TSaga : class;
        void Complete<TSaga>(TSaga saga) where TSaga : class;
    }
}
