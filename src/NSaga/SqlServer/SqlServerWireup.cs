using System;

namespace NSaga
{
    public static class SqlServerWireup
    {
        public static SqlServerBuilderExtension UseSqlServer(this SagaMediatorBuilder builder)
        {
            return new SqlServerBuilderExtension(builder);
        }
    }

    public sealed class SqlServerBuilderExtension
    {
        private readonly SagaMediatorBuilder builder;

        public SqlServerBuilderExtension(SagaMediatorBuilder builder)
        {
            this.builder = builder;
        }

        public SagaMediatorBuilder WithConnectionString(String connectionString)
        {
            builder.Register(typeof(IConnectionFactory), new ConnectionFactory(connectionString));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }

        public SagaMediatorBuilder WithConnectionStringName(String connectionStringName)
        {
            builder.Register(typeof(IConnectionFactory), ConnectionFactory.FromConnectionStringName(connectionStringName));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }
    }
}
