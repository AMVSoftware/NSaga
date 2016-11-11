using System.Linq;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests
{
    public class NamespaceTests
    {
        [Fact]
        public void NSaga_Contains_Only_One_Namespace()
        {
            //Arrange
            var assembly = typeof(ISagaMediator).Assembly;

            // Act
            var namespaces = assembly.GetTypes().Select(t => t.Namespace).Distinct().ToList();

            // Assert
            namespaces.Should().HaveCount(1);
        }
    }
}
