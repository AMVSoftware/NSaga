using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace NSaga
{
    /// <summary>
    /// Factory for creating <see cref="IDbConnection"/> to connect to a database.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Creates the open connection.
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateOpenConnection();
    }

    /// <summary>
    /// Default implementation of <see cref="IConnectionFactory"/> that returns <see cref="SqlConnection"/> either from actual connection string or from connection string name in your config file.
    /// </summary>
    /// <seealso cref="NSaga.IConnectionFactory" />
    public sealed class ConnectionFactory : IConnectionFactory
    {
        private readonly String connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }


        /// <summary>
        /// Creates the open connection.
        /// </summary>
        /// <returns><see cref="SqlConnection"/> with provided connection string</returns>
        public IDbConnection CreateOpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }


        /// <summary>
        /// Creates an instance of <see cref="ConnectionFactory"/> by specifying a connection string name. 
        /// Looks up the actual connection string from your .config file.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns>An instance of <see cref="ConnectionFactory"/></returns>
        /// <exception cref="System.ArgumentException">Connection string name must not be null or empty - connectionStringName</exception>
        /// <exception cref="System.InvalidOperationException"></exception>
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
