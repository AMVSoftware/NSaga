using System;
using Autofac;

namespace NSaga.Autofac
{
    /// <summary>
    /// SqlRepository builder is a helper class that aids with registration of <see cref="SqlSagaRepository"/> in Autofac container.
    /// </summary>
    public sealed class SqlRepositoryBuilder
    {
        private readonly ContainerBuilder container;

        /// <summary>
        /// Creates an instance of <see cref="SqlSagaRepository"/>
        /// </summary>
        /// <param name="container">Autofac ContainerBuilder to register instances in</param>
        public SqlRepositoryBuilder(ContainerBuilder container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            this.container = container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string
        /// </summary>
        /// <param name="connectionString">Connection string to be used for SqlSagaRepository</param>
        /// <returns>Autofac ContainerBuilder for fluent configuration</returns>
        public ContainerBuilder WithConnectionString(String connectionString)
        {
            container.Register(c => new ConnectionFactory(connectionString)).As<IConnectionFactory>();

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string name. Actual connection string looked up from app.config or web.config by a provided name.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string in app.config or web.config</param>
        /// <returns>Autofac ContainerBuilder for fluent configuration</returns>
        public ContainerBuilder WithConnectionStringName(String connectionStringName)
        {
            container.Register(c => ConnectionFactory.FromConnectionStringName(connectionStringName)).As<IConnectionFactory>();

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }
    }
}