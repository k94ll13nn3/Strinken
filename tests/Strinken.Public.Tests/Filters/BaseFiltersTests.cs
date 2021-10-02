namespace Strinken.Public.Tests.Filters;

public class BaseFiltersTests
{
    [Fact]
    public void Validate_FilterRegistered_ReturnsTrue()
    {
        BaseFilters.Register(new FilterGenerator("ROne"));
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:ROne} is in the kitchen.");
        validationResult.IsValid.Should().BeTrue();

        BaseFilters.Unregister("ROne");
    }

    [Fact]
    public void Validate_FilterRegisteredAfterValidation_ReturnsFalse()
    {
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        BaseFilters.Register(new FilterGenerator("RTwo"));
        ValidationResult validationResult = stringSolver.Validate("The {DataName:RTwo} is in the kitchen.");
        validationResult.IsValid.Should().BeFalse();

        BaseFilters.Unregister("RTwo");
    }

    [Fact]
    public void Validate_FilterRegisteredAndThenUnRegistered_ReturnsTrueAndThenFalse()
    {
        BaseFilters.Register(new FilterGenerator("RThree"));
        BaseFilters.Unregister("RThree");
        Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
        ValidationResult validationResult = stringSolver.Validate("The {DataName:RThree} is in the kitchen.");
        validationResult.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Register_FilterWithEmptyName_ThrowsArgumentException()
    {
        Action act = () => BaseFilters.Register(new EmptyNameFilter());

        act.Should().Throw<ArgumentException>().WithMessage("A name cannot be empty.");
    }

    [Fact]
    public void Register_FilterWithInvalidName_ThrowsArgumentException()
    {
        Action act = () => BaseFilters.Register(new InvalidNameFilter());

        act.Should().Throw<ArgumentException>().WithMessage("! is an invalid character for a name.");
    }

    [Fact]
    public void Register_FilterWithNameAlreadyRegistered_ThrowsArgumentException()
    {
        Action act = () => BaseFilters.Register(new FilterGenerator("Length"));

        act.Should().Throw<ArgumentException>().WithMessage("Length was already registered in the base filter list.");
    }

    [Fact]
    public void Register_FilterNull_ThrowsArgumentNullException()
    {
        Action act = () => BaseFilters.Register(null!);

        act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "filter");
    }

    [Fact]
    public void Register_FilterWithAlternativeNameAlreadyRegistered_ThrowsArgumentException()
    {
        BaseFilters.Register(new FilterGenerator("BAOne", "!**"));
        Action act = () => BaseFilters.Register(new FilterGenerator("BARTwo", "!**"));

        act.Should().Throw<ArgumentException>().WithMessage("A base filter already has !** as its alternative name.");
    }

    [Fact]
    public void Register_FilterWithInvalidAlternativeName_ThrowsArgumentException()
    {
        Action act = () => BaseFilters.Register(new InvalidAlternativeNameFilter());

        act.Should().Throw<ArgumentException>().WithMessage("n is an invalid character for an alternative name.");
    }

    private class FilterGenerator : IFilter
    {
        public FilterGenerator(string name)
            : this(name, string.Empty)
        {
        }

        public FilterGenerator(string name, string alternativeName)
        {
            Name = name;
            AlternativeName = alternativeName;
        }

        public string Description => Name;

        public string Name { get; }

        public string Usage => "";

        public string AlternativeName { get; }

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }
}
