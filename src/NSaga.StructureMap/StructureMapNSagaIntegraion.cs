using System;
using System.Linq;
using System.Reflection;
using StructureMap;

namespace NSaga.StructureMap
{
    /// <summary>
    /// Adapter to integrate NSaga with StructureMap DI container
    /// </summary>
    public static class StructureMapNSagaIntegraion
    {
        /// <summary>
        /// Constructor to create <see cref="StructureMapNSagaIntegraion"/> adapter
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static Container RegisterNSagaComponents(this Container builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            return builder.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Extension method to execute default NSaga components registration in StructureMap Container Builder.
        /// <para>
        /// Default registrations are:
        /// <list type="bullet">
        /// <item><description><see cref="JsonNetSerialiser"/> to serialise messages; </description></item> 
        /// <item><description><see cref="InMemorySagaRepository"/> to store saga datas; </description></item> 
        /// <item><description><see cref="StructureMapSagaFactory"/> to resolve instances of Sagas;</description></item> 
        /// <item><description><see cref="SagaMetadata"/> to work as the key component - SagaMediator;</description></item> 
        /// <item><description><see cref="MetadataPipelineHook"/> added to the pipeline to preserve metadata about incoming messages.</description></item> 
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <param name="assemblies">Assemblies to scan for Sagas</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static Container RegisterNSagaComponents(this Container builder, params Assembly[] assemblies)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            Guard.ArgumentIsNotNull(assemblies, nameof(assemblies));

            builder.Configure(x => {
                x.AddRegistry<NSagaRegistry>();

                var sagatTypesDefinitions = NSagaReflection.GetAllSagasInterfaces(assemblies);
                foreach (var sagatTypesDefinition in sagatTypesDefinitions)
                {
                    x.AddType(sagatTypesDefinition.Value, sagatTypesDefinition.Key.UnderlyingSystemType);
                }

                var allSagaTypes = NSagaReflection.GetAllSagaTypes(assemblies).ToList();
                allSagaTypes.ToList().ForEach(t => { x.For(t); });

            });
                        
            return builder;
        }

        /// <summary>
        /// Override default ISagaRepository registration with the container.
        /// </summary>
        /// <typeparam name="TSagaRepository">Type of new ISagaRepository implementation</typeparam>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static Container UseSagaRepository<TSagaRepository>(this Container builder) where TSagaRepository : ISagaRepository
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            builder.Configure(x =>
            {
                x.For<ISagaRepository>().Use<TSagaRepository>();
            });

            return builder;
        }

        /// <summary>
        /// This chain is aiding with registering <see cref="SqlSagaRepository"/> as the storage mechanism. 
        /// </summary>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns><see cref="SqlRepositoryBuilder"/> to aid with constructing connection string to the database</returns>
        public static SqlRepositoryBuilder UseSqlServer(this Container builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            return new SqlRepositoryBuilder(builder);
        }
        
        /// <summary>
        /// Adds another pipeline hook into the pipeline. <see cref="IPipelineHook"/> for description of possible interception points.
        /// </summary>
        /// <typeparam name="TPipelineHook">Type of hook to insert into the pipeline</typeparam>
        /// <param name="builder">Container to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static Container AddSagaPipelineHook<TPipelineHook>(this Container builder) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Configure(x=>
            {
                x.For<IPipelineHook>().Use<TPipelineHook>();
            });
            return builder;
        }

        /// <summary>
        /// Replaces the default implementation of <see cref="IMessageSerialiser"/>. By default it is <see cref="JsonNetSerialiser"/>
        /// </summary>
        /// <typeparam name="TMessageSerialiser">Type of message serialiser to be injected</typeparam>
        /// <param name="builder">Container Builder to do the registration</param>
        /// <returns>The same container builder so the calls can be chained in Builder-fashion</returns>
        public static Container UseMessageSerialiser<TMessageSerialiser>(this Container builder) where TMessageSerialiser : IMessageSerialiser
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            builder.Configure(x =>
            {
                x.For<IMessageSerialiser>().Use<TMessageSerialiser>();
            });
            return builder;
        }
    }
}
