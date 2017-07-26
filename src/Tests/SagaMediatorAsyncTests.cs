using FluentAssertions;
using NSaga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Stubs;
using Xunit;

namespace Tests
{
    public class SagaMediatorAsyncTests
    {
        private readonly InMemorySagaRepository repository;
        private readonly SagaMediator sut;

        public SagaMediatorAsyncTests()
        {
            var container = new TinyIoCContainer();
            container.RegisterSagas(typeof(SagaMediatorConsumingTests).Assembly);
            var serviceLocator = new TinyIocSagaFactory(container);
            repository = new InMemorySagaRepository(new JsonNetSerialiser(), serviceLocator);
            sut = new SagaMediator(repository, serviceLocator, new[] { new NullPipelineHook() });
        }


        [Fact]
        public async Task Consume_MessageConsumed_SagaPersisted()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            sut.Consume(new MyAsyncSagaInitialser(correlationId));
            var message = new MyAsyncMessage(correlationId);

            // Act
            await sut.ConsumeAsync(message);

            // Assert
            var saga = repository.Find<MyAsyncSaga>(correlationId);
            saga.SagaData.IsConsumingMessageReceived.Should().BeTrue();
        }
    }
}
