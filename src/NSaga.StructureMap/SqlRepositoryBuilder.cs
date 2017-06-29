using System;
using StructureMap;

namespace NSaga.StructureMap
{
    /// <summary>
    /// SqlRepository builder is a helper class that aids with registration of <see cref="SqlSagaRepository"/> in StructureMap container.
    /// </summary>
    public sealed class SqlRepositoryBuilder
    {
        private Container container;

        /// <summary>
        /// Creates an instance of <see cref="SqlSagaRepository"/>
        /// </summary>
        /// <param name="container">StructureMap Container to register instances in</param>
        public SqlRepositoryBuilder(Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            this.container = container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string
        /// </summary>
        /// <param name="connectionString">Connection string to be used for SqlSagaRepository</param>
        /// <returns>StructureMap Container for fluent configuration</returns>
        public Container WithConnectionString(String connectionString)
        {
            container.Configure(x =>
            {               
                x.For<IConnectionFactory>().Use<ConnectionFactory>(new ConnectionFactory(connectionString));
                x.For<ISagaRepository>().ClearAll().Use<SqlSagaRepository>();
            });

            return container;
        }

        /// <summary>
        /// Register <see cref="SqlSagaRepository"/> with a given connection string name. Actual connection string looked up from app.config or web.config by a provided name.
        /// </summary>
        /// <param name="connectionString">Name of the connection string in app.config or web.config</param>
        /// <returns>StructureMap Container for fluent configuration</returns>
        public Container WithConnectionStringName(String connectionString)
        {
            container.Configure(x =>
            {
                x.For<IConnectionFactory>().Use<ConnectionFactory>(ConnectionFactory.FromConnectionStringName(connectionString));
                x.For<ISagaRepository>().ClearAll().Use<SqlSagaRepository>();
            });

            return container;
        }
    }
}
