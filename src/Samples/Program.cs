using System;

namespace Samples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            //var internalContainerSample = new InternalContainerSample();
            //internalContainerSample.Run();
            //Console.WriteLine("Internal Container Demo is finished. Press Any Key to continue");
            //Console.ReadKey();


            //var simpleInjectorSample = new SimpleInjectorSample();
            //simpleInjectorSample.RunSample();
            //Console.WriteLine("Simple Injector Demo is finished. Press Any Key to continue");
            //Console.ReadKey();


            //var staticBuilderReference = new StaticBuilderReference();
            //staticBuilderReference.Run();
            //Console.WriteLine("Static Builder reference Demo is finished. Press Any Key to continue");
            //Console.ReadKey();

            var azureTablesSample = new AzureTableStorageSample();
            azureTablesSample.Run();
            Console.WriteLine("Azure Tables persistence demo is finished. Press Any Key to continue");
            Console.ReadKey();
        }
    }
}
