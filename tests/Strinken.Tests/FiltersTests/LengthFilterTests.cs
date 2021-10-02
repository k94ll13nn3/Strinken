namespace Strinken.Tests.FiltersTests;

public class LengthFilterTests
{
    [Fact]
    public void Resolve_Data_ReturnsDataLength()
    {
        var filter = new LengthFilter();

        filter.Resolve("DAta", Array.Empty<string>()).Should().Be("4");
    }

    [Fact]
    public void Validate_NoArguments_ReturnsTrue()
    {
        var filter = new LengthFilter();

        filter.Validate(null!).Should().BeTrue();
        filter.Validate(Array.Empty<string>()).Should().BeTrue();
    }

    [Fact]
    public void Validate_OneOrMoreArguments_ReturnsFalse()
    {
        var filter = new LengthFilter();

        filter.Validate(new string[] { "" }).Should().BeFalse();
        filter.Validate(new string[] { "", "" }).Should().BeFalse();
    }

    [Fact]
    public void Resolve_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 5 is in the kitchen.");
    }

    [Fact]
    public void Validate_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Validate("The {DataName:Length} is in the kitchen.").IsValid.Should().BeTrue();
    }

    [Fact]
    public void Resolve_NullString_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag("Null", string.Empty, _ => null!);
        stringSolver.Resolve("The {Null:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 0 is in the kitchen.");
    }
}
