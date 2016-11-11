using System.Collections.Generic;
using System.Reflection;

namespace NSaga
{
    public static class TinyIocExtensions
    {
        public static void RegisterSagas(this TinyIoCContainer container, Assembly assemblyToScan)
        {
            container.RegisterSagas(new List<Assembly>() { assemblyToScan });
        }


        public static void RegisterSagas(this TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            var sagaTypePairs = SagaReflection.GetAllSagasInterfaces(assembliesToScan);
            foreach (var sagaTypePair in sagaTypePairs)
            {
                container.Register(sagaTypePair.Value, sagaTypePair.Key);
            }
        }
    }
}
