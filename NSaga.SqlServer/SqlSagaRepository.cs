using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace NSaga.SqlServer
{
    public class SqlSagaRepository : ISagaRepository
    {
        private readonly IServiceLocator serviceLocator;
        private readonly String connectionString;
        private readonly Database database;

        public SqlSagaRepository(IServiceLocator serviceLocator, string connectionString)
        {
            this.connectionString = connectionString;
            this.serviceLocator = serviceLocator;
            this.database = new Database(connectionString);
        }


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

        public void Complete(Guid correlationId)
        {
            throw new NotImplementedException();
        }
    }



    [TableName("NSaga.Sagas")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaData
    {
        public Guid CorrelationId { get; set; }
        public String BlobData { get; set; }
    }


    [TableName("NSaga.Headers")]
    [PrimaryKey("CorrelationId", AutoIncrement = false)]
    class SagaHeaders
    {
        public Guid CorrelationId { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
    }
}
