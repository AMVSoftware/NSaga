using System;
using System.Collections.Generic;
using System.Reflection;
using TinyIoC;


namespace NSaga
{
    public class Wireup
    {
        public static InternalMediatorBuilder UseInternalContainer()
        {
            var builder = new InternalMediatorBuilder(TinyIoCContainer.Current, AppDomain.CurrentDomain.GetAssemblies());

            return builder;
        }

        public static InternalMediatorBuilder UseInternalContainer(IEnumerable<Assembly> assembliesToScan)
        {
            var builder = new InternalMediatorBuilder(TinyIoCContainer.Current, assembliesToScan);

            return builder;
        }

        public static InternalMediatorBuilder UseInternalContainer(TinyIoCContainer container, IEnumerable<Assembly> assembliesToScan)
        {
            var builder = new InternalMediatorBuilder(container, assembliesToScan);

            return builder;
        }
        
        //public static Wireup Init()
        //{
        //    return new Wireup();
        //}
    }
}
