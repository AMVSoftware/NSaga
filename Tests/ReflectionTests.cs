using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests
{
    public class ReflectionTests
    {
        [Fact]
        public void InvokeGenericMethod_Does_NotThrow()
        {
            // Arrange
            var sagaType = Reflection.GetSagaTypesInitiatedBy(new MySagaInitiatingMessage()).First(); // there could be only one!

            var spy = new RepositorySpy();
            var repository = new MyStubRepository(spy);
            var expectedGuid = Guid.NewGuid();

            // Act
            Reflection.InvokeGenericMethod(repository, "Find", sagaType, expectedGuid);

            // Assert
            spy.Find.Should().Be(expectedGuid);
        }


        [Fact]
        public void GetSagaTypesInitiatedBy_Returns_MySaga()
        {
            var initiatingMessage = new MySagaInitiatingMessage();

            var result = Reflection.GetSagaTypesInitiatedBy(initiatingMessage, typeof(ReflectionTests).Assembly);

            result.Should().HaveCount(1).And.Contain(typeof(MySaga));
        }





        class MySagaInitiatingMessage : IInitiatingSagaMessage
        {
            public Guid CorrelationId { get; set; }
        }


        class MySagaData
        {
            public String Name { get; set; }
        }

        class MySaga : ISaga<MySagaData>, InitiatedBy<MySagaInitiatingMessage>
        {
            public MySagaData SagaData { get; set; }
            public Guid CorrelationId { get; set; }
            public bool IsCompleted { get; set; }

            public OperationResult Initiate(MySagaInitiatingMessage message)
            {
                throw new NotImplementedException();
            }
        }

        class RepositorySpy
        {
            public Guid Find { get; set; }
        }

        class MyStubRepository : ISagaRepository
        {
            public RepositorySpy Spy { get; set; }

            public MyStubRepository(RepositorySpy spy = null)
            {
                Spy = spy ?? new RepositorySpy();
            }

            public TSaga Find<TSaga>(Guid correlationId) where TSaga : class
            {
                Spy.Find = correlationId;
                return null;
            }

            public void Save<TSaga>(TSaga saga) where TSaga : class
            {
            }

            public void Complete<TSaga>(TSaga saga) where TSaga : class
            {
            }
        }
    }
}
