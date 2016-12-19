using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga.AzureTables
{
    public class AzureTablesSagaRepository : ISagaRepository
    {
        public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
        {
            throw new NotImplementedException();
        }

        public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            throw new NotImplementedException();
        }

        public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
        {
            throw new NotImplementedException();
        }

        public void Complete(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }
}
