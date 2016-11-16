using System;
using System.Collections.Generic;
using System.Reflection;
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
            Set(saga, "CorrelationId", correlationId);

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

        private static void Set(object instance, string propertyName, object value)
        {
            var type = instance.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null || property.CanWrite == false)
            {
                return;
            }
            property.SetValue(instance, value, null);
        }
    }
}
