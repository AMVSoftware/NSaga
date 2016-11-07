using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TinyIoC;

namespace NSaga.Composition
{
    public static class TinyIocExtensions
    {
        public static void RegisterSagas(this TinyIoCContainer container, Assembly assemblyToScan)
        {
            container.RegisterSagas(new List<Assembly>() { assemblyToScan });
        }

        public static void RegisterSagas(this TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            var sagaInterfaces = new List<Type>() { typeof(ISaga<>), typeof(InitiatedBy<>), typeof(ConsumerOf<>) };

            var allSagaTypes = assembliesToScan.SelectMany(a => a.GetTypes())
                                .Where(t => Reflection.TypeImplementsInterface(t, typeof(ISaga<>)))
                                .ToList();

            foreach (var sagaType in allSagaTypes)
            {
                var interfaces = sagaType.GetInterfaces()
                                         .Where(i => i.IsGenericType && sagaInterfaces.Contains(i.GetGenericTypeDefinition()))
                                         .ToList();

                foreach (var @interface in interfaces)
                {
                    container.Register(@interface, sagaType);
                }
            }
        }
    }
}
