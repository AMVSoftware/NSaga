using System;
using System.Linq;
using FluentAssertions;
using NSaga;
using NSaga.Composition;
using NSaga.Pipeline;
using TinyIoC;
using Xunit;


namespace Tests.PipelineHook
{
    public class MetadataPipelineIntegrationTests
    {
        private readonly DateTime initialisationTime;
        private readonly SagaMetadata sagaMetadata;
        private readonly Guid correlationId;


        public MetadataPipelineIntegrationTests()
        {
            initialisationTime = new DateTime(1905, 9, 13);
            TimeProvider.Current = new StubTimeProvider(initialisationTime);

            var container = TinyIoCContainer.Current;
            container.RegisterSagas(typeof(MetadataPipelineIntegrationTests).Assembly);
            var serviceLocator = new TinyIocSagaFactory(container);


            var jsonNetSerialiser = new JsonNetSerialiser();
            var sagaRepository = new InMemorySagaRepository(jsonNetSerialiser, serviceLocator);
            var hooks = new IPipelineHook[] { new MetadataPipelineHook(jsonNetSerialiser) };

            var sagaMediator = new SagaMediator(sagaRepository, serviceLocator, hooks);

            correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaInitiatingMessage(correlationId);
            sagaMediator.Consume(initiatingMessage);
            var saga = sagaRepository.Find<MySaga>(correlationId);
            sagaMetadata = saga.GetSagaMetadata(new JsonNetSerialiser());
        }

        [Fact]
        public void Initialise_Creates_MetadataRecord()
        {
            sagaMetadata.Should().NotBeNull();
        }

        [Fact]
        public void AfterConsuming_DateCreated_SetToday()
        {
            sagaMetadata.DateCreated.Should().Be(initialisationTime);
        }

        [Fact]
        public void AfterConsuming_DateLastModified_SetToday()
        {
            sagaMetadata.DateLastModified.Should().Be(initialisationTime);
        }

        [Fact]
        public void AfterConsuming_ReceivedMessages_ContainsMessage()
        {
            sagaMetadata.ReceivedMessages.Should().HaveCount(1);
        }

        [Fact]
        public void AfterConsuming_ReceivedMessages_MatchesType()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            receivedMessage.SagaMessage.Should().BeAssignableTo<MySagaInitiatingMessage>();
        }


        [Fact]
        public void AfterConsuming_ReceivedMessage_CorrelationId()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            receivedMessage.SagaMessage.CorrelationId.Should().Be(correlationId);
        }
    }
}
