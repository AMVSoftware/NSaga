using System;
using System.Linq;
using FluentAssertions;
using NSaga;
using Xunit;


namespace Tests.PipelineHook
{
    public class MetadataPipelineHookConsumingTests : IDisposable
    {
        private readonly MetadataPipelineHook sut;
        private readonly MySaga saga;
        private readonly DateTime initialisationTime;
        private readonly DateTime consumingTime;
        private readonly SagaMetadata sagaMetadata;
        private readonly Guid correlationId;
        private readonly MySagaConsumingMessage consumingMessage;

        public MetadataPipelineHookConsumingTests()
        {
            initialisationTime = new DateTime(1905, 9, 13);
            TimeProvider.Current = new StubTimeProvider(initialisationTime);

            sut = new MetadataPipelineHook(new JsonNetSerialiser());
            correlationId = Guid.NewGuid();
            var initiatingMessage = new MySagaInitiatingMessage(correlationId)
            {
                SomeRandomValue = Guid.NewGuid(),
            };

            saga = new MySaga();
            var context = new PipelineContext(initiatingMessage, saga);
            sut.AfterInitialisation(context);

            // change time so we can compare dates
            consumingTime = new DateTime(1955, 7, 13);
            TimeProvider.Current = new StubTimeProvider(consumingTime);

            // and throw in another message
            consumingMessage = new MySagaConsumingMessage(correlationId) { SomeRandomValue = Guid.NewGuid().ToString() };
            sut.AfterConsuming(new PipelineContext(consumingMessage, saga));

            sagaMetadata = saga.GetSagaMetadata(new JsonNetSerialiser());
        }

        [Fact]
        public void AfterConsuming_Key_Set()
        {
            saga.Headers.Should().ContainKey(MetadataPipelineHook.MetadataKeyName);
        }

        [Fact]
        public void AfterConsuming_DateCreated_SetToday()
        {
            sagaMetadata.DateCreated.Should().Be(initialisationTime);
        }

        [Fact]
        public void AfterConsuming_DateLastModified_SetToday()
        {
            sagaMetadata.DateLastModified.Should().Be(consumingTime);
        }


        [Fact]
        public void AfterConsuming_ReceivedMessages_ContainsMessage()
        {
            sagaMetadata.ReceivedMessages.Should().HaveCount(2);
        }

        [Fact]
        public void AfterConsuming_ReceivedMessages_MatchesType()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.Skip(1).First();

            receivedMessage.SagaMessage.Should().BeAssignableTo<MySagaConsumingMessage>();
        }


        [Fact]
        public void AfterConsuming_ReceivedMessage_Timestamp()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.Skip(1).First();

            receivedMessage.Timestamp.Should().Be(consumingTime);
        }

        [Fact]
        public void AfterConsuming_ReceivedMessage_CorrelationId()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.Skip(1).First();

            receivedMessage.SagaMessage.CorrelationId.Should().Be(correlationId);
        }

        [Fact]
        public void AfterConsuming_ReceivedMessage_MessageData()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.Skip(1).First();

            var sagaMessage = (MySagaConsumingMessage)receivedMessage.SagaMessage;
            sagaMessage.SomeRandomValue.Should().Be(consumingMessage.SomeRandomValue);
        }

        public void Dispose()
        {
            TimeProvider.ResetToDefault();
        }
    }
}
