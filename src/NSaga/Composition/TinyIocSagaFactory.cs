using System;
using TinyIoC;


namespace NSaga
{
    public class TinyIocSagaFactory : ISagaFactory
    {
        public T Resolve<T>() where T : class
        {
            var result = TinyIoCContainer.Current.Resolve<T>();

            return result;
        }


        public object Resolve(Type type)
        {
            var result = TinyIoCContainer.Current.Resolve(type);

            return result;
        }
    }

    public class TinyIocConformingContainer : IConformingContainer
    {
        private TinyIoCContainer container;

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
    }
}
