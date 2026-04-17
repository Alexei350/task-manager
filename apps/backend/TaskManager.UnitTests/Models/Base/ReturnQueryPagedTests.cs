using TaskManager.Models.Base;
using Xunit;

namespace TaskManager.UnitTests.ModelTests.Base;

public class ReturnQueryPagedTests
{
    [Fact]
    public void ReturnQueryPaged_Should_SetProperties()
    {
        var result = new ReturnQueryPaged<string>
        {
            TotalRecords = 100,
            TotalPages = 10,
            Data = new List<string> { "Item1", "Item2", "Item3" }
        };

        Assert.Equal(100, result.TotalRecords);
        Assert.Equal(10, result.TotalPages);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count);
    }

    [Fact]
    public void ReturnQueryPaged_Should_HandleEmptyData()
    {
        var result = new ReturnQueryPaged<string>
        {
            TotalRecords = 0,
            TotalPages = 0,
            Data = new List<string>()
        };

        Assert.Equal(0, result.TotalRecords);
        Assert.Equal(0, result.TotalPages);
        Assert.Empty(result.Data);
    }

    [Fact]
    public void ReturnQueryPaged_Should_WorkWithComplexTypes()
    {
        var result = new ReturnQueryPaged<TestObject>
        {
            TotalRecords = 2,
            TotalPages = 1,
            Data = new List<TestObject>
            {
                new() { Id = 1, Name = "Test1" },
                new() { Id = 2, Name = "Test2" }
            }
        };

        Assert.Equal(2, result.TotalRecords);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("Test1", result.Data[0].Name);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
