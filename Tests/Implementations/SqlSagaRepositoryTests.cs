using System.Configuration;
using NSaga;
using NSaga.SqlServer;

namespace Tests
{
    public class SqlSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public SqlSagaRepositoryTests()
        {
            var connectionString = ConfigurationManager.AppSettings["TestingConnectionString"];

            this.Sut = new SqlSagaRepository(new DumbServiceLocator(), connectionString, new JsonNetSerialiser());
        }
    }
}