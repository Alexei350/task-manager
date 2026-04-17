using TaskManager.Models.Base.Dynamic;
using Xunit;

namespace TaskManager.UnitTests.ModelTests.Base;

public class DynamicTests
{
    [Fact]
    public void DynamicFilter_Should_SetProperties()
    {
        var filter = new DynamicFilter
        {
            Connector = "AND",
            Values = new List<DynamicFilterItem>
            {
                new() { Operation = "Equals", PropertyName = "Name", Value = "Test" }
            }
        };

        Assert.Equal("AND", filter.Connector);
        Assert.NotNull(filter.Values);
        Assert.Single(filter.Values);
    }

    [Fact]
    public void DynamicFilterItem_Should_SetProperties()
    {
        var item = new DynamicFilterItem
        {
            Operation = "Contains",
            PropertyName = "Description",
            Value = "test"
        };

        Assert.Equal("Contains", item.Operation);
        Assert.Equal("Description", item.PropertyName);
        Assert.Equal("test", item.Value);
    }

    [Fact]
    public void DynamicOrderBy_Should_SetProperties()
    {
        var orderBy = new DynamicOrderBy
        {
            Fields = new List<DynamicOrderByItem>
            {
                new() { PropertyName = "Name", Direction = "ASC" }
            }
        };

        Assert.NotNull(orderBy.Fields);
        Assert.Single(orderBy.Fields);
    }

    [Fact]
    public void DynamicOrderByItem_Should_SetProperties()
    {
        var item = new DynamicOrderByItem
        {
            PropertyName = "CreatedAt",
            Direction = "DESC"
        };

        Assert.Equal("CreatedAt", item.PropertyName);
        Assert.Equal("DESC", item.Direction);
    }

    [Fact]
    public void DynamicFilter_Should_SupportMultipleFilterItems()
    {
        var filter = new DynamicFilter
        {
            Connector = "OR",
            Values = new List<DynamicFilterItem>
            {
                new() { Operation = "GreaterThanOrEquals", PropertyName = "Age", Value = "18" },
                new() { Operation = "LessThanOrEquals", PropertyName = "Age", Value = "65" }
            }
        };

        Assert.Equal("OR", filter.Connector);
        Assert.Equal(2, filter.Values.Count);
        Assert.Equal("GreaterThanOrEquals", filter.Values[0].Operation);
        Assert.Equal("LessThanOrEquals", filter.Values[1].Operation);
    }
}
