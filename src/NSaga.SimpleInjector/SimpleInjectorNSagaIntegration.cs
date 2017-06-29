using System;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;


namespace NSaga.SimpleInjector
{
    /// <summary>
    /// Adapter to integrate NSaga with SimpleInjector DI container
    /// </summary>
    public static class SimpleInjectorNSagaIntegration
    {
        /// <summary>
        /// Registers all the saga classes and all default components
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container RegisterNSagaComponents(this Container container)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            //exclude xunit runner from the loading assemblies, so the tests can be run via visual studio.net
            var subset = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.Contains("xunit"));
            return container.RegisterNSagaComponents(subset.ToArray());
        }

        /// <summary>
        /// Registers all the saga classes and all default components
        /// <para>
        /// Default registrations are:
        /// <list type="bullet">
        /// <item><description><see cref="JsonNetSerialiser"/> to serialise messages; </description></item> 
        /// <item><description><see cref="InMemorySagaRepository"/> to store saga datas; </description></item> 
        /// <item><description><see cref="SimpleInjectorSagaFactory"/> to resolve instances of Sagas;</description></item> 
        /// <item><description><see cref="SagaMetadata"/> to work as the key component - SagaMediator;</description></item> 
        /// <item><description><see cref="MetadataPipelineHook"/> added to the pipeline to preserve metadata about incoming messages.</description></item> 
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
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

        /// <summary>
        /// Override default ISagaRepository registration with the container.
        /// </summary>
        /// <typeparam name="TSagaRepository">The type of the saga repository.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container UseSagaRepository<TSagaRepository>(this Container container) where TSagaRepository : ISagaRepository
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), typeof(TSagaRepository)));

            return container;
        }


        /// <summary>
        /// Override default ISagaRepository registration with the container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="repositoryFactory">The repository factory.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container UseSagaRepository(this Container container, Func<ISagaRepository> repositoryFactory)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(repositoryFactory, nameof(repositoryFactory));

            container.OverrideRegistration(c => c.Register(typeof(ISagaRepository), repositoryFactory));

            return container;
        }


        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <typeparam name="TPipelineHook">The type of the pipeline hook.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container AddSagaPipelineHook<TPipelineHook>(this Container container) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.AppendToCollection(typeof(IPipelineHook), typeof(TPipelineHook));

            return container;
        }


        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>.
        /// </summary>
        /// <typeparam name="TMessageSerialiser">The type of the message serialiser.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container UseMessageSerialiser<TMessageSerialiser>(this Container container) where TMessageSerialiser : IMessageSerialiser
        {
            Guard.ArgumentIsNotNull(container, nameof(container));

            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), typeof(TMessageSerialiser)));

            return container;
        }

        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageSerialiserFactory">The message serialiser factory.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
        public static Container UseMessageSerialiser(this Container container, Func<IMessageSerialiser> messageSerialiserFactory)
        {
            Guard.ArgumentIsNotNull(container, nameof(container));
            Guard.ArgumentIsNotNull(messageSerialiserFactory, nameof(messageSerialiserFactory));

            container.OverrideRegistration(c => c.Register(typeof(IMessageSerialiser), messageSerialiserFactory));

            return container;
        }


        /// <summary>
        /// This chain is aiding with registering <see cref="SqlSagaRepository"/> as the storage mechanism. 
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>Intance of the same container for fluent configuration.</returns>
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
