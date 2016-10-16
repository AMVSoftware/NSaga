using System;
using TinyIoC;


namespace NSaga
{
    public class TinyIocServiceLocator : IServiceLocator
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
}
