using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class LowerFilterTests
    {
        [Fact]
        public void Resolve_Data_ReturnsDataToLowerCase()
        {
            var filter = new LowerFilter();

            filter.Resolve("DAta", null).Should().Be("data");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new LowerFilter();

            filter.Validate(null).Should().BeTrue();
            filter.Validate(new string[] { }).Should().BeTrue();
        }

        [Fact]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new LowerFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Resolve_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Lower} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The lorem is in the kitchen.");
        }

        [Fact]
        public void Validate_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Validate("The {DataName:Lower} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}