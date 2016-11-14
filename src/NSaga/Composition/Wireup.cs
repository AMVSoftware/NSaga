namespace NSaga
{
    public sealed class Wireup
    {
        public static SagaMediatorBuilder UseInternalContainer()
        {
            var builder = new SagaMediatorBuilder();

            return builder;
        }
    }
}
