using System;
using NSaga;
using NSaga.AzureTables;

namespace Tests.AzureTables
{
    public class AzureTablesSagaRepositoryTests : SagaRepositoryTestsTemplate
    {
        public AzureTablesSagaRepositoryTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("NSagaAzureTableStorage");// "UseDevelopmentStorage=true";

            this.Sut = new AzureTablesSagaRepository(new TableClientFactory(connectionString), new JsonNetSerialiser(), new DumbSagaFactory());
        }
    }
}
