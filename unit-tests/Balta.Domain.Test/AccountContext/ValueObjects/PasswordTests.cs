using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using System.Reflection;


namespace Balta.Domain.Test.AccountContext.ValueObjects
{
    public class PasswordTests
    {
        [Fact]
        public void ShouldFailIfPasswordIsNull()
        {
            var exception = Assert.Throws<InvalidPasswordException>(() =>
                Password.ShouldCreate(null));
            Assert.Equal("Password cannot be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldFailIfPasswordIsEmpty()
        {
            var exception = Assert.Throws<InvalidPasswordException>(() =>
                Password.ShouldCreate(string.Empty));
            Assert.Equal("Password cannot be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldFailIfPasswordIsWhiteSpace()
        {
            var exception = Assert.Throws<InvalidPasswordException>(() =>
                Password.ShouldCreate("   "));
            Assert.Equal("Password cannot be null or empty", exception.Message);
        }

        [Fact]
        public void ShouldFailIfPasswordLenIsLessThanMinimumChars()
        {
            var exception = Assert.Throws<InvalidPasswordException>(() =>
                Password.ShouldCreate("1234567")); 
            Assert.Equal("Password should have at least 8 characters", exception.Message);
        }

        [Fact]
        public void ShouldFailIfPasswordLenIsGreaterThanMaxChars()
        {
            var longPassword = new string('a', 49); 
            var exception = Assert.Throws<InvalidPasswordException>(() =>
                Password.ShouldCreate(longPassword));
            Assert.Equal("Password should have less than 48 characters", exception.Message);
        }

        [Fact]
        public void ShouldHashPassword()
        {
            string plainTextPassword = "SecureP@ssw0rd";
            var password = Password.ShouldCreate(plainTextPassword);

            Assert.NotNull(password.Hash);
            Assert.NotEqual(plainTextPassword, password.Hash);
        }

        [Fact]
        public void ShouldVerifyPasswordHash()
        {
            string plainTextPassword = "SecureP@ssw0rd";
            var password = Password.ShouldCreate(plainTextPassword);

            var isMatch = Password.ShouldMatch(password.Hash, plainTextPassword);
            Assert.True(isMatch);
        }

        [Fact]
        public void ShouldGenerateStrongPassword()
        {
            var generatedPassword = Password.ShouldGenerate(16, true, true);

            Assert.NotNull(generatedPassword);
            Assert.Equal(16, generatedPassword.Length);
            Assert.Contains(generatedPassword, c => char.IsUpper(c));
            Assert.Contains(generatedPassword, c => "!@#$%ˆ&*(){}[];".Contains(c));
        }

        [Fact]
        public void ShouldImplicitConvertToString()
        {
            string plainTextPassword = "ImplicitP@ssw0rd";
            Password password = Password.ShouldCreate(plainTextPassword);

            string passwordString = password;

            Assert.Equal(password.Hash, passwordString);
        }

        [Fact]
        public void ShouldReturnHashAsStringWhenCallToStringMethod()
        {
            string plainTextPassword = "ToStringP@ss";
            var password = Password.ShouldCreate(plainTextPassword);

            Assert.Equal(password.Hash, password.ToString());
        }

        [Fact]
        public void ShouldMarkPasswordAsExpired()
        {
            // Arrange
            var password = Password.ShouldCreate("MarkExpired123!");

            // Act
            
            bool isExpired = password.ExpiresAtUtc == null;

            // Assert
            Assert.True(isExpired, "A senha deveria estar marcada como expirada inicialmente.");
        }

        [Fact]
        public void ShouldFailIfPasswordIsExpired()
        {
            // Arrange
            var password = Password.ShouldCreate("ExpiredP@ssword");

            
            var expiresAtField = typeof(Password)
                .GetField("<ExpiresAtUtc>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

            if (expiresAtField == null)
                throw new Exception("Field for ExpiresAtUtc not found.");

            
            expiresAtField.SetValue(password, DateTime.UtcNow.AddDays(-1));

            // Act
            bool isExpired = password.ExpiresAtUtc.HasValue && password.ExpiresAtUtc < DateTime.UtcNow;

            // Assert
            Assert.True(isExpired, "A senha deveria estar marcada como expirada.");
        }

        [Fact]
        public void ShouldMarkPasswordAsMustChange()
        {
            // Arrange
            var password = Password.ShouldCreate("MustChange123!");

            // Act
            bool mustChangeCondition = password.MustChange;

            // Assert
            Assert.False(mustChangeCondition, "A senha não deveria estar marcada como `MustChange` por padrão.");
        }

        [Fact]
        public void ShouldFailIfPasswordIsMarkedAsMustChange()
        {
            // Arrange
            var password = Password.ShouldCreate("MustChangeTest123");

            // Act
            bool mustChangeCondition = password.MustChange;

            // Assert
            Assert.False(mustChangeCondition, "A senha não deveria estar marcada como `MustChange` por padrão.");
        }
    }
}
