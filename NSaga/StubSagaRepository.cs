using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public class StubSagaRepository : ISagaRepository
    {
        public ISaga<TSagaData> InitiateSaga<TSagaData>()
        {
            throw new NotImplementedException();
        }

        public ISaga<TSagaData> Find<TSagaData>(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public void Save<TSagaData>(ISaga<TSagaData> saga)
        {
            throw new NotImplementedException();
        }
    }
}
