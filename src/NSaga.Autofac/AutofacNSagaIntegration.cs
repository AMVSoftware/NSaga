using System;
using System.Linq;
using System.Reflection;
using Autofac;


namespace NSaga.Autofac
{
    public static class AutofacNSagaIntegration
    {
        public static ContainerBuilder RegisterNSagaComponents(this ContainerBuilder builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));
            return builder.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }


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

        public static ContainerBuilder UseSagaRepository<TSagaRepository>(this ContainerBuilder builder) where TSagaRepository : ISagaRepository
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TSagaRepository>().As<ISagaRepository>();
            return builder;
        }

        public static ContainerBuilder UseSagaRepository(this ContainerBuilder builder, Func<IComponentContext, ISagaRepository> factory)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<ISagaRepository>();
            return builder;
        }


        public static SqlRepositoryBuilder UseSqlServer(this ContainerBuilder builder)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            return new SqlRepositoryBuilder(builder);
        }


        public static ContainerBuilder AddSagaPipelineHook<TPipelineHook>(this ContainerBuilder builder) where TPipelineHook : IPipelineHook
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TPipelineHook>().As<IPipelineHook>();
            return builder;
        }

        public static ContainerBuilder AddSagaPipelineHook(this ContainerBuilder builder, Func<IComponentContext, IPipelineHook> factory) 
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<IPipelineHook>();
            return builder;
        }


        public static ContainerBuilder UseMessageSerialiser<TMessageSerialiser>(this ContainerBuilder builder) where TMessageSerialiser : IMessageSerialiser
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.RegisterType<TMessageSerialiser>().As<ISagaRepository>();
            return builder;
        }


        public static ContainerBuilder UseMessageSerialiser(this ContainerBuilder builder, Func<IComponentContext, IMessageSerialiser> factory)
        {
            Guard.ArgumentIsNotNull(builder, nameof(builder));

            builder.Register(factory).As<IMessageSerialiser>();
            return builder;
        }
    }
}
