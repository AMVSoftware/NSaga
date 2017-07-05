using System.Reflection;

namespace NSaga
{
    /// <summary>
    /// Class to accommodate start of configuration of NSaga 
    /// </summary>
    public sealed class Wireup
    {
        /// <summary>
        /// Uses the internal DI container to configure all NSaga components
        /// </summary>
        /// <returns>an instance of <see cref="SagaMediatorBuilder"/> that guides configuration</returns>
        public static SagaMediatorBuilder UseInternalContainer()
        {
            var builder = new SagaMediatorBuilder();

            return builder;
        }

        /// <summary>
        /// Uses the internal DI container to configure all NSaga components
        /// </summary>
        /// <param name="assembliesToScan">List of assemblies to scan for registration</param>
        /// <returns>an instance of <see cref="SagaMediatorBuilder"/> that guides configuration</returns>
        public static SagaMediatorBuilder UseInternalContainer(params Assembly[] assembliesToScan)
        {
            var builder = new SagaMediatorBuilder(assembliesToScan);

            return builder;
        }

    }
}
