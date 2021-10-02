namespace Strinken.Public.Tests.Parser;

public class ValidateTests
{
    [Fact]
    public void Validate_AllFiltersKnown_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:Upper} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_AllFiltersKnownWithAlternativeName_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new SomeFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:!*} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_AllFiltersWithGoodArguments_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:Length:Zeros+6} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_AllTagsKnown_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_AllParameterTagsKnown_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {!Blue} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_AllFiltersKnownWithNameAndAlternativeName_ReturnsTrue()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new SomeFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:!*:Null+Ok} is in the kitchen.");

        validationResult.IsValid.Should().BeTrue();
        validationResult.Message.Should().BeEmpty();
    }

    [Fact]
    public void Validate_FilterWithTooManyArguments_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:Length+Arg} is in the kitchen.");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("Length does not have valid arguments.");
    }

    [Fact]
    public void Validate_FilterWithTooFewArguments_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:Zeros} is in the kitchen.");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("Zeros does not have valid arguments.");
    }

    [Fact]
    public void Validate_InvalidInput_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:} is in the kitchen.");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("Empty filter");
    }

    [Fact]
    public void Validate_UnknownFilter_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:Bryan} is in the kitchen.");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("Bryan is not a valid filter.");
    }

    [Fact]
    public void Validate_UnknownAlternativeNameForAFilter_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:!*$} is in the kitchen.");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("!*$ is not a valid filter.");
    }

    [Fact]
    public void Validate_UnknownTag_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {SomeTag}).");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("SomeTag is not a valid tag.");
    }

    [Fact]
    public void Validate_UnknownParameterTag_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {!SomeTag}).");

        validationResult.IsValid.Should().BeFalse();
        validationResult.Message.Should().Be("SomeTag is not a valid parameter tag.");
    }
}
