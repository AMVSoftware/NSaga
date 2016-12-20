using System;
using NSaga;
using NSaga.AzureTables;

namespace Tests.AzureTables
{
    public class AzureTablesSagaRepositoryTests : SagaRepositoryTestsTemplate, IDisposable
    {
        public AzureTablesSagaRepositoryTests()
        {
            var connectionString = AzureTablesHelper.GetConnectionString();

            this.Sut = new AzureTablesSagaRepository(new TableClientFactory(connectionString), new JsonNetSerialiser(), new DumbSagaFactory());
        }


        public void Dispose()
        {
            var connectionString = AzureTablesHelper.GetConnectionString();

            var tableClient = new TableClientFactory(connectionString).CreateTableClient();

            var table = tableClient.GetTableReference("nsaga");

            table.DeleteIfExists();
        }
    }
}
