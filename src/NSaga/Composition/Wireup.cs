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
    }
}
