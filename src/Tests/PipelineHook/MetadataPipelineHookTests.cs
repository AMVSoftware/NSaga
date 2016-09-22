using System;
using System.Collections.Generic;
using NSaga;
using NSubstitute;
using Tests.Stubs;
using Xunit;

namespace Tests.PipelineHook
{
    public class MetadataPipelineHookTests
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;
        private readonly IPipelineHook pipelineHook;

        public MetadataPipelineHookTests()
        {
            pipelineHook = Substitute.For<IPipelineHook>();
            var serviceLocator = new DumbServiceLocator();
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, pipelineHook, typeof(SagaMediatorInitiationsTests).Assembly);
        }

        [Fact]
        public void Initiation_PipelineHooks_ExecutedInOrder()
        {
            //Arrange
            var initiatingMessage = new MySagaInitiatingMessage(Guid.NewGuid());

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            Received.InOrder(() =>
            {
                pipelineHook.BeforeInitialisation(Arg.Any<PipelineContext>());
                pipelineHook.AfterInitialisation(Arg.Any<PipelineContext>());
                pipelineHook.AfterSave(Arg.Any<PipelineContext>());
            });
        }


        [Fact]
        public void Initiation_ValidationFails_SaveNotCalled()
        {
            //Arrange
            var initiatingMessage = new InitiatingSagaWithErrors(Guid.NewGuid());

            // Act
            sut.Consume(initiatingMessage);

            // Assert
            Received.InOrder(() =>
            {
                pipelineHook.BeforeInitialisation(Arg.Any<PipelineContext>());
                pipelineHook.AfterInitialisation(Arg.Any<PipelineContext>());
            });
            pipelineHook.DidNotReceive().AfterSave(Arg.Any<PipelineContext>());
        }


        [Fact]
        public void Consumed_PipelineHooks_ExecutedInOrder()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            repository.Save(new MySaga() { CorrelationId = correlationId });

            var message = new MySagaConsumingMessage(correlationId);

            // Act
            sut.Consume(message);

            // Assert
            Received.InOrder(() =>
            {
                pipelineHook.BeforeConsuming(Arg.Any<PipelineContext>());
                pipelineHook.AfterConsuming(Arg.Any<PipelineContext>());
                pipelineHook.AfterSave(Arg.Any<PipelineContext>());
            });
        }


        [Fact]
        public void Consumed_MessageWithErrors_AfterSaveWasNotCalled()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            repository.Save(new SagaWithErrors() { CorrelationId = correlationId, SagaData = new SagaWithErrorsData(), Headers = new Dictionary<string, string>() });

            var message = new GetSomeConsumedErrorsForSagaWithErrors(correlationId);

            // Act
            sut.Consume(message);

            // Assert
            Received.InOrder(() =>
            {
                pipelineHook.BeforeConsuming(Arg.Any<PipelineContext>());
                pipelineHook.AfterConsuming(Arg.Any<PipelineContext>());
            });
            pipelineHook.DidNotReceive().AfterSave(Arg.Any<PipelineContext>());
        }
    }
}
