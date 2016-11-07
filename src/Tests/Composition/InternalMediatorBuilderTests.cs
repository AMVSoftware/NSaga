using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests.Composition
{
    public class InternalMediatorBuilderTests
    {
        [Fact]
        public void Can_Resolve_AllObjects()
        {
            //Arrange
            var builder = Wireup.UseInternalContainer().RegisterComponents();
            var container = builder.Container;

            // Act
            var repository = container.Resolve<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull()
                               .And.Should().BeOfType<InMemorySagaRepository>();
        }

        [Fact]
        public void Can_Resolve_Registration()
        {
            //Arrange
            var container = TinyIoC.TinyIoCContainer.Current;
            var repositoryRegistration = new Registration(typeof(InMemorySagaRepository));
            //container.Register(repositoryRegistration.Type);
            container.Register(typeof(ISagaRepository), typeof(InMemorySagaRepository));

            // Act
            var repository = container.Resolve<ISagaRepository>();

            // Assert
            repository.Should().NotBeNull()
                               .And.Should().BeOfType<InMemorySagaRepository>();

        }
    }
}
