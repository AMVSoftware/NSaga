using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSaga
{
    public static class Reflection
    {
        public static void Set(object instance, string name, object value)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(name);
            if (property == null || property.CanWrite == false)
            {
                return;
            }
            property.SetValue(instance, value, null);
        }


        public static object Get(object instance, string name)
        {
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty(name);
            if (property == null)
            {
                return null;
            }

            return property.GetValue(instance, null);
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


        public static Type GetInterfaceGenericType(Type interfaceType, Type instanceType)
        {
            var genericInterface = instanceType.GetInterface(interfaceType.Name);

            if (!genericInterface.IsGenericType)
            {
                return null;
            }

            var genericParameter = genericInterface.GetGenericArguments().FirstOrDefault();

            return genericParameter;
        }
    }
}
