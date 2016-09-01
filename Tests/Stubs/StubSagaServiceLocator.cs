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

            if (typeof(T) == typeof(SagaWithErrors))
            {
                return (dynamic)new SagaWithErrors(); 
            }

            throw new ArgumentException("Unable to resolve saga of this type");
        }


        public object Resolve(Type type)
        {
            if (type == typeof(MySaga))
            {
                return new MySaga();
            }

            if (type == typeof(SagaWithErrors))
            {
                return new SagaWithErrors();
            }

            throw new ArgumentException("Unable to resolve saga of this type");
        }
    }
}