using System;
using Autofac;

namespace NSaga.Autofac
{
    public class SqlRepositoryBuilder
    {
        private readonly ContainerBuilder container;

        public SqlRepositoryBuilder(ContainerBuilder container)
        {
            this.container = container;
        }

        public ContainerBuilder WithConnectionString(String connectionString)
        {
            container.Register(c => new ConnectionFactory(connectionString)).As<IConnectionFactory>();

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }

        public ContainerBuilder WithConnectionStringName(String connectionStringName)
        {
            container.Register(c => ConnectionFactory.FromConnectionStringName(connectionStringName)).As<IConnectionFactory>();

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }
    }
}