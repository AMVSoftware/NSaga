namespace NSaga.SqlServer
{
    public static class SqlServerWireup
    {
        public static AbstractSagaMediatorBuilder UseSqlServerStorage(this AbstractSagaMediatorBuilder builder, string connectionStringName)
        {
            //builder.UseRepository(new SqlSagaRepository(
            //                            connectionStringName,
            //                            builder.Container.Resolve<ISagaFactory>(),
            //                            builder.Container.Resolve<IMessageSerialiser>()));
            return builder;
        }

        public static AbstractSagaMediatorBuilder UseSqlServerStorage(this AbstractSagaMediatorBuilder builder, string connectionString, string providerName)
        {
            //builder.UseRepository(new SqlSagaRepository(
            //                            connectionString,
            //                            providerName,
            //                            builder.Container.Resolve<ISagaFactory>(),
            //                            builder.Container.Resolve<IMessageSerialiser>()));
            return builder;
        }

    }
}
