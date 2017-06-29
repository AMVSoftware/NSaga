using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NSaga
{
    /// <summary>
    /// Provides some useful shortcuts when working with reflaction.
    /// </summary>
    public static class NSagaReflection
    {
        internal static void Set(object instance, string propertyName, object value)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null || property.CanWrite == false)
            {
                return;
            }
            property.SetValue(instance, value, null);
        }


        internal static object Get(object instance, string propertyName)
        {
            var type = instance.GetType();
            var property = type.GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }

            return property.GetValue(instance, null);
        }

        internal static Type GetInterfaceGenericType(object instance, Type interfaceType)
        {
            var instanceType = instance.GetType();

            return GetInterfaceGenericType(interfaceType, instanceType);
        }


        internal static Type GetInterfaceGenericType<TInstance>(Type interfaceType)
        {
            var instanceType = typeof(TInstance);

            return GetInterfaceGenericType(interfaceType, instanceType);
        }


        private static Type GetInterfaceGenericType(Type interfaceType, Type instanceType)
        {
            var genericInterface = instanceType.GetInterface(interfaceType.Name);

            if (!genericInterface.IsGenericType)
            {
                return null;
            }

            var genericParameter = genericInterface.GetGenericArguments().FirstOrDefault();

            return genericParameter;
        }


        /// <summary>
        /// Return all saga types that are initiated by this type of message
        /// </summary>
        /// <param name="message">Initialisation message to check for</param>
        /// <param name="assemblies">Assemblies to scan for sagas</param>
        /// <returns></returns>
        internal static IEnumerable<Type> GetSagaTypesInitiatedBy(IInitiatingSagaMessage message, params Assembly[] assemblies)
        {
            try
            {
                if (assemblies.Length == 0)
                {
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }

                var messageType = message.GetType();
                var initiatingInterfaceType = typeof(InitiatedBy<>).MakeGenericType(messageType);

                var scan = assemblies.SelectMany(a => a.GetTypes())
                                     .Where(t => initiatingInterfaceType.IsAssignableFrom(t))
                                     .ToList();

                return scan;
            }
            catch (ReflectionTypeLoadException ex)
            {
                //Display or log the error based on your application.

                throw new Exception(BuildFusionException(ex));
            }

        }


        /// <summary>
        /// Return all saga types that consume this type of message
        /// </summary>
        /// <param name="message">Initialisation message to check for</param>
        /// <param name="assemblies">Assemblies to scan for sagas</param>
        /// <returns></returns>
        internal static IEnumerable<Type> GetSagaTypesConsuming(ISagaMessage message, params Assembly[] assemblies)
        {
            try
            {
                if (assemblies.Length == 0)
                {
                    assemblies = AppDomain.CurrentDomain.GetAssemblies();
                }

                var messageType = message.GetType();
                var initiatingInterfaceType = typeof(ConsumerOf<>).MakeGenericType(messageType);

                var scan = assemblies.SelectMany(a => a.GetTypes())
                                     .Where(t => initiatingInterfaceType.IsAssignableFrom(t))
                                     .ToList();

                return scan;
            }
            catch (ReflectionTypeLoadException ex)
            {
                //Display or log the error based on your application.

                throw new Exception(BuildFusionException(ex));
            }
            
        }


        internal static object InvokeGenericMethod(object invocationTarget, string methodName, Type genericParameterType, params object[] parameters)
        {
            var invocationTargetType = invocationTarget.GetType();
            var methodInfo = invocationTargetType.GetMethod(methodName);
            var genericMethod = methodInfo.MakeGenericMethod(genericParameterType);
            return genericMethod.Invoke(invocationTarget, parameters);
        }


        internal static object InvokeMethod(object invocationTarget, string methodName, object parameter)
        {
            var invocationTargetType = invocationTarget.GetType();
            var methodInfo = invocationTargetType.GetMethods()
                                                 .Where(m => m.Name == methodName)
                                                 .FirstOrDefault(m => m.GetParameters().First().ParameterType == parameter.GetType());
            if (methodInfo == null)
            {
                throw new ArgumentException($"Unable to find method {methodName} with parameter type {parameter.GetType().Name}");
            }

            return methodInfo.Invoke(invocationTarget, new []{ parameter });
        }


        /// <summary>
        /// Checks if provided type implements a given interface. Also works with open generics. It will tell you if MyClass implements MyInterface
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        private static bool TypeImplementsInterface(Type type, Type @interface)
        {
            if (@interface.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == @interface))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Returns a list of all the Saga classes in the provided assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan</param>
        /// <returns>List of Types that implement <see cref="ISaga{TSagaData}"/></returns>
        public static IEnumerable<Type> GetAllSagaTypes(IEnumerable<Assembly> assemblies)
        {
            try
            {
                var types = assemblies.SelectMany(a => a.GetTypes()).ToList();

                var allSagaTypes = types
                                .Where(t => NSagaReflection.TypeImplementsInterface(t, typeof(ISaga<>)))
                                .Where(t => t.IsClass)
                                .ToList();

                return allSagaTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                //Display or log the error based on your application.

                throw new Exception(BuildFusionException(ex));
            }
            
        }

        /// <summary>
        /// Returns list of all pairs of SagaType -> implemented interface
        /// 
        /// Saga Types will appear here multiple type - once for every interface related to NSaga (<see cref="ISaga{TSagaData}"/>, <see cref="InitiatedBy{TMsg}"/>, <see cref="ConsumerOf{TMsg}"/>)
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

        /// <summary>
        /// Provides a details on the reflection exceptions
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string BuildFusionException(ReflectionTypeLoadException ex)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Exception exSub in ex.LoaderExceptions)
            {
                sb.AppendLine(exSub.Message);
                FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                if (exFileNotFound != null)
                {
                    if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
