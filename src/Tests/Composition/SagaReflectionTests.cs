using System.Reflection;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests.Composition
{
    public class SagaReflectionTests
    {
        [Fact]
        public void GetSagaTypes_Always_ContainsMySaga()
        {
            // Act
            var result = SagaReflection.GetAllSagaTypes(new Assembly[] { typeof(SagaReflectionTests).Assembly });

            // Assert
            result.Should().Contain(s => s == typeof(MySaga))
                        .And.Contain(s => s == typeof(SagaWithErrors))
                        .And.HaveCount(2);
        }
    }
}