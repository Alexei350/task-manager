using System.ComponentModel;
using TaskManager.Utils.Extensions;

namespace TaskManager.UnitTests.Utils;

public class AttributeExtensionsTests
{
    private enum TestEnum
    {
        [Description("Description 1")]
        Value1,
        Value2
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnDescription_WhenAttributeExists()
    {
        var result = TestEnum.Value1.GetEnumDescription();
        Assert.Equal("Description 1", result);
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnToString_WhenAttributeDoesNotExist()
    {
        var result = TestEnum.Value2.GetEnumDescription();
        Assert.Equal("Value2", result);
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnEmptyString_WhenEnumIsNull()
    {
        TestEnum? nullEnum = null;
        var result = nullEnum.GetEnumDescription();
        Assert.Equal(string.Empty, result);
    }
}
