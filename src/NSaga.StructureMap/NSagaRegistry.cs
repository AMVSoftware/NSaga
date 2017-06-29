using StructureMap;

namespace NSaga.StructureMap
{
    public class NSagaRegistry : Registry
    {
        public NSagaRegistry()
        {           
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
                
                x.ConnectImplementationsToTypesClosing(typeof(ISaga<>));
                x.ConnectImplementationsToTypesClosing(typeof(ConsumerOf<>));
                x.ConnectImplementationsToTypesClosing(typeof(InitiatedBy<>));
            });

            For<ISagaFactory>().Singleton().Use<StructureMapSagaFactory>();
            For<IMessageSerialiser>().Use<JsonNetSerialiser>();
            For<ISagaRepository>().Use<InMemorySagaRepository>();
            For<ISagaMediator>().Use<SagaMediator>();
            For<IPipelineHook>().Use<MetadataPipelineHook>();
            For(typeof(ConsumerOf<>));
            For(typeof(InitiatedBy<>));
        }
    }
}
