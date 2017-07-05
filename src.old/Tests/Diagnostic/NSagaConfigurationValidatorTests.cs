using System;
using FluentAssertions;
using NSaga;
using Xunit;
using System.Reflection;

namespace Tests.Diagnostic
{
    public class NSagaConfigurationValidatorTests
    {
        [Fact]
        public void MethodName_StateUnderTests_ExpectedBehaviour()
        {
            //Arrange
            var sut = new NSagaConfigurationValidator(new[] { Assembly.GetExecutingAssembly() });

            // Act
            Action act = () => sut.AssertConfigurationIsValid();

            // Assert
            act.ShouldThrow<AggregateException>();
        }
    }
}
