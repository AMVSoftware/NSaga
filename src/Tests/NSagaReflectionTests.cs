using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSaga;
using Tests.Composition;
using Xunit;

namespace Tests
{
    public class NSagaReflectionTests
    {
        [Fact]
        public void InvokeGenericMethod_Does_NotThrow()
        {
            // Arrange
            var sagaType = NSagaReflection.GetSagaTypesInitiatedBy(new MySagaInitiatingMessage()).First(); // there could be only one!

            var spy = new RepositorySpy();
            var repository = new MyStubRepository(spy);
            var expectedGuid = Guid.NewGuid();

            // Act
            NSagaReflection.InvokeGenericMethod(repository, "Find", sagaType, expectedGuid);

            // Assert
            spy.Find.Should().Be(expectedGuid);
        }


        [Fact]
        public void GetSagaTypesInitiatedBy_Returns_MySaga()
        {
            // Arrange
            var initiatingMessage = new MySagaInitiatingMessage();

            // Act
            var result = NSagaReflection.GetSagaTypesInitiatedBy(initiatingMessage, typeof(NSagaReflectionTests).Assembly);

            // Assert
            result.Should().HaveCount(1).And.Contain(typeof(MySaga));
        }


        [Fact]
        public void GetSagaTypesConsuming_Returns_MySaga()
        {
            //Arrange
            var consumedMessage = new MySagaConsumingMessage();

            // Act
            var result = NSagaReflection.GetSagaTypesConsuming(consumedMessage, typeof(NSagaReflectionTests).Assembly);

            // Assert
            result.Should().HaveCount(1).And.Contain(typeof(MySaga));
        }


        [Fact]
        public void InvokeMethod_Does_NotThrow()
        {
            //Arrange
            var testSubject = new MyReflectionTestSubject();
            var expected = Guid.NewGuid();
            // Act
            NSagaReflection.InvokeMethod(testSubject, "Initialise", expected);

            // Assert
            testSubject.Id.Should().Be(expected);
        }

        [Fact]
        public void InvokeMethod_OverloadString_CallsTheCorrectOverload()
        {
            //Arrange
            var testSubject = new MyReflectionTestSubject();
            var expectedString = Guid.NewGuid().ToString();

            // Act
            NSagaReflection.InvokeMethod(testSubject, "Overload", expectedString);

            // Assert
            testSubject.OverloadString.Should().Be(expectedString);
        }

        [Fact]
        public void InvokeMethod_OverloadInt_CallsTheCorrectOverload()
        {
            //Arrange
            var testSubject = new MyReflectionTestSubject();
            var expectedInt = 42;

            // Act
            NSagaReflection.InvokeMethod(testSubject, "Overload", expectedInt);

            // Assert
            testSubject.OverloadInt.Should().Be(expectedInt);
        }


        [Fact]
        public void GetSagaTypes_Always_ContainsMySaga()
        {
            // Act
            var result = NSagaReflection.GetAllSagaTypes(new Assembly[] { typeof(NSagaReflectionTests).Assembly });

            // Assert
            result.Should().Contain(s => s == typeof(MySaga))
                        .And.Contain(s => s == typeof(SagaWithErrors))
                        .And.HaveCount(2);
        }

        public class MyReflectionTestSubject
        {
            public Guid Id { get; set; }
            public String OverloadString { get; set; }
            public int OverloadInt { get; set; }

            public void Initialise(Guid id)
            {
                this.Id = id;
            }

            public void Overload(string value)
            {
                OverloadString = value;
            }

            public void Overload(int value)
            {
                OverloadInt = value;
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

            public TSaga Find<TSaga>(Guid correlationId) where TSaga : class, IAccessibleSaga
            {
                Spy.Find = correlationId;
                return null;
            }

            public void Save<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
            {
            }

            public void Complete<TSaga>(TSaga saga) where TSaga : class, IAccessibleSaga
            {
            }

            public void Complete(Guid correlationId)
            {
                throw new NotImplementedException();
            }
        }
    }
}
