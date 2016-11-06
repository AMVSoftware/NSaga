using System;
using TinyIoC;


namespace NSaga
{
    public class TinyIocSagaFactory : ISagaFactory
    {
        private readonly TinyIoCContainer container;

        public TinyIocSagaFactory(TinyIoCContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>() where T : class
        {
            var result = container.Resolve<T>();

            return result;
        }


        public object Resolve(Type type)
        {
            var result = container.Resolve(type);

            return result;
        }
    }
}
