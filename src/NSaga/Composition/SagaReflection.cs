using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    public static class SagaReflection
    {
        public static IEnumerable<Type> GetAllSagaTypes(IEnumerable<Assembly> assemblies)
        {
            var allSagaTypes = assemblies.SelectMany(a => a.GetTypes())
                                .Where(t => Reflection.TypeImplementsInterface(t, typeof(ISaga<>)))
                                .ToList();

            return allSagaTypes;
        }


        /// <summary>
        /// Returns list of all pairs of SagaType -> implemented interface
        /// 
        /// Saga Types will appear here multiple type - once for every interface related to NSaga (ISaga<>, InitiatedBy<>, ConsumedBy<>)
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<Type, Type>> GetAllSagasInterfaces(IEnumerable<Assembly> assemblies)
        {
            var sagaInterfaces = new List<Type>() { typeof(ISaga<>), typeof(InitiatedBy<>), typeof(ConsumerOf<>) };

            var allSagaTypes = GetAllSagaTypes(assemblies);

            foreach (var sagaType in allSagaTypes)
            {
                var interfaces = sagaType.GetInterfaces()
                                         .Where(i => i.IsGenericType && sagaInterfaces.Contains(i.GetGenericTypeDefinition()))
                                         .ToList();

                foreach (var @interface in interfaces)
                {
                    yield return new KeyValuePair<Type, Type>(sagaType, @interface);
                }
            }
        }
    }
}
