using System;
using NSaga;
using NSaga.Pipeline;


namespace Samples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var simpleInjectorSample = new SimpleInjectorSample();

            simpleInjectorSample.RunSample();
        }
    }
}
