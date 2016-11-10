using TinyIoC;


namespace NSaga
{
    public class Wireup
    {
        public static SagaMediatorBuilder UseInternalContainer()
        {
            var builder = new SagaMediatorBuilder(TinyIoCContainer.Current);

            return builder;
        }

        public static SagaMediatorBuilder UseInternalContainer(TinyIoCContainer container)
        {
            var builder = new SagaMediatorBuilder(container);

            return builder;
        }
    }
}
