using System;
using System.Collections.Generic;
using System.Reflection;
using TinyIoC;


namespace NSaga
{
    public class Wireup
    {
        public static SagaMediatorBuilder UseInternalContainer()
        {
            var builder = new SagaMediatorBuilder(AppDomain.CurrentDomain.GetAssemblies(), TinyIoCContainer.Current);

            return builder;
        }

        public static SagaMediatorBuilder UseInternalContainer(IEnumerable<Assembly> assembliesToScan)
        {
            var builder = new SagaMediatorBuilder(assembliesToScan, TinyIoCContainer.Current);

            return builder;
        }

        public static SagaMediatorBuilder UseInternalContainer(IEnumerable<Assembly> assembliesToScan, TinyIoCContainer container)
        {
            var builder = new SagaMediatorBuilder(assembliesToScan, container);

            return builder;
        }


        public static SagaMediatorBuilder UseInternalContainer(TinyIoCContainer container)
        {
            var builder = new SagaMediatorBuilder(AppDomain.CurrentDomain.GetAssemblies(), container);

            return builder;
        }

    }
}
