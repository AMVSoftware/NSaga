using NSaga;
using NSaga.SqlServer;

namespace Tests
{
    public class SqlSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public SqlSagaRepositoryTests()
        {
            this.Sut = new SqlSagaRepository(new DumbServiceLocator(), "");
        }
    }
}