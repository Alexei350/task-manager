using TaskManager.Models.Enums;
using TaskManager.Utils.Extensions;
using Xunit;

namespace TaskManager.UnitTests.Utils.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void OnlyNumbers_ShouldRemoveNonNumericCharacters()
    {
        var input = "123-456-789";
        var result = input.OnlyNumbers();
        
        Assert.Equal("123456789", result);
    }

    [Fact]
    public void OnlyNumbers_ShouldHandleComplexStrings()
    {
        var input = "ABC123!@#456XYZ789";
        var result = input.OnlyNumbers();
        
        Assert.Equal("123456789", result);
    }

    [Theory]
    [InlineData("12345678901", "123.456.789-01")]
    [InlineData("11122233344", "111.222.333-44")]
    public void FormatTxId_Cpf_ShouldFormatCorrectly(string input, string expected)
    {
        var result = input.FormatTxId(TxIdTypeEnum.Cpf);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatTxId_Cpf_InvalidLength_ShouldReturnUnformatted()
    {
        var input = "123456789";
        var result = input.FormatTxId(TxIdTypeEnum.Cpf);
        
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("12345678901234", "12.345.678/9012-34")]
    [InlineData("11222333000144", "11.222.333/0001-44")]
    public void FormatTxId_Cnpj_ShouldFormatCorrectly(string input, string expected)
    {
        var result = input.FormatTxId(TxIdTypeEnum.Cnpj);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatTxId_Cnpj_InvalidLength_ShouldReturnUnformatted()
    {
        var input = "123456789012";
        var result = input.FormatTxId(TxIdTypeEnum.Cnpj);
        
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("11987654321", "(11) 98765-4321")]
    [InlineData("1133334444", "(11) 3333-4444")]
    public void FormatPhone_ShouldFormatCorrectly(string input, string expected)
    {
        var result = input.FormatPhone();
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatPhone_InvalidLength_ShouldReturnUnformatted()
    {
        var input = "123456";
        var result = input.FormatPhone();
        
        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("12345678", "12345-678")]
    [InlineData("01310100", "01310-100")]
    public void FormatZipCode_ShouldFormatCorrectly(string input, string expected)
    {
        var result = input.FormatZipCode();
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatZipCode_InvalidLength_ShouldReturnUnformatted()
    {
        var input = "12345";
        var result = input.FormatZipCode();
        
        Assert.Equal(input, result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrue_ForNull()
    {
        string? input = null;
        var result = input.IsNullOrEmpty();
        
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrue_ForEmpty()
    {
        var input = string.Empty;
        var result = input.IsNullOrEmpty();
        
        Assert.True(result);
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnFalse_ForNonEmpty()
    {
        var input = "test";
        var result = input.IsNullOrEmpty();
        
        Assert.False(result);
    }

    [Fact]
    public void FormatTxId_WithMaskedInput_ShouldRemoveMaskAndReformat()
    {
        var input = "123.456.789-01";
        var result = input.FormatTxId(TxIdTypeEnum.Cpf);
        
        Assert.Equal("123.456.789-01", result);
    }

    [Fact]
    public void FormatPhone_WithMaskedInput_ShouldRemoveMaskAndReformat()
    {
        var input = "(11) 98765-4321";
        var result = input.FormatPhone();
        
        Assert.Equal("(11) 98765-4321", result);
    }
}
