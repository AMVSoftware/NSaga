using System;
using System.Collections.Generic;
using NSaga;

namespace Tests.Stubs
{
    public class SagaRepositoryInMemoryStub : ISagaRepository
    {
        public SagaRepositoryInMemoryStub()
        {
            Sagas = new Dictionary<Guid, ISaga<MySagaData>>();
        }

        public Dictionary<Guid, ISaga<MySagaData>> Sagas { get; set; }


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