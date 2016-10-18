namespace NSaga.SqlServer
{
    public static class BuilderExtension
    {
        public static SagaMediatorBuilder UseSqlServerStorage(this SagaMediatorBuilder builder, string connectionStringName)
        {
            builder.UseRepository(new SqlSagaRepository(
                                        connectionStringName, 
                                        builder.Container.Resolve<ISagaFactory>(),
                                        builder.Container.Resolve<IMessageSerialiser>()));
            return builder;
        }

        public static SagaMediatorBuilder UseSqlServerStorage(this SagaMediatorBuilder builder, string connectionString, string providerName)
        {
            builder.UseRepository(new SqlSagaRepository(
                                        connectionString,
                                        providerName,
                                        builder.Container.Resolve<ISagaFactory>(),
                                        builder.Container.Resolve<IMessageSerialiser>()));
            return builder;
        }

    }
}
