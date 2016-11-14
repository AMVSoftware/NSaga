using NSaga;
using Xunit;


namespace Tests.SqlServer
{
    [AutoRollback]
    [Collection("Sql Tests")]
    public class SqlSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public SqlSagaRepositoryTests()
        {
            this.Sut = new SqlSagaRepository(ConnectionFactory.FromConnectionStringName("TestingConnectionString"), new DumbSagaFactory(), new JsonNetSerialiser());
        }
    }
}