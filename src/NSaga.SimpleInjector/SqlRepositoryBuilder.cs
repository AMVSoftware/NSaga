using System;
using SimpleInjector;

namespace NSaga.SimpleInjector
{
    /// <summary>
    /// SqlRepository builder is a helper class that aids with registration of <see cref="SqlSagaRepository"/> in SimpleInjector container.
    /// </summary>
    public sealed class SqlRepositoryBuilder
    {
        private readonly Container container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlRepositoryBuilder"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public SqlRepositoryBuilder(Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            this.container = container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string
        /// </summary>
        /// <param name="connectionString">Connection string to be used for SqlSagaRepository</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public Container WithConnectionString(String connectionString)
        {
            container.Register(typeof(IConnectionFactory), () => new ConnectionFactory(connectionString));

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string name. Actual connection string looked up from app.config or web.config by a provided name.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string in app.config or web.config</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public Container WithConnectionStringName(String connectionStringName)
        {
            container.Register(typeof(IConnectionFactory), () => ConnectionFactory.FromConnectionStringName(connectionStringName));

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }
    }
}