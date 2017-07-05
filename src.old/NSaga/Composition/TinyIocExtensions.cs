using System.Collections.Generic;
using System.Reflection;

namespace NSaga
{
    internal static class TinyIocExtensions
    {
        internal static void RegisterSagas(this TinyIoCContainer container, Assembly assemblyToScan)
        {
            container.RegisterSagas(new List<Assembly>() { assemblyToScan });
        }


        internal static void RegisterSagas(this TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            var sagaTypePairs = NSagaReflection.GetAllSagasInterfaces(assembliesToScan);
            foreach (var sagaTypePair in sagaTypePairs)
            {
                container.Register(sagaTypePair.Value, sagaTypePair.Key);
            }
        }
    }
}
