namespace Strinken.Public.Tests.Parser;

public class ResolveTests
{
    [Fact]
    public void Resolve_OneTag_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneParameterTag_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {!Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Blue is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilter_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName:Upper} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LOREM is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilterAndOneArgument_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 5 is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndTwoFilters_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName:Append+One,Two:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 11 is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilterWithTagAsArgument_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName:Append+=DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremLorem is in the kitchen.");
        stringSolver.Resolve("The {DataName:Append+DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremDataName is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilterWithParameterTagAsArgument_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {DataName:Append+=!Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremBlue is in the kitchen.");
        stringSolver.Resolve("The {DataName:Append+Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremBlue is in the kitchen.");
    }

    [Fact]
    public void Resolve_InvalidString_ThrowsFormatException()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        Action act = () => stringSolver.Resolve("The {DataName:Append+} is in the kitchen.", new Data { Name = "Lorem" });

        act.Should().Throw<FormatException>().WithMessage("Empty argument");
    }

    [Fact]
    public void Resolve_OneValueTag_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("The {@som3th!ng$} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The som3th!ng$ is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneValueTagWithFilter_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        stringSolver.Resolve("{@som3th!ng$:Length}", new Data { Name = "Lorem" }).Should().Be("10");
    }

    [Fact]
    public void Resolve_OneNumberTagBinaryFormat_ReturnsResolvedString()
    {
        var stringSolver = new Parser<Data>();

        stringSolver.Resolve("{#b1101100}", new Data { Name = "Lorem" }).Should().Be("1101100");
    }

    [Fact]
    public void Resolve_OneNumberTagOctalFormat_ReturnsResolvedString()
    {
        var stringSolver = new Parser<Data>();

        stringSolver.Resolve("{#o4012546}", new Data { Name = "Lorem" }).Should().Be("4012546");
    }

    [Fact]
    public void Resolve_OneNumberTagDecimalFormat_ReturnsResolvedString()
    {
        var stringSolver = new Parser<Data>();

        stringSolver.Resolve("{#d487902654}", new Data { Name = "Lorem" }).Should().Be("487902654");
    }

    [Fact]
    public void Resolve_OneNumberTagHexadecimalFormat_ReturnsResolvedString()
    {
        var stringSolver = new Parser<Data>();

        stringSolver.Resolve("{#x787AEEF454601BB}", new Data { Name = "Lorem" }).Should().Be("787AEEF454601BB");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilterUsedWithItsAlternativeName_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter());

        stringSolver.Resolve("The {DataName:..+Ipsum} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremIpsum is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneBaseFilterUsedWithItsAlternativeName_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());

        stringSolver.Resolve("The {DataName:??+Ipsum} is in the kitchen.", new Data { Name = null! }).Should().Be("The Ipsum is in the kitchen.");
    }

    [Fact]
    public void Resolve_OneTagAndOneFilterUsedWithItsAlternativeNameAndOneFilterUsedWithItsName_ReturnsResolvedString()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());

        stringSolver.Resolve("The {DataName:??+Ipsum:Repeat+2} is in the kitchen.", new Data { Name = null! }).Should().Be("The IpsumIpsum is in the kitchen.");
    }

    [Fact]
    public void Resolve_Values_ReturnsResolvedStrings()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

        string[] expected = ["The 5 is in the kitchen.", "The 5 is in the kitchen.", "The 7 is in the kitchen."];
        Data[] data = [new Data { Name = "Lorem" }, new Data { Name = "Ipsum" }, new Data { Name = "Sanctum" }];
        stringSolver.Resolve("The {DataName:Length} is in the kitchen.", data).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Resolve_ValidStringWithNullValues_ThrowsArgumentNullException()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        Action act = () => stringSolver.Resolve("The {DataName:Append+} is in the kitchen.", (IEnumerable<Data>)null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "values");
    }

    [Fact]
    public void Resolve_InvalidStringWithValues_ThrowsFormatException()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        Action act = () => stringSolver.Resolve("The {DataName:Append+} is in the kitchen.", []);

        act.Should().Throw<FormatException>().WithMessage("Empty argument");
    }
}
