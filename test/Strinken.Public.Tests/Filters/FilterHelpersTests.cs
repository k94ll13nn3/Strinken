using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Filters
{
    public class FilterHelpersTests
    {
        [StrinkenTest]
        public void Validate_FilterRegistered_ReturnsTrue()
        {
            FilterHelpers.Register(new RegisterFilter());
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:ROne} is in the kitchen.");
            validationResult.IsValid.Should().BeTrue();

            FilterHelpers.UnRegister(new RegisterFilter());
        }

        [StrinkenTest]
        public void Validate_FilterRegisteredAfterValidation_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            FilterHelpers.Register(new Register2Filter());
            var validationResult = stringSolver.Validate("The {DataName:RTwo} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();

            FilterHelpers.UnRegister(new Register2Filter());
        }

        [StrinkenTest]
        public void Validate_FilterRegisteredAndThenUnRegistered_ReturnsTrueAndThenFalse()
        {
            FilterHelpers.Register(new Register3Filter());
            FilterHelpers.UnRegister(new Register3Filter());
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:RThree} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();
        }

        private class RegisterFilter : IFilter
        {
            public string Description => "ROne";

            public string Name => "ROne";

            public string Usage => "";

            public string Resolve(string value, string[] arguments) => value;

            public bool Validate(string[] arguments) => true;
        }

        private class Register2Filter : IFilter
        {
            public string Description => "RTwo";

            public string Name => "RTwo";

            public string Usage => "";

            public string Resolve(string value, string[] arguments) => value;

            public bool Validate(string[] arguments) => true;
        }

        private class Register3Filter : IFilter
        {
            public string Description => "RThree";

            public string Name => "RThree";

            public string Usage => "";

            public string Resolve(string value, string[] arguments) => value;

            public bool Validate(string[] arguments) => true;
        }
    }
}