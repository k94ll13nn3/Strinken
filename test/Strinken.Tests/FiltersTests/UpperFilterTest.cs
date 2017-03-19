using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class UpperFilterTest
    {
        [Fact]
        public void Resolve_Data_ReturnsDataToUpperCase()
        {
            var filter = new UpperFilter();

            filter.Resolve("data", null).Should().Be("DATA");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new UpperFilter();

            filter.Validate(null).Should().BeTrue();
            filter.Validate(new string[] { }).Should().BeTrue();
        }

        [Fact]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new UpperFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Upper} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LOREM is in the kitchen.");
        }
    }
}