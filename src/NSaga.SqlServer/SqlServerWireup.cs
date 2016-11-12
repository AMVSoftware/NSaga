namespace NSaga.SqlServer
{
    public static class SqlServerWireup
    {
        public static SagaMediatorBuilder UseSqlServerConnectionStringName(this SagaMediatorBuilder builder, string connectionStringName)
        {
            builder.Register(typeof(IConnectionFactory), ConnectionFactory.FromConnectionStringName(connectionStringName));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }


        public static SagaMediatorBuilder UseSqlServerConnectionString(this SagaMediatorBuilder builder, string connectionString)
        {
            builder.Register(typeof(IConnectionFactory), new ConnectionFactory(connectionString));

            builder.UseRepository<SqlSagaRepository>();

            return builder;
        }
    }
}
