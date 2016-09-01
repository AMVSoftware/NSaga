using System;


namespace NSaga.Implementations
{
    public class DumbServiceLocator : IServiceLocator
    {
        public T Resolve<T>()
        {
            if (typeof(T).IsInterface)
            {
                throw new ArgumentException("Unable to create interface. Sorry - this is a dumb service locator - use a DI of your choice to do real service resolution");
            }

            return Activator.CreateInstance<T>();
        }


        public object Resolve(Type type)
        {
            if (type.IsInterface)
            {
                throw new ArgumentException("Unable to create interface. Sorry - this is a dumb service locator - use a DI of your choice to do real service resolution");
            }

            return Activator.CreateInstance(type);
        }
    }
}
