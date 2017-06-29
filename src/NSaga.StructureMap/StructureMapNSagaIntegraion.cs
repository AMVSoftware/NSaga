using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace NSaga.StructureMap
{
    public static class StructureMapNSagaIntegraion
    {
        public static Container RegisterNSagaComponents(this Container builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            return builder.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static Container RegisterNSagaComponents(this Container builder, params Assembly[] assemblies)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            Guard.ArgumentIsNotNull(assemblies, nameof(assemblies));

            builder.Configure(x => {
                x.AddRegistry<NSagaRegistry>();

                x.For<ISagaFactory>().Use<StructureMapSagaFactory>().Singleton();
                x.For<IMessageSerialiser>().Use<JsonNetSerialiser>();
                x.For<ISagaRepository>().Use<InMemorySagaRepository>();
                x.For<ISagaMediator>().Use<SagaMediator>();
                x.For<IPipelineHook>().Use<MetadataPipelineHook>();
                x.For<ConsumerOf<ISagaMessage>>();

                var sagatTypesDefinitions = NSagaReflection.GetAllSagasInterfaces(assemblies);
                foreach (var sagatTypesDefinition in sagatTypesDefinitions)
                {
                    x.For(sagatTypesDefinition.Value).Use(sagatTypesDefinition.Key);
                }
            });

            //builder.Configure(x =>
            //    {
            //        x.AssemblyContainingType<ISagaMediator>();
            //        x.For<ISagaFactory>().Use<StructureMapSagaFactory>().Singleton();
            //        x.For<IMessageSerialiser>().Use<JsonNetSerialiser>();
            //        x.For<ISagaRepository>().Use<InMemorySagaRepository>();
            //        x.For<ISagaMediator>().Use<SagaMediator>();
            //        x.For<IPipelineHook>().Use<MetadataPipelineHook>();

            //        var sagatTypesDefinitions = NSagaReflection.GetAllSagasInterfaces(assemblies);
            //        foreach (var sagatTypesDefinition in sagatTypesDefinitions)
            //        {
            //            x.For(sagatTypesDefinition.Value).Use(sagatTypesDefinition.Key);
            //        }

            //        var allSagaTypes = NSagaReflection.GetAllSagaTypes(assemblies);
            //        TODO: add the array to the registry
            //    });

            return builder;
        }

        public static Container UseSagaRepository<TSagaRepository>(this Container builder) where TSagaRepository : ISagaRepository
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            builder.Configure(x =>
            {
                x.For<ISagaRepository>().Use<TSagaRepository>();
            });

            return builder;
        }

        public static SqlRepositoryBuilder UseSqlServer(this Container builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            return new SqlRepositoryBuilder(builder);
        }

        public static Container AddSagaPipelineHook<TPipelineHook>(this Container builder) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Configure(x=>
            {
                x.For<IPipelineHook>().Use<TPipelineHook>();
            });
            return builder;
        }

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
