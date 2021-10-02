using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Strinken.Public.Tests.Parser;

public class TokenTests
{
    [Fact]
    [SuppressMessage("Usage", "RCS1202:Avoid NullReferenceException.", Justification = "It is the goal of the test to test nullability")]
    public void Tags_Get_ReturnsReadOnlyCollection()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        (stringSolver.GetTags() as List<ITag<Data>>).Should().BeNull();
        (stringSolver.GetTags() as ReadOnlyCollection<ITag<Data>>).Should().NotBeNull();
    }

    [Fact]
    public void Tags_Get_ReturnsOneTag()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.GetTags().Should().HaveCount(1);
        stringSolver.GetTags().First().Name.Should().Be("DataName");
    }

    [Fact]
    [SuppressMessage("Usage", "RCS1202:Avoid NullReferenceException.", Justification = "It is the goal of the test to test nullability")]
    public void Filters_Get_ReturnsReadOnlyCollection()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        (stringSolver.GetFilters() as List<IFilter>).Should().BeNull();
        (stringSolver.GetFilters() as ReadOnlyCollection<IFilter>).Should().NotBeNull();
    }

    [Fact]
    public void Filters_GetWithoutBaseFilters_ReturnsOneFilter()
    {
        Parser<Data> stringSolver = new Parser<Data>(true).WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.GetFilters().Should().HaveCount(1);
    }

    [Fact]
    [SuppressMessage("Usage", "RCS1202:Avoid NullReferenceException.", Justification = "It is the goal of the test to test nullability")]
    public void ParameterTags_Get_ReturnsReadOnlyCollection()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        (stringSolver.GetParameterTags() as List<IParameterTag>).Should().BeNull();
        (stringSolver.GetParameterTags() as ReadOnlyCollection<IParameterTag>).Should().NotBeNull();
    }

    [Fact]
    public void ParameterTags_Get_ReturnsOneParameterTag()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.GetParameterTags().Should().HaveCount(1);
        stringSolver.GetParameterTags().First().Name.Should().Be("Blue");
    }
}
