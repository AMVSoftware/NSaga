using System;
using System.Linq;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests.PipelineHook
{
    [Collection("TimeProvider")]
    public class MetadataPipelineHookInitialisationTests : IDisposable
    {
        private readonly MetadataPipelineHook sut;
        private readonly MySaga saga;
        private readonly DateTime currentTime;
        private readonly SagaMetadata sagaMetadata;
        private readonly Guid correlationId;
        private MySagaInitiatingMessage message;

        public MetadataPipelineHookInitialisationTests()
        {
            currentTime = new DateTime(1905, 9, 13);
            TimeProvider.Current = new StubTimeProvider(currentTime);

            sut = new MetadataPipelineHook(new JsonNetSerialiser());
            correlationId = Guid.NewGuid();
            message = new MySagaInitiatingMessage(correlationId)
            {
                SomeRandomValue = Guid.NewGuid(),
            };

            saga = new MySaga();
            var context = new PipelineContext(message,saga);
            sut.AfterInitialisation(context);

            sagaMetadata = saga.GetSagaMetadata(new JsonNetSerialiser());
        }

        [Fact]
        public void AfterIntiation_Key_Set()
        {
            saga.Headers.Should().ContainKey(MetadataPipelineHook.MetadataKeyName);
        }

        [Fact]
        public void AfterInitialisation_DateCreated_SetToday()
        {
            sagaMetadata.DateCreated.Should().Be(currentTime);
        }
        
        [Fact]
        public void AfterInitialisation_DateLastModified_SetToday()
        {
            sagaMetadata.DateLastModified.Should().Be(currentTime);
        }

        [Fact]
        public void AfterInitialisation_ReceivedMessages_ContainsMessage()
        {
            sagaMetadata.ReceivedMessages.Should().HaveCount(1);
        }

        [Fact]
        public void AfterInitialisation_ReceivedMessages_MatchesType()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            receivedMessage.SagaMessage.Should().BeAssignableTo<MySagaInitiatingMessage>();
        }

        [Fact]
        public void AfterInitialisation_ReceivedMessage_Timestamp()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            receivedMessage.Timestamp.Should().Be(currentTime);
        }

        [Fact]
        public void AfterInitialisation_ReceivedMessage_CorrelationId()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            receivedMessage.SagaMessage.CorrelationId.Should().Be(correlationId);
        }

        [Fact]
        public void AfterInitialisation_ReceivedMessage_MessageData()
        {
            var receivedMessage = sagaMetadata.ReceivedMessages.First();

            var sagaMessage = (MySagaInitiatingMessage)receivedMessage.SagaMessage;
            sagaMessage.SomeRandomValue.Should().Be(message.SomeRandomValue);
        }

        public void Dispose()
        {
            TimeProvider.ResetToDefault();
        }
    }
}
