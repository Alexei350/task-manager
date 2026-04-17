using TaskManager.Models.Enums;
using TaskManager.Utils.Extensions;

namespace TaskManager.UnitTests.Utils
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("123.456.789-00", "12345678900")]
        [InlineData("abc123def456", "123456")]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("", "")]
        [InlineData("abcdef", "")]
        public void OnlyNumbers_ShouldReturnOnlyNumericCharacters(string input, string expected)
        {
            // Act
            var result = input.OnlyNumbers();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("12345678900", TxIdTypeEnum.Cpf, "123.456.789-00")]
        [InlineData("11122233344", TxIdTypeEnum.Cpf, "111.222.333-44")]
        public void FormatTxId_WithValidCpf_ShouldFormatCorrectly(string input, TxIdTypeEnum type, string expected)
        {
            // Act
            var result = input.FormatTxId(type);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("12345678000190", TxIdTypeEnum.Cnpj, "12.345.678/0001-90")]
        [InlineData("11222333000144", TxIdTypeEnum.Cnpj, "11.222.333/0001-44")]
        public void FormatTxId_WithValidCnpj_ShouldFormatCorrectly(string input, TxIdTypeEnum type, string expected)
        {
            // Act
            var result = input.FormatTxId(type);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("123", TxIdTypeEnum.Cpf, "123")]
        [InlineData("12345", TxIdTypeEnum.Cnpj, "12345")]
        public void FormatTxId_WithInvalidLength_ShouldReturnOriginal(string input, TxIdTypeEnum type, string expected)
        {
            // Act
            var result = input.FormatTxId(type);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatTxId_WithInvalidType_ShouldReturnCleanedNumber()
        {
            // Arrange
            var input = "123.456.789-00";
            var type = (TxIdTypeEnum)999; // Tipo inválido

            // Act
            var result = input.FormatTxId(type);

            // Assert
            Assert.Equal("12345678900", result);
        }
    }
}
