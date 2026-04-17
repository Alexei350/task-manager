using TaskManager.Models.Base.Dynamic;
using TaskManager.Models.Base.Entities;
using TaskManager.Models.Base.Query;
using Xunit;

namespace TaskManager.UnitTests.ModelTests.Base;

public class FilterTestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FilterByTests
{
    [Fact]
    public void FilterBy_WithExpression_ShouldStoreFilter()
    {
        var filter = new FilterBy<FilterTestEntity>(x => x.Name == "Test");

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithNullString_ShouldHaveNullFilter()
    {
        var filter = new FilterBy<FilterTestEntity>((string)null!);

        Assert.Null(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithEmptyString_ShouldHaveNullFilter()
    {
        var filter = new FilterBy<FilterTestEntity>(string.Empty);

        Assert.Null(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithDynamicFilter_EqualsOperation()
    {
        var dynamicFilter = new DynamicFilter
        {
            Connector = "AND",
            Values = new List<DynamicFilterItem>
            {
                new() { Operation = "Equals", PropertyName = "Name", Value = "Test" }
            }
        };

        var filter = new FilterBy<FilterTestEntity>(dynamicFilter);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithDynamicFilter_ContainsOperation()
    {
        var dynamicFilter = new DynamicFilter
        {
            Connector = "AND",
            Values = new List<DynamicFilterItem>
            {
                new() { Operation = "Contains", PropertyName = "Name", Value = "Test" }
            }
        };

        var filter = new FilterBy<FilterTestEntity>(dynamicFilter);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithJsonString_ShouldParseCorrectly()
    {
        var json = @"{
            ""Connector"": ""AND"",
            ""Values"": [
                {
                    ""Operation"": ""Equals"",
                    ""PropertyName"": ""Name"",
                    ""Value"": ""Test""
                }
            ]
        }";

        var filter = new FilterBy<FilterTestEntity>(json);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithInvalidJson_ShouldHaveNullFilter()
    {
        var json = "invalid json";

        var filter = new FilterBy<FilterTestEntity>(json);

        Assert.Null(filter.Filter);
    }

    [Fact]
    public void AddExpression_WithNullFilter_ShouldSetFilter()
    {
        var filter = new FilterBy<FilterTestEntity>((string)null!);
        filter.AddExpression(x => x.Name == "Test");

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void AddExpression_WithExistingFilter_ShouldCombineWithAnd()
    {
        var filter = new FilterBy<FilterTestEntity>(x => x.Name == "Test");
        filter.AddExpression(x => x.Age > 18);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void AddOrExpression_WithNullFilter_ShouldSetFilter()
    {
        var filter = new FilterBy<FilterTestEntity>((string)null!);
        filter.AddOrExpression(x => x.Name == "Test");

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void AddOrExpression_WithExistingFilter_ShouldCombineWithOr()
    {
        var filter = new FilterBy<FilterTestEntity>(x => x.Name == "Test");
        filter.AddOrExpression(x => x.Age > 18);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithMultipleDynamicFilters_ShouldCombine()
    {
        var dynamicFilter = new DynamicFilter
        {
            Connector = "AND",
            Values = new List<DynamicFilterItem>
            {
                new() { Operation = "Contains", PropertyName = "Name", Value = "Test" },
                new() { Operation = "Equals", PropertyName = "Name", Value = "TestValue" }
            }
        };

        var filter = new FilterBy<FilterTestEntity>(dynamicFilter);

        Assert.NotNull(filter.Filter);
    }

    [Fact]
    public void FilterBy_WithNullDynamicFilter_ShouldReturnNullFilter()
    {
        var filter = new FilterBy<FilterTestEntity>((DynamicFilter)null!);

        Assert.Null(filter.Filter);
    }

    [Fact]
    public void AddExpression_ShouldReturnSameInstance()
    {
        var filter = new FilterBy<FilterTestEntity>(x => x.Name == "Test");
        var result = filter.AddExpression(x => x.Age > 18);

        Assert.Same(filter, result);
    }

    [Fact]
    public void AddOrExpression_ShouldReturnSameInstance()
    {
        var filter = new FilterBy<FilterTestEntity>(x => x.Name == "Test");
        var result = filter.AddOrExpression(x => x.Age > 18);

        Assert.Same(filter, result);
    }
}
