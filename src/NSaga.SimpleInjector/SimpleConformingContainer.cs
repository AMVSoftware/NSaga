using System;
using SimpleInjector;

namespace NSaga.SimpleInjector
{
    public class SimpleConformingContainer : IConformingContainer
    {
        private readonly Container container;

        public SimpleConformingContainer(Container container)
        {
            this.container = container;
        }

        public T Resolve<T>() where T : class
        {
            return container.GetInstance<T>();
        }

        public void Register(Type registerType, object instance)
        {
            container.Register(registerType, () => instance);
        }

        public void Register(Type registerType, Type implementation)
        {
            container.Register(registerType, implementation);
        }
    }
}
