using System;
using FluentAssertions;
using NSaga;

using Xunit;


namespace Tests
{
    /// <summary>
    /// Template for all repository tests
    /// </summary>
    public abstract class SagaRepositoryTestsTemplate
    {
        protected ISagaRepository Sut;


        [Fact]
        public void Find_NoSaga_ReturnsNull()
        {
            // Act
            var result = Sut.Find<MySaga>(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }


        [Fact]
        public void Save_Persists_CorrelationId()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };

            // Act
            Sut.Save(saga);

            // Assert
            var restoredSaga = Sut.Find<MySaga>(correlationId);
            restoredSaga.CorrelationId.Should().Be(correlationId);
        }

        [Fact]
        public void Save_Persists_SagaData()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            var expectedGuid = Guid.NewGuid();
            saga.SagaData.SomeGuid = expectedGuid;

            // Act
            Sut.Save(saga);

            // Assert
            var restoredSaga = Sut.Find<MySaga>(correlationId);
            restoredSaga.SagaData.SomeGuid.Should().Be(expectedGuid);
        }


        [Fact]
        public void Save_Persists_Headers()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            saga.Headers.Add(key, value);

            // Act
            Sut.Save(saga);

            // Assert
            var restoredSaga = Sut.Find<MySaga>(correlationId);
            restoredSaga.Headers[key].Should().Be(value);
        }


        [Fact]
        public void Complete_Removes_Saga()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            saga.SagaData.SomeGuid = Guid.NewGuid();
            saga.Headers.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Sut.Save(saga);

            // Act
            Sut.Complete(saga);

            // Assert
            var deletedSaga = Sut.Find<MySaga>(correlationId);
            deletedSaga.Should().BeNull();
        }


        [Fact]
        public void CompleteByCorrleationId_Removes_Saga()
        {
            //Arrange
            var correlationId = Guid.NewGuid();
            var saga = new MySaga { CorrelationId = correlationId };
            saga.SagaData.SomeGuid = Guid.NewGuid();
            saga.Headers.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Sut.Save(saga);

            // Act
            Sut.Complete(saga.CorrelationId);

            // Assert
            var deletedSaga = Sut.Find<MySaga>(correlationId);
            deletedSaga.Should().BeNull();
        }
    }
}
