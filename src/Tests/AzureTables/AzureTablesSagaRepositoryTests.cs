using NSaga;
using NSaga.AzureTables;

namespace Tests.AzureTables
{
    public class AzureTablesSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public AzureTablesSagaRepositoryTests()
        {
            this.Sut = new AzureTablesSagaRepository(new TableClientFactory("UseDevelopmentStorage=true"), new JsonNetSerialiser(), new DumbSagaFactory());
        }
    }
}
