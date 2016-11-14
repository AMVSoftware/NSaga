using System;
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
            var namespaces = assembly.GetTypes()
                                     .Where(t => t.IsPublic)
                                     .Select(t => t.Namespace)
                                     .Distinct()
                                     .ToList();

            // Assert
            var names = String.Join(", ", namespaces);
            namespaces.Should().HaveCount(1, $"Should only contain 'NSaga' namespace, but found '{names}'");
        }

        [Fact]
        public void PetaPoco_Stays_Internal()
        {
            //Arrange
            var petapocoTypes = typeof(SqlSagaRepository).Assembly
                                .GetTypes()
                                .Where(t => !String.IsNullOrEmpty(t.Namespace))
                                .Where(t => t.Namespace.StartsWith("PetaPoco", StringComparison.OrdinalIgnoreCase))
                                .Where(t => t.IsPublic)
                                .ToList();

            petapocoTypes.Should().BeEmpty();
        }
    }
}
