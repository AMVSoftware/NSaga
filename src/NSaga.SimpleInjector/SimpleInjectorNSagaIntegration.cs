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
            Guard.ArgumentIsNotNull(container, nameof(container));

            return container.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static Container RegisterNSagaComponents(this Container container, params Assembly[] assemblies)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(assemblies, nameof(assemblies));

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
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), typeof(TSagaRepository)));

            return container;
        }


        public static Container UseSagaRepository(this Container container, Func<ISagaRepository> repositoryFactory)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(repositoryFactory, nameof(repositoryFactory));

            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), repositoryFactory));

            return container;
        }


        public static Container AddSagaPipelineHook<TPipelineHook>(this Container container) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.AppendToCollection(typeof(IPipelineHook), typeof(TPipelineHook));

            return container;
        }


        public static Container UseMessageSerialiser<TMessageSerialiser>(this Container container) where TMessageSerialiser : IMessageSerialiser
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), typeof(TMessageSerialiser)));

            return container;
        }

        public static Container UseMessageSerialiser(this Container container, Func<IMessageSerialiser> messageSerialiserFactory)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(messageSerialiserFactory, nameof(messageSerialiserFactory));

            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), messageSerialiserFactory));

            return container;
        }


        public static SqlRepositoryBuilder UseSqlServer(this Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            return new SqlRepositoryBuilder(container);
        }


        private static void OverrideRegistration(this Container container, Action<Container> act)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            var oldValue = container.Options.AllowOverridingRegistrations;
            container.Options.AllowOverridingRegistrations = true;

            act.Invoke(container);

            container.Options.AllowOverridingRegistrations = oldValue;
        }
    }
}
