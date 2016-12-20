using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace NSaga.AzureTables
{
    /// <summary>
    /// Factory for creating <see cref="CloudTableClient"/> to connect to Azure Tables
    /// </summary>
    public interface ITableClientFactory
    {
        /// <summary>
        /// Creates the table client.
        /// </summary>
        /// <returns>The created client</returns>
        CloudTableClient CreateTableClient();
    }

    /// <summary>
    /// Default Table Client factory. Simply creates the client from a connection string
    /// </summary>
    /// <seealso cref="NSaga.AzureTables.ITableClientFactory" />
    public class TableClientFactory : ITableClientFactory
    {
        private readonly String connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableClientFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connectio string.</param>
        public TableClientFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Creates the table client.
        /// </summary>
        /// <returns>
        /// The created client
        /// </returns>
        public CloudTableClient CreateTableClient()
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            return tableClient;
        }
    }
}