using TaskManager.Utils.Extensions;

namespace TaskManager.UnitTests.Utils
{
    public class ValidationExtensionsTests
    {
        [Theory]
        [InlineData("11144477735", true)]
        [InlineData("52998224725", true)]
        [InlineData("00000000000", false)]
        [InlineData("11111111111", false)]
        [InlineData("12345678901", false)]
        [InlineData("123456789", false)]
        [InlineData("", false)]
        public void IsValidCpf_ShouldValidateCorrectly(string cpf, bool expected)
        {
            // Act
            var result = cpf.IsValidCpf();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("11222333000181", true)]
        [InlineData("11444777000161", true)]
        [InlineData("11111111111111", false)]
        [InlineData("12345678000191", false)]
        [InlineData("1234567800019", false)]
        [InlineData("", false)]
        public void IsValidCnpj_ShouldValidateCorrectly(string cnpj, bool expected)
        {
            // Act
            var result = cnpj.IsValidCnpj();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("user.name@domain.com", true)]
        [InlineData("invalid.email", false)]
        [InlineData("@example.com", false)]
        [InlineData("test@", false)]
        [InlineData("", false)]
        public void IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
        {
            // Act
            var result = email.IsValidEmail();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsValidCpf_WithNull_ShouldReturnFalse()
        {
            // Act
#pragma warning disable CS8600
            var result = ((string)null!).IsValidCpf();
#pragma warning restore CS8600

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidCnpj_WithNull_ShouldReturnFalse()
        {
            // Act
#pragma warning disable CS8600
            var result = ((string)null!).IsValidCnpj();
#pragma warning restore CS8600

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_WithNull_ShouldReturnFalse()
        {
            // Act
#pragma warning disable CS8600
            var result = ((string)null!).IsValidEmail();
#pragma warning restore CS8600

            // Assert
            Assert.False(result);
        }
    }
}
