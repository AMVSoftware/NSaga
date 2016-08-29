using System;
using System.Collections.Generic;
using NSaga;

namespace Tests.Stubs
{
    public class SagaRepositoryInMemoryStub : ISagaRepository
    {
        public Dictionary<Guid, object> Sagas { get; set; }

        public SagaRepositoryInMemoryStub()
        {
            Sagas = new Dictionary<Guid, object>();
        }



        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
        {
            if (Sagas.ContainsKey(correlationId))
            {
                return (TSaga)Sagas[correlationId];
            }
            return null;
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class
        {
            dynamic correlationId = ((dynamic)saga).CorrelationId;
            Sagas[correlationId] = saga;
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class
        {
            throw new NotImplementedException();
        }
    }
}