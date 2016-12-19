using System;

namespace NSaga.AzureTables
{
    public class AzureTablesSagaRepository : ISagaRepository
    {
        private readonly ITableClientFactory tableClientFactory;

        public AzureTablesSagaRepository(ITableClientFactory tableClientFactory)
        {
            this.tableClientFactory = tableClientFactory;
        }


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
