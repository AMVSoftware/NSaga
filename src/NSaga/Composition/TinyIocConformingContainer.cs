using System;
using System.Collections.Generic;
using TinyIoC;

namespace NSaga
{
    public class TinyIocConformingContainer : IConformingContainer
    {
        private readonly TinyIoCContainer container;

        public TinyIocConformingContainer(TinyIoCContainer container)
        {
            this.container = container;
        }

        public TinyIocConformingContainer()
        {
            this.container = TinyIoCContainer.Current;
        }

        public T Resolve<T>() where T : class
        {
            return container.Resolve<T>();
        }

        public void Register(Type registerType, object instance)
        {
            container.Register(registerType, instance);
        }

        public void Register(Type registerType, Type implementation)
        {
            container.Register(registerType, implementation);
        }

        public void RegisterMultiple(Type registerType, IEnumerable<Type> implementations)
        {
            container.RegisterMultiple(registerType, implementations);
        }
    }
}