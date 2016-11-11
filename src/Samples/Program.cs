namespace Samples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var internalContainerSample = new InternalContainerSample();
            internalContainerSample.Run();

            var simpleInjectorSample = new SimpleInjectorSample();
            simpleInjectorSample.RunSample();
        }
    }
}
