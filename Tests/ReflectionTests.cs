using System;
using System.Linq;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
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



        [Fact]
        public void InvokeMethod_Does_NotThrow()
        {
            //Arrange
            var testSubject = new MyReflectionTestSubject();
            var expected = Guid.NewGuid();
            // Act
            Reflection.InvokeMethod(testSubject, "Initialise", expected);

            // Assert
            testSubject.Id.Should().Be(expected);
        }


        public class MyReflectionTestSubject
        {
            public Guid Id { get; set; }

            public void Initialise(Guid id)
            {
                this.Id = id;
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
