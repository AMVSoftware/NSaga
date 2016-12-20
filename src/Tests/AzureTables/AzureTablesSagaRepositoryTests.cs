using System;
using NSaga;
using NSaga.AzureTables;

namespace Tests.AzureTables
{
    public class AzureTablesSagaRepositoryTests : SagaRepositoryTestsTemplate, IDisposable
    {
        public AzureTablesSagaRepositoryTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("NSagaAzureTableStorage");// "UseDevelopmentStorage=true";

            this.Sut = new AzureTablesSagaRepository(new TableClientFactory(connectionString), new JsonNetSerialiser(), new DumbSagaFactory());
        }



        public void Dispose()
        {
            var connectionString = Environment.GetEnvironmentVariable("NSagaAzureTableStorage");

            var tableClient = new TableClientFactory(connectionString).CreateTableClient();

            var table = tableClient.GetTableReference("nsaga");

            table.DeleteIfExists();
        }
    }
}
