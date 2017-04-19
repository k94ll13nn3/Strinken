using System;
using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Filters
{
    public class FilterHelpersTests
    {
        [Fact]
        public void Validate_FilterRegistered_ReturnsTrue()
        {
            FilterHelpers.Register(new FilterGenerator("ROne"));
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:ROne} is in the kitchen.");
            validationResult.IsValid.Should().BeTrue();

            FilterHelpers.Unregister("ROne");
        }

        [Fact]
        public void Validate_FilterRegisteredAfterValidation_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            FilterHelpers.Register(new FilterGenerator("RTwo"));
            var validationResult = stringSolver.Validate("The {DataName:RTwo} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();

            FilterHelpers.Unregister("RTwo");
        }

        [Fact]
        public void Validate_FilterRegisteredAndThenUnRegistered_ReturnsTrueAndThenFalse()
        {
            FilterHelpers.Register(new FilterGenerator("RThree"));
            FilterHelpers.Unregister("RThree");
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:RThree} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Register_FilterWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => FilterHelpers.Register(new EmptyNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [Fact]
        public void Register_FilterWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => FilterHelpers.Register(new InvalidNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("! is an invalid character for a name.");
        }

        [Fact]
        public void Register_FilterWithNameAlreadyRegistered_ThrowsArgumentException()
        {
            Action act = () => FilterHelpers.Register(new FilterGenerator("Length"));

            act.ShouldThrow<ArgumentException>().WithMessage("Length was already registered in the filter list.");
        }

        private class FilterGenerator : IFilter
        {
            private readonly string data;

            public FilterGenerator(string data)
            {
                this.data = data;
            }

            public string Description => data;

            public string Name => data;

            public string Usage => "";

            public string Resolve(string value, string[] arguments) => value;

            public bool Validate(string[] arguments) => true;
        }
    }
}