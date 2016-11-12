using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace NSaga.SqlServer
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();
    }

    public class ConnectionFactory : IConnectionFactory
    {
        private readonly String connectionString;

        public ConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public IDbConnection GetConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }


        public static ConnectionFactory FromConnectionStringName(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ArgumentException("Connection string name must not be null or empty", nameof(connectionStringName));
            }
            var entry = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (entry == null)
            {
                throw new InvalidOperationException($"Can't find a connection string with the name '{connectionStringName}'");
            }

            var connectionString = entry.ConnectionString;

            return new ConnectionFactory(connectionString);
        }
    }
}
