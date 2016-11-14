using System;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;


namespace NSaga.SimpleInjector
{
    public static class SimpleInjectorNSagaIntegration
    {
        public static Container RegisterNSagaComponents(this Container container)
        {
            return container.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static Container RegisterNSagaComponents(this Container container, params Assembly[] assemblies)
        {
            container.Register<ISagaFactory, SimpleInjectorSagaFactory>();
            container.Register<IMessageSerialiser, JsonNetSerialiser>();
            container.Register<ISagaRepository, InMemorySagaRepository>();
            container.RegisterCollection<IPipelineHook>(new Type[] {typeof(MetadataPipelineHook)});
            container.Register<ISagaMediator, SagaMediator>();

            container.Register(typeof(ISaga<>), assemblies);
            container.Register(typeof(InitiatedBy<>), assemblies);
            container.Register(typeof(ConsumerOf<>), assemblies);

            return container;
        }

        public static Container UseSagaRepository<TSagaRepository>(this Container container) where TSagaRepository : ISagaRepository
        {
            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), typeof(TSagaRepository)));

            return container;
        }


        public static Container UseSagaRepository(this Container container, Func<ISagaRepository> repositoryFactory)
        {
            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), repositoryFactory));

            return container;
        }


        public static Container AddSagaPipelineHook<TPipelineHook>(this Container container) where TPipelineHook : IPipelineHook
        {
            container.AppendToCollection(typeof(IPipelineHook), typeof(TPipelineHook));

            return container;
        }


        public static Container UseMessageSerialiser<TMessageSerialiser>(this Container container) where TMessageSerialiser : IMessageSerialiser
        {
            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), typeof(TMessageSerialiser)));

            return container;
        }

        public static Container UseMessageSerialiser(this Container container, Func<IMessageSerialiser> messageSerialiserFactory)
        {
            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), messageSerialiserFactory));

            return container;
        }


        public static SqlServerRepositoryBuilder UseSqlServer(this Container container)
        {
            return new SqlServerRepositoryBuilder(container);
        }


        private static void OverrideRegistration(this Container container, Action<Container> act)
        {
            var oldValue = container.Options.AllowOverridingRegistrations;
            container.Options.AllowOverridingRegistrations = true;

            act.Invoke(container);

            container.Options.AllowOverridingRegistrations = oldValue;
        }
    }

    public class SqlServerRepositoryBuilder
    {
        private readonly Container container;

        public SqlServerRepositoryBuilder(Container container)
        {
            this.container = container;
        }

        public Container WithConnectionString(String connectionString)
        {
            container.Register(typeof(IConnectionFactory), () => new ConnectionFactory(connectionString));

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }

        public Container WithConnectionStringName(String connectionStringName)
        {
            container.Register(typeof(IConnectionFactory), () => ConnectionFactory.FromConnectionStringName(connectionStringName));

            container.UseSagaRepository<SqlSagaRepository>();

            return container;
        }
    }
}
