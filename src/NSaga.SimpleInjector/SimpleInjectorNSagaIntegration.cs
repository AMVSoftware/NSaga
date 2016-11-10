using System;
using System.Reflection;
using NSaga.Pipeline;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace NSaga.SimpleInjector
{
    public static class SimpleInjectorNSagaIntegration
    {
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

        //TODO Test
        public static Container UseSagaRepository<TSagaRepository>(this Container container)
        {
            var oldValue = container.Options.AllowOverridingRegistrations;
            container.Options.AllowOverridingRegistrations = true;

            container.Register(typeof(ISagaRepository), typeof(TSagaRepository));

            container.Options.AllowOverridingRegistrations = oldValue;

            return container;
        }


        //TODO Test
        public static Container UseSagaRepository(this Container container, Func<ISagaRepository> repositoryFactory)
        {
            //var oldValue = container.Options.AllowOverridingRegistrations;
            //container.Options.AllowOverridingRegistrations = true;

            //container.Register(typeof(ISagaRepository), repositoryFactory);
            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), repositoryFactory));

            //container.Options.AllowOverridingRegistrations = oldValue;


            return container;
        }


        //TODO test
        //TODO override to register factory
        public static Container AddSagaPipelineHook<TPipelineHook>(this Container container)
            where TPipelineHook : IPipelineHook
        {
            container.AppendToCollection(typeof(IPipelineHook), typeof(TPipelineHook));

            return container;
        }


        //TODO override serialiser


        private static void OverrideRegistration(this Container container, Action<Container> act)
        {
            var oldValue = container.Options.AllowOverridingRegistrations;
            container.Options.AllowOverridingRegistrations = true;

            act.Invoke(container);

            container.Options.AllowOverridingRegistrations = oldValue;
        }
    }
}
