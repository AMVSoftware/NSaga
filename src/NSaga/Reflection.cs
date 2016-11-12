using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace NSaga
{
    /// <summary>
    /// Because of all the generics and the way .Net works with Generics,
    /// we have to use a metric ton of reflection
    /// </summary>
    internal static class Reflection
    {
        public static void Set(object instance, string propertyName, object value)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null || property.CanWrite == false)
            {
                return;
            }
            property.SetValue(instance, value, null);
        }


        public static object Get(object instance, string propertyName)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }

            return property.GetValue(instance, null);
        }


        public static object GetPrivate(object instance, string propertyName)
        {
            Type type = instance.GetType();
            FieldInfo property = type.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                return null;
            }

            return property.GetValue(instance);
        }

        public static Type GetInterfaceGenericType(object instance, Type interfaceType)
        {
            var instanceType = instance.GetType();

            return GetInterfaceGenericType(interfaceType, instanceType);
        }


        public static Type GetInterfaceGenericType<TInstance>(Type interfaceType)
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
        public static IEnumerable<Type> GetSagaTypesInitiatedBy(IInitiatingSagaMessage message, params Assembly[] assemblies)
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


        /// <summary>
        /// Return all saga types that consume this type of message
        /// </summary>
        /// <param name="message">Initialisation message to check for</param>
        /// <param name="assemblies">Assemblies to scan for sagas</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetSagaTypesConsuming(ISagaMessage message, params Assembly[] assemblies)
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


        public static object InvokeGenericMethod(object invocationTarget, string methodName, Type genericParameterType, params object[] parameters)
        {
            var invocationTargetType = invocationTarget.GetType();
            var methodInfo = invocationTargetType.GetMethod(methodName);
            var genericMethod = methodInfo.MakeGenericMethod(genericParameterType);
            return genericMethod.Invoke(invocationTarget, parameters);
        }


        public static object InvokeMethod(object invocationTarget, string methodName, object parameter)
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
        /// Checks if provided type implements a given interface. Also works with open generics. It will tell you if MyClass implements MyInterface<>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interface"></param>
        /// <returns></returns>
        public static bool TypeImplementsInterface(Type type, Type @interface)
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
    }
}
