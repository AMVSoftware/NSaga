using System;
using System.Collections.Generic;
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
            var assembly = typeof(TinyIocTests).Assembly;

            var sagaInterfaces = new List<Type>() { typeof(ISaga<>), typeof(InitiatedBy<>), typeof(ConsumerOf<>) };

            var allSagaTypes = assembly.GetTypes()
                                .Where(t => DoesTypeSupportInterface(t, typeof(ISaga<>)))
                                .ToList();

            foreach (var sagaType in allSagaTypes)
            {
                var interfaces = sagaType.GetInterfaces()
                                         .Where(i => i.IsGenericType && sagaInterfaces.Contains(i.GetGenericTypeDefinition()))
                                         .ToList();

                foreach (var @interface in interfaces)
                {
                    container.Register(@interface, sagaType);
                }
            }

            // Act
            var result = container.Resolve<InitiatedBy<MySagaInitiatingMessage>>();

            // Assert
            result.Should().NotBeNull()
                       .And.BeOfType<MySaga>();
        }

        private static bool DoesTypeSupportInterface(Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
            {
                return true;
            }

            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
            {
                return true;
            }
            return false;
        }
    }
}
