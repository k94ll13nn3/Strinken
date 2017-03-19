using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class LengthFilterTests
    {
        [Fact]
        public void Resolve_Data_ReturnsDataLength()
        {
            var filter = new LengthFilter();

            filter.Resolve("DAta", null).Should().Be("4");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new LengthFilter();

            filter.Validate(null).Should().BeTrue();
            filter.Validate(new string[] { }).Should().BeTrue();
        }

        [Fact]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new LengthFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 5 is in the kitchen.");
        }
    }
}