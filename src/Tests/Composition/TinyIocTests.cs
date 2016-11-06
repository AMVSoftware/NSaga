using System.Linq;
using FluentAssertions;
using NSaga;
using Tests.Stubs;
using TinyIoC;
using Xunit;

namespace Tests.Composition
{
    public class TinyIocTests
    {
        [Fact]
        public void Container_CanResolve_OpenGeneric()
        {
            //Arrange
            var container = TinyIoCContainer.Current;
            var assembliesToScan = typeof(SagaMediatorInitiationsTests).Assembly;

            var typesToRegister = assembliesToScan.GetTypes().Where(t => typeof(InitiatedBy<>).IsAssignableFrom(t)).ToList();
            container.RegisterMultiple(typeof(InitiatedBy<>), typesToRegister);

            // Act
            var result = container.Resolve<InitiatedBy<MySagaInitiatingMessage>>();

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType<MySaga>();
        }
    }
}
