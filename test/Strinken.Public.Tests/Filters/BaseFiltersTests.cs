using System;
using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Filters
{
    public class BaseFiltersTests
    {
        [Fact]
        public void Validate_FilterRegistered_ReturnsTrue()
        {
            BaseFilters.Register(new FilterGenerator("ROne"));
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:ROne} is in the kitchen.");
            validationResult.IsValid.Should().BeTrue();

            BaseFilters.Unregister("ROne");
        }

        [Fact]
        public void Validate_FilterRegisteredAfterValidation_ReturnsFalse()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            BaseFilters.Register(new FilterGenerator("RTwo"));
            var validationResult = stringSolver.Validate("The {DataName:RTwo} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();

            BaseFilters.Unregister("RTwo");
        }

        [Fact]
        public void Validate_FilterRegisteredAndThenUnRegistered_ReturnsTrueAndThenFalse()
        {
            BaseFilters.Register(new FilterGenerator("RThree"));
            BaseFilters.Unregister("RThree");
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:RThree} is in the kitchen.");
            validationResult.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Register_FilterWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => BaseFilters.Register(new EmptyNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [Fact]
        public void Register_FilterWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => BaseFilters.Register(new InvalidNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("! is an invalid character for a name.");
        }

        [Fact]
        public void Register_FilterWithNameAlreadyRegistered_ThrowsArgumentException()
        {
            Action act = () => BaseFilters.Register(new FilterGenerator("Length"));

            act.ShouldThrow<ArgumentException>().WithMessage("Length was already registered in the filter list.");
        }

        [Fact]
        public void Register_FilterNull_ThrowsArgumentNullException()
        {
            Action act = () => BaseFilters.Register(null);

            act.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName == "filter");
        }

        private class FilterGenerator : IFilter
        {
            public FilterGenerator(string data)
            {
                Name = data;
            }

            public string Description => Name;

            public string Name { get; }

            public string Usage => "";

            public string Resolve(string value, string[] arguments) => value;

            public bool Validate(string[] arguments) => true;
        }
    }
}