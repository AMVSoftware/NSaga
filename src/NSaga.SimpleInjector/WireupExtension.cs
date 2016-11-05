//using SimpleInjector;

//namespace NSaga.SimpleInjector
//{
//    public static class WireupExtension
//    {
//        public static SagaMediatorBuilder UseSimpleInjector(this Wireup wireup, Container simpleInjectorContainer)
//        {
//            var conformingContainer = new SimpleConformingContainer(simpleInjectorContainer);

//            var mediatorBuilder = new SagaMediatorBuilder(conformingContainer);
//            mediatorBuilder.UseSagaFactory<SimpleInjectorSagaFactory>();

//            return mediatorBuilder;
//        }
//    }
//}
