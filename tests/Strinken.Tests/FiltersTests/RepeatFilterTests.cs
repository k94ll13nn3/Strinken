namespace Strinken.Tests.FiltersTests;

public class RepeatFilterTests
{
    [Fact]
    public void Resolve_Data_ReturnsDataWithLeadingZeros()
    {
        var filter = new RepeatFilter();

        filter.Resolve("some string", ["4"]).Should().Be("some stringsome stringsome stringsome string");
        filter.Resolve("", ["4"]).Should().Be("");
        filter.Resolve("string", ["1"]).Should().Be("string");
    }

    [Fact]
    public void Validate_NoArguments_ReturnsFalse()
    {
        var filter = new RepeatFilter();

        filter.Validate(null!).Should().BeFalse();
        filter.Validate([]).Should().BeFalse();
    }

    [Fact]
    public void Validate_OneArgumentButNotInt_ReturnsFalse()
    {
        var filter = new RepeatFilter();

        filter.Validate(["t"]).Should().BeFalse();
    }

    [Fact]
    public void Validate_MoreThanOneArgument_ReturnsFalse()
    {
        var filter = new RepeatFilter();

        filter.Validate(["", "", ""]).Should().BeFalse();
    }

    [Fact]
    public void Validate_OneIntArgument_ReturnsTrue()
    {
        var filter = new RepeatFilter();

        filter.Validate(["3"]).Should().BeTrue();
    }

    [Fact]
    public void Resolve_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Resolve("The {DataName:Repeat+3} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremLoremLorem is in the kitchen.");
    }

    [Fact]
    public void Resolve_WithAlternativeName_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Resolve("The {DataName:*+3} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremLoremLorem is in the kitchen.");
    }

    [Fact]
    public void Validate_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        stringSolver.Validate("The {DataName:Repeat+3} is in the kitchen.").IsValid.Should().BeTrue();
    }

    [Fact]
    public void Resolve_NullString_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag("Null", string.Empty, _ => null!);
        stringSolver.Resolve("The {Null:Repeat+3} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The  is in the kitchen.");
    }
}
