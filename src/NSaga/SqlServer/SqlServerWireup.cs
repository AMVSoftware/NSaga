using System;

namespace NSaga
{
    /// <summary>
    /// Extension class that allows SqlSagaRepository to be configured as part of general Wireup builder
    /// </summary>
    public static class SqlServerWireup
    {
        public static SqlServerBuilderExtension UseSqlServer(this SagaMediatorBuilder builder)
        {
            return new SqlServerBuilderExtension(builder);
        }
    }

    /// <summary>
    /// Class that allows SqlSagaRepository to be configured as part of general Wireup builder
    /// </summary>
    public sealed class SqlServerBuilderExtension
    {
        private readonly SagaMediatorBuilder builder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerBuilderExtension"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public SqlServerBuilderExtension(SagaMediatorBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Configures a connection string that will be used in SqlSagaRepository
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public SagaMediatorBuilder WithConnectionString(String connectionString)
        {
            builder.Register(typeof(IConnectionFactory), new ConnectionFactory(connectionString));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }

        /// <summary>
        /// Configures a connection string name that will be used in SqlSagaRepository
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns></returns>
        public SagaMediatorBuilder WithConnectionStringName(String connectionStringName)
        {
            builder.Register(typeof(IConnectionFactory), ConnectionFactory.FromConnectionStringName(connectionStringName));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }
    }
}
