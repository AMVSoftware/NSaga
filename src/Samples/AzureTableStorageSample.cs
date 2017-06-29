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
            try
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
            catch (Exception)
            {
                Console.WriteLine("Quite likely you don't have Azure Storage Emulator running, so this sample can't be executed");
            }
        }


        public void WithSimpleInjector()
        {
            try
            {
                var container = new Container();
                container.RegisterNSagaComponents();
                container.UseSagaRepository<AzureTablesSagaRepository>();

                // You need Azure Storage emulator running to run this
                var connectionString = "UseDevelopmentStorage=true";
                container.Register<ITableClientFactory>(() => new TableClientFactory(connectionString), Lifestyle.Singleton);

                sagaMediator = container.GetInstance<ISagaMediator>();

                var correlationId = Guid.NewGuid();
                var initMessage = new PersonalDetailsVerification(correlationId)
                {
                    FirstName = "James",
                    LastName = "Bond",
                };

                sagaMediator.Consume(initMessage);
            }
            catch (Exception)
            {
                Console.WriteLine("Quite likely you don't have Azure Storage Emulator running, so this sample can't be executed");
            }
        }
    }
}
