using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;


namespace NSaga.Autofac
{
    public static class AutofacNSagaIntegration
    {
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterNSagaComponents(this ContainerBuilder builder)
        {
            return builder.RegisterNSagaComponents(AppDomain.CurrentDomain.GetAssemblies());
        }


        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterNSagaComponents(this ContainerBuilder builder, params Assembly[] assemblies)
        {
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

            return builder.RegisterTypes(allSagaTypes.ToArray());
        }

        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            UseSagaRepository<TSagaRepository>(this ContainerBuilder builder) where TSagaRepository : ISagaRepository
        {
            throw new NotImplementedException();
            //return builder.RegisterType<TSagaRepository>().As<ISagaRepository>();
        }
    }
}
