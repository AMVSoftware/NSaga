using System;
using NSaga;
using NSaga.AzureTables;

namespace Samples
{
    public class AzureTableStorageSample
    {
        private ISagaMediator sagaMediator;
        private ISagaRepository sagaRepository;

        public void Run()
        {
            // You need Azure Storage emulator running to run this
            var builder = Wireup.UseInternalContainer()
                                .UseRepository<AzureTablesSagaRepository>()
                                .Register(typeof(ITableClientFactory), new TableClientFactory("UseDevelopmentStorage=true"));

            sagaMediator = builder.ResolveMediator();

            sagaRepository = builder.ResolveRepository();

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
