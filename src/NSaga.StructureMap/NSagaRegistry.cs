using StructureMap;

namespace NSaga.StructureMap
{
    public class NSagaRegistry : Registry
    {
        public NSagaRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ISagaMediator>();
                x.AssemblyContainingType<ISagaMessage>();
                x.TheCallingAssembly();
                x.WithDefaultConventions();

            });
        }
    }
}
