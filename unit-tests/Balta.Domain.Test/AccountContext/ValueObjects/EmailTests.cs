using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;


namespace Balta.Domain.Test.AccountContext.ValueObjects
{
    public class EmailTests
    {
        private const string ExpectedAddress = "emailtests@testes.com";
        private readonly Mock<IDateTimeProvider> dateTimeProviderMock;

        public EmailTests()
        {
            dateTimeProviderMock = new Mock<IDateTimeProvider>();
        }

        [Fact]
        public void ShouldLowerCaseEmail()
        {
            // Arrange
            var upperCasedAddress = "EmailTests@Testes.Com";

            // Act
            var email = Email.ShouldCreate(upperCasedAddress, dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(ExpectedAddress, email.Address); 
        }

        [Fact]
        public void ShouldTrimEmail()
        {
            // Arrange
            var untrimmedAddress = " emailTests@testes.com ";

            // Act
            var email = Email.ShouldCreate(untrimmedAddress, dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(ExpectedAddress, email.Address);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldFailIfEmailIsNullOrEmpty(string invalidAddress)
        {
            // Arrange
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            // Act & Assert
            var exception = Record.Exception(() =>
                Email.ShouldCreate(invalidAddress ?? string.Empty, dateTimeProviderMock.Object)
            );

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidEmailException>(exception);
            Assert.Equal("E-mail inválido", exception.Message);
        }

        [Theory]
        [InlineData("emailTests@@testes.com")]
        [InlineData("emailTests@testes..com")]
        [InlineData("emailTests@.com")]
        [InlineData("emailTests@com")]
        public void ShouldFailIfEmailIsInvalid(string invalidAddress)
        {
            // Act & Assert
            Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(invalidAddress, dateTimeProviderMock.Object));
        }

        [Theory]
        [InlineData("emailTests@testes.com")]
        [InlineData("TestEmail@TESTES.COM")]
        public void ShouldPassIfEmailIsValid(string validEmail)
        {
            // Act
            var email = Email.ShouldCreate(validEmail, dateTimeProviderMock.Object);

            // Assert
            Assert.NotNull(email);
            Assert.Equal(validEmail.ToLower(), email.Address); 
        }


        [Fact]
        public void ShouldHashEmailAddress()
        {
            // Arrange
            var email = Email.ShouldCreate(ExpectedAddress.ToLowerInvariant(), dateTimeProviderMock.Object); 
            var expectedHash = ExpectedAddress.ToLowerInvariant().ToBase64(); 

            // Act & Assert
            Assert.Equal(expectedHash, email.Hash);
        }


        [Fact]
        public void ShouldGenerateUniqueHashForDifferentEmails()
        {
            // Arrange
            var email1 = Email.ShouldCreate("email1@testes.com", dateTimeProviderMock.Object);
            var email2 = Email.ShouldCreate("email2@testes.com", dateTimeProviderMock.Object);

            // Act & Assert
            Assert.NotEqual(email1.Hash, email2.Hash);
        }

       
        [Fact]
        public void ShouldExplicitConvertFromString()
        {
            // Arrange
            var emailString = "test@test.com";

            // Act
            var email = Email.ShouldCreate(emailString, dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(emailString, email.Address);
        }


        [Fact]
        public void ShouldExplicitConvertToString()
        {
            // Arrange
            var emailString = "emailTests@testes.com"; 
            var email = Email.ShouldCreate(emailString, dateTimeProviderMock.Object);

            // Act
            var result = (string)email;

            // Assert
            Assert.Equal(emailString.ToLower(), result); 
        }
        [Fact]
        public void ShouldReturnEmailWhenCallToStringMethod()
        {
            // Arrange
            var email = Email.ShouldCreate(ExpectedAddress, dateTimeProviderMock.Object);

            // Act
            var result = email.ToString();

            // Assert
            Assert.Equal(ExpectedAddress, result);
        }
    }
}
