using System;
using NSaga;

namespace Tests
{
    /// <summary>
    /// This is not meant to be used in production, only for demonstration purposes. 
    /// Please replace service locator with a real DI service locator
    /// </summary>
    public class DumbSagaFactory : ISagaFactory
    {
        public T ResolveSaga<T>() where T : class, IAccessibleSaga
        {
            if (typeof(T).IsInterface)
            {
                throw new ArgumentException("Unable to create interface. Sorry - this is a dumb service locator - use a DI of your choice to do real service resolution");
            }

            return Activator.CreateInstance<T>();
        }


        public IAccessibleSaga ResolveSaga(Type type)
        {
            if (type.IsInterface)
            {
                throw new ArgumentException("Unable to create interface. Sorry - this is a dumb service locator - use a DI of your choice to do real service resolution");
            }

            return (IAccessibleSaga)Activator.CreateInstance(type);
        }

        public IAccessibleSaga ResolveSagaInititatedBy(IInitiatingSagaMessage message)
        {
            throw new NotImplementedException();
        }

        public IAccessibleSaga ResolveSagaConsumedBy(ISagaMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
