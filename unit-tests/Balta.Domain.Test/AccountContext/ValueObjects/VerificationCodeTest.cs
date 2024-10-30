using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects
{
    public class VerificationCodeTest
    {
        private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;

        public VerificationCodeTest()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(DateTime.UtcNow);


        }

        [Fact]
        public void ShouldGenerateVerificationCode()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.NotNull(verificationCode.Code);
            Assert.Equal(6, verificationCode.Code.Length);
        }

        [Fact]
        public void ShouldGenerateExpiresAtInFuture()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.True(verificationCode.ExpiresAtUtc > DateTime.UtcNow);
        }

        [Fact]
        public void ShouldGenerateVerifiedAtAsNull()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.Null(verificationCode.VerifiedAtUtc);
        }

        [Fact]
        public void ShouldBeInactiveWhenCreated()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.False(verificationCode.IsActive);
        }

        [Fact]
        public void ShouldFailIfExpired()
        {
            _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(DateTime.UtcNow.AddMinutes(6));
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.True(verificationCode.ExpiresAtUtc < _mockDateTimeProvider.Object.UtcNow);
        }

        [Fact]
        public void ShouldFailIfCodeIsInvalid()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(null));
        }

        [Fact]
        public void ShouldFailIfIsNotActive()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("123456"));
        }



        public void ShouldFailIfIsAlreadyVerified()
        {
            // Arrange
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            var validCode = verificationCode.Code;

            // Act
            verificationCode.ShouldVerify(validCode);

            // Assert
            Assert.Throws<InvalidVerificationCodeException>(() =>
            {
                
                verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
                verificationCode.ShouldVerify(validCode);
            });
        }



        [Fact]
        public void ShouldFailIfIsVerificationCodeDoesNotMatch()
        {
            var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);
            Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify("WRONGCODE"));
        }

        //faltou teste para verificar se o código foi verificado com sucesso
        


    }
}


