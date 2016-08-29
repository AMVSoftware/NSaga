using System;
using NSaga;

namespace Tests.Stubs
{
    public class StubSagaServiceLocator : IServiceLocator
    {
        public T Resolve<T>()
        {
            if (typeof(T) == typeof(MySaga))
            {
                return (dynamic)new MySaga(); // <-- dynamic? wtf? compiles!
            }

            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            if (type == typeof(MySaga))
            {
                return new MySaga();
            }

            throw new NotImplementedException();
        }
    }
}