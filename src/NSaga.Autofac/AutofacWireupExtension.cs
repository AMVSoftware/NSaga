using Autofac;

namespace NSaga.Autofac
{
    public static class AutofacWireupExtension
    {
        public static SagaMediatorBuilder UseAutofac(this Wireup wireup, IContainer autofacContainer)
        {
            var conformingContainer = new AutofacConformingContainer(autofacContainer);

            var mediatorBuilder = new SagaMediatorBuilder(conformingContainer);
            mediatorBuilder.UseSagaFactory<AutofacSagaFactory>();

            return mediatorBuilder;
        }
    }
}
