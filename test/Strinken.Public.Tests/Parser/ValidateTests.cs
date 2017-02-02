using FluentAssertions;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Parser
{
    public class ValidateTests
    {
        [StrinkenTest]
        public void Validate_AllFiltersKnown_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName:Upper} is in the kitchen.");

            validationResult.IsValid.Should().BeTrue();
            validationResult.Message.Should().BeNull();
        }

        [StrinkenTest]
        public void Validate_AllFiltersWithGoodArguments_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName:Length} is in the kitchen.");

            validationResult.IsValid.Should().BeTrue();
            validationResult.Message.Should().BeNull();
        }

        [StrinkenTest]
        public void Validate_AllTagsKnown_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen.");

            validationResult.IsValid.Should().BeTrue();
            validationResult.Message.Should().BeNull();
        }

        [StrinkenTest]
        public void Validate_AllParameterTagsKnown_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {!Blue} is in the kitchen.");

            validationResult.IsValid.Should().BeTrue();
            validationResult.Message.Should().BeNull();
        }

        [StrinkenTest]
        public void Validate_FilterWithWrongArguments_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName:Length+Arg} is in the kitchen.");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Message.Should().Be("Length does not have valid arguments.");
        }

        [StrinkenTest]
        public void Validate_InvalidInput_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName:} is in the kitchen.");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Message.Should().Be("Empty filter");
        }

        [StrinkenTest]
        public void Validate_UnknownFilter_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName:Bryan} is in the kitchen.");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Message.Should().Be("Bryan is not a valid filter.");
        }

        [StrinkenTest]
        public void Validate_UnknownTag_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {SomeTag}).");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Message.Should().Be("SomeTag is not a valid tag.");
        }

        [StrinkenTest]
        public void Validate_UnknownParameterTag_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {!SomeTag}).");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Message.Should().Be("SomeTag is not a valid parameter tag.");
        }
    }
}