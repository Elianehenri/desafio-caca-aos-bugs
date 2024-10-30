using System;
using Balta.Domain.SharedContext.Extensions;
using Xunit;

namespace Balta.Domain.Test.SharedContext.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ShouldGenerateBase64FromString()
        {
            // Arrange
            var originalString = "Hello, World!";
            var expectedBase64 = "SGVsbG8sIFdvcmxkIQ=="; 

            // Act
            var base64String = originalString.ToBase64();

            // Assert
            Assert.Equal(expectedBase64, base64String);
        }

        [Fact]
        public void ShouldGenerateBase64FromEmptyString()
        {
            // Arrange
            var originalString = "";
            var expectedBase64 = "";

            // Act
            var base64String = originalString.ToBase64();

            // Assert
            Assert.Equal(expectedBase64, base64String);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenNull()
        {
            // Arrange
            string originalString = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => originalString.ToBase64());
        }
    }
}
