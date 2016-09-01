using System;


namespace NSaga.Implementations
{
    /// <summary>
    /// This is not meant to be used in production, only for demonstration purposes. 
    /// Please replace service locator with a real DI service locator
    /// </summary>
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
