using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class ReapeatFilterTests
    {
        [Fact]
        public void Resolve_Data_ReturnsDataWithLeadingZeros()
        {
            var filter = new ReapeatFilter();

            filter.Resolve("some string", new string[] { "4" }).Should().Be("some stringsome stringsome stringsome string");
            filter.Resolve("", new string[] { "4" }).Should().Be("");
            filter.Resolve("string", new string[] { "1" }).Should().Be("string");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new ReapeatFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(new string[] { }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneArgumentButNotInt_ReturnsFalse()
        {
            var filter = new ReapeatFilter();

            filter.Validate(new string[] { "t" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_MoreThanOneArgument_ReturnsFalse()
        {
            var filter = new ReapeatFilter();

            filter.Validate(new string[] { "", "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneIntArgument_ReturnsTrue()
        {
            var filter = new ReapeatFilter();

            filter.Validate(new string[] { "3" }).Should().BeTrue();
        }

        [Fact]
        public void Resolve_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Repeat+3} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremLoremLorem is in the kitchen.");
        }

        [Fact]
        public void Validate_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Validate("The {DataName:Repeat+3} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}
