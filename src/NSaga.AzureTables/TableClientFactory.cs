using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NSaga.AzureTables
{
    public interface ITableClientFactory
    {
        CloudTableClient CreateTableClient();
    }

    public class TableClientFactory : ITableClientFactory
    {
        private readonly String connectionString;

        public TableClientFactory(string connectioString)
        {
            this.connectionString = connectioString;
        }

        public CloudTableClient CreateTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            return tableClient;
        }
    }
}