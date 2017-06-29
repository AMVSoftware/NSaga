using StructureMap;

namespace NSaga.StructureMap
{
    public sealed class SqlRepositoryBuilder
    {
        private Container container;

        public SqlRepositoryBuilder(Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            this.container = container;
        }

        public Container WithConnectionString(string connectionString)
        {
            container.Configure(x =>
            {
                x.For<IConnectionFactory>().Use<ConnectionFactory>(new ConnectionFactory(connectionString));
                x.For<SqlSagaRepository>().Use<SqlSagaRepository>();
            });

            return container;
        }

        public Container WithConnectionStringName(string connectionString)
        {
            container.Configure(x =>
            {
                x.For<IConnectionFactory>().Use<ConnectionFactory>(ConnectionFactory.FromConnectionStringName(connectionString));
                x.For<SqlSagaRepository>().Use<SqlSagaRepository>();
            });

            return container;
        }
    }
}
