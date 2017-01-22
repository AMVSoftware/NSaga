using System;
using NSaga;
using NSaga.AzureTables;
using NSaga.SimpleInjector;
using SimpleInjector;

namespace Samples
{
    public class AzureTableStorageSample
    {
        private ISagaMediator sagaMediator;

        public void Run()
        {
            // You need Azure Storage emulator running to run this
            var connectionString = "UseDevelopmentStorage=true";
            var builder = Wireup.UseInternalContainer()
                                .UseRepository<AzureTablesSagaRepository>()
                                .Register(typeof(ITableClientFactory), new TableClientFactory(connectionString));

            sagaMediator = builder.ResolveMediator();

            var correlationId = Guid.NewGuid();
            var initMessage = new PersonalDetailsVerification(correlationId)
            {
                FirstName = "James",
                LastName = "Bond",
            };

            sagaMediator.Consume(initMessage);
        }


        public void WithSimpleInjector()
        {
            var container = new Container();
            container.RegisterNSagaComponents();
            container.UseSagaRepository<AzureTablesSagaRepository>();

            // You need Azure Storage emulator running to run this
            var connectionString = "UseDevelopmentStorage=true";
            container.Register<ITableClientFactory>(()=> new TableClientFactory(connectionString), Lifestyle.Singleton);

            sagaMediator = container.GetInstance<ISagaMediator>();

            var correlationId = Guid.NewGuid();
            var initMessage = new PersonalDetailsVerification(correlationId)
            {
                FirstName = "James",
                LastName = "Bond",
            };

            sagaMediator.Consume(initMessage);
        }
    }
}
