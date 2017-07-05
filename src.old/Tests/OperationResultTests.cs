using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSaga;
using Xunit;

namespace Tests
{
    public class OperationResultTests
    {
        [Fact]
        public void IsSuccessful_ReturnsTrue_WhenNoErrors()
        {
            //Arrange
            var sut = new OperationResult();

            // Act
            var result = sut.IsSuccessful;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsSuccessful_ReturnsFalse_WhenHasErrors()
        {
            //Arrange
            var sut = new OperationResult().AddError("some error");

            // Act
            var result = sut.IsSuccessful;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasErrors_ReturnsFalse_WhenNoErrors()
        {
            //Arrange
            var sut = new OperationResult();

            // Act
            var result = sut.HasErrors;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasErrors_ReturnsTrue_WhenHasErrors()
        {
            //Arrange
            var sut = new OperationResult().AddError("some error");

            // Act
            var result = sut.HasErrors;

            // Assert
            result.Should().BeTrue();
        }


        [Fact]
        public void AddPayload_Sets_PayloadObject()
        {
            //Arrange
            var expected = "Hello";
            var sut = new OperationResult().AddPayload(expected);

            // Act
            var result = sut.PayLoad;

            // Assert
            result.Should().Be(expected);
        }


        [Fact]
        public void ToString_MultipleErrors_GenerateCSV()
        {
            //Arrange
            var sut = new OperationResult().AddError("One").AddError("Two").AddError("Three");

            // Act
            var result = sut.ToString();

            // Assert
            result.Should().Be("One, Two, Three");
        }

        [Fact]
        public void ToString_SingleErrors_ReturnsError()
        {
            //Arrange
            var sut = new OperationResult().AddError("One");

            // Act
            var result = sut.ToString();

            // Assert
            result.Should().Be("One");
        }

        [Fact]
        public void ToString_NoErrors_ReturnsEmptyString()
        {
            //Arrange
            var sut = new OperationResult();

            // Act
            var result = sut.ToString();

            // Assert
            result.Should().Be(String.Empty);
        }

        [Fact]
        public void Ctor_Adds_ErrorMessages()
        {
            var sut = new OperationResult("one", "two", "three");

            sut.Errors.Should().Contain("one", "two", "three")
                               .And.HaveCount(3);
        }

        [Fact]
        public void AddError_Should_AddErrorToList()
        {
            //Arrange
            var sut = new OperationResult();
            var expected = Guid.NewGuid().ToString();

            // Act
            sut.AddError(expected);

            // Assert
            sut.Errors.First().Should().Be(expected);
        }

        [Fact]
        public void AddErrors_Adds_MultipleErrors()
        {
            //Arrange
            var sut = new OperationResult("three");

            // Act
            sut.AddErrors(new List<string>() {"one", "two"});

            // Assert
            sut.Errors.Should().Contain("one", "two", "three")
                               .And.HaveCount(3);
        }

        [Fact]
        public void AddErrors_AsParams_AddsMultipleErrors()
        {
            //Arrange
            var sut = new OperationResult("three");

            // Act
            sut.AddErrors("one", "two");

            // Assert
            sut.Errors.Should().Contain("one", "two", "three")
                               .And.HaveCount(3);
        }
    }
}
