using System;
using System.Linq;
using System.Reflection;
using Autofac;


namespace NSaga.Autofac
{
    /// <summary>
    /// Adapter to integrate NSaga with Autofac DI container
    /// </summary>
    public static class AutofacNSagaIntegration
    {
        /// <summary>
        /// Constructor to create <see cref="AutofacNSagaIntegration"/> adapter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterNSagaComponents(this ContainerBuilder builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            return builder.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }


        /// <summary>
        /// Extension method to execute default NSaga components registration in Autofac Container Builder.
        /// <para>
        /// The default components are: <see cref="JsonNetSerialiser"/> to serialise messages; 
        /// <see cref="InMemorySagaRepository"/> to store saga datas;
        /// <see cref="AutofacSagaFactory"/> to resolve instances of Sagas;
        /// <see cref="SagaMetadata"/> to work as the key component - SagaMediator;
        /// <see cref="MetadataPipelineHook"/> added to the pipeline to preserve metadata about incoming messages.
        /// </para>
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <param name="assemblies">Assemblies to scan for Sagas</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder RegisterNSagaComponents(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            Guard.ArgumentIsNotNull(assemblies, nameof(assemblies));

            builder.RegisterType<AutofacSagaFactory>().As<ISagaFactory>();
            builder.RegisterType<JsonNetSerialiser>().As<IMessageSerialiser>();
            builder.RegisterType<InMemorySagaRepository>().As<ISagaRepository>();
            builder.RegisterType<SagaMediator>().As<ISagaMediator>();
            builder.RegisterType<MetadataPipelineHook>().As<IPipelineHook>();


            var sagatTypesDefinitions = NSagaReflection.GetAllSagasInterfaces(assemblies);
            foreach (var sagatTypesDefinition in sagatTypesDefinitions)
            {
                builder.RegisterType(sagatTypesDefinition.Key).As(sagatTypesDefinition.Value);
            }

            var allSagaTypes = NSagaReflection.GetAllSagaTypes(assemblies);
            builder.RegisterTypes(allSagaTypes.ToArray());

            return builder;
        }


        /// <summary>
        /// Override default ISagaRepository registration with the container.
        /// </summary>
        /// <typeparam name="TSagaRepository">Type of new ISagaRepository implementation</typeparam>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder UseSagaRepository<TSagaRepository>(this ContainerBuilder builder) where TSagaRepository : ISagaRepository
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TSagaRepository>().As<ISagaRepository>();
            return builder;
        }


        /// <summary>
        /// Override default ISagaRepository registration with the container.
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <param name="factory">Function that generates an instance of ISagaRepository - to override default registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder UseSagaRepository(this ContainerBuilder builder, Func<IComponentContext, ISagaRepository> factory)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<ISagaRepository>();
            return builder;
        }


        /// <summary>
        /// This chain is aiding with registering <see cref="SqlSagaRepository"/> as the storage mechanism. 
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns><see cref="SqlRepositoryBuilder"/> to aid with constructing connection string to the database</returns>
        public static SqlRepositoryBuilder UseSqlServer(this ContainerBuilder builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            return new SqlRepositoryBuilder(builder);
        }

        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <typeparam name="TPipelineHook">Type of hook to insert into the pipeline</typeparam>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder AddSagaPipelineHook<TPipelineHook>(this ContainerBuilder builder) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TPipelineHook>().As<IPipelineHook>();
            return builder;
        }


        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <param name="factory">Function that produces an instance of <see cref="IPipelineHook"/> that should be inserted into the pipeline</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder AddSagaPipelineHook(this ContainerBuilder builder, Func<IComponentContext, IPipelineHook> factory) 
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<IPipelineHook>();
            return builder;
        }

        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>. By default it is <see cref="JsonNetSerialiser"/>
        /// </summary>
        /// <typeparam name="TMessageSerialiser">Type of message serialiser to be injected</typeparam>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder UseMessageSerialiser<TMessageSerialiser>(this ContainerBuilder builder) where TMessageSerialiser : IMessageSerialiser
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TMessageSerialiser>().As<ISagaRepository>();
            return builder;
        }

        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>. By default it is <see cref="JsonNetSerialiser"/>
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <param name="factory">Fuction that generates an instance of IMessageSerialiser</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static ContainerBuilder UseMessageSerialiser(this ContainerBuilder builder, Func<IComponentContext, IMessageSerialiser> factory)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<IMessageSerialiser>();
            return builder;
        }
    }
}
