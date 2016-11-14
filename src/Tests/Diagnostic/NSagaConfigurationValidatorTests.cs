using System;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests.Diagnostic
{
    public class NSagaConfigurationValidatorTests
    {
        [Fact]
        public void MethodName_StateUnderTests_ExpectedBehaviour()
        {
            //Arrange
            var sut = new NSagaConfigurationValidator(AppDomain.CurrentDomain.GetAssemblies());

            // Act
            Action act = () => sut.AssertConfigurationIsValid();

            // Assert
            act.ShouldThrow<AggregateException>();
        }
    }
}
