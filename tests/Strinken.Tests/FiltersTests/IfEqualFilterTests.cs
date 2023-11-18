namespace Strinken.Tests.FiltersTests;

public class IfEqualFilterTests
{
    [Fact]
    public void Resolve_IsEqual_ReturnsData()
    {
        var filter = new IfEqualFilter();

        filter.Resolve("data", ["data", "true", "false"]).Should().Be("true");
    }

    [Fact]
    public void Resolve_IsNotEqual_ReturnsArgument()
    {
        var filter = new IfEqualFilter();

        filter.Resolve("value", ["data", "true", "false"]).Should().Be("false");
    }

    [Fact]
    public void Validate_NoArguments_ReturnsFalse()
    {
        var filter = new IfEqualFilter();

        filter.Validate(null!).Should().BeFalse();
        filter.Validate([]).Should().BeFalse();
    }

    [Fact]
    public void Validate_OneArgument_ReturnsFalse()
    {
        var filter = new IfEqualFilter();

        filter.Validate([""]).Should().BeFalse();
    }

    [Fact]
    public void Validate_ThreeArguments_ReturnsTrue()
    {
        var filter = new IfEqualFilter();

        filter.Validate(["", "", ""]).Should().BeTrue();
    }

    [Fact]
    public void Resolve_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Resolve("The {DataName:IfEqual+Lorem,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The T is in the kitchen.");
        stringSolver.Resolve("The {DataName:IfEqual+Ipsum,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The F is in the kitchen.");
    }

    [Fact]
    public void Resolve_WithAlternativeName_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Resolve("The {DataName:?+Lorem,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The T is in the kitchen.");
        stringSolver.Resolve("The {DataName:?+Ipsum,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The F is in the kitchen.");
    }

    [Fact]
    public void Validate_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Validate("The {DataName:IfEqual+Lorem,T,F} is in the kitchen.").IsValid.Should().BeTrue();
        stringSolver.Validate("The {DataName:IfEqual+Ipsum,T,F} is in the kitchen.").IsValid.Should().BeTrue();
    }
}
