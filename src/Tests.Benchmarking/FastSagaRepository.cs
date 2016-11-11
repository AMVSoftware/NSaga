using System;
using System.Collections.Generic;
using NSaga;

namespace Benchmarking
{
    /// <summary>
    /// Saga repository only for benchmarking. 
    /// Does not really store anything
    /// </summary>
    internal class FastSagaRepository : ISagaRepository
    {
        private readonly ISagaFactory sagaFactory;

        public FastSagaRepository(ISagaFactory sagaFactory)
        {
            this.sagaFactory = sagaFactory;
        }


        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            if (correlationId == Program.FirstGuid)
            {
                return null;
            }

            var saga = sagaFactory.ResolveSaga<TSaga>();
            Reflection.Set(saga, "CorrelationId", correlationId);

            return saga;
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            // nothing
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            // nothing
        }

        public void Complete(Guid correlationId)
        {
            // nothing
        }
    }
}
