using System;
using Autofac;

namespace NSaga.Autofac
{
    public class AutofacSagaFactory : ISagaFactory
    {
        private readonly IContainer container;

        public AutofacSagaFactory(IContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>() where T : class
        {
            return container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return container.Resolve(type);
        }
    }
}
