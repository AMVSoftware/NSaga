using NSaga;
using NSaga.SqlServer;


namespace Tests.SqlServer
{
    public class SqlSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public SqlSagaRepositoryTests()
        {
            this.Sut = new SqlSagaRepository("TestingConnectionString", new DumbSagaFactory(), new JsonNetSerialiser());
        }
    }
}