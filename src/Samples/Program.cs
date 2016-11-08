namespace Samples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var internalSample = new InternalSample();
            internalSample.Run();

            //var simpleInjectorSample = new SimpleInjectorSample();
            //simpleInjectorSample.RunSample();
        }
    }
}
