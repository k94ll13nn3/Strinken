using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class LeadingZerosFilterTests
    {
        [Fact]
        public void Resolve_Data_ReturnsDataWithLeadingZeros()
        {
            var filter = new LeadingZerosFilter();

            filter.Resolve("3", new string[] { "4" }).Should().Be("0003");
            filter.Resolve("3000", new string[] { "4" }).Should().Be("3000");
            filter.Resolve("30000", new string[] { "4" }).Should().Be("30000");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(new string[] { }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneArgumentButNotInt_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            filter.Validate(new string[] { "t" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_MoreThanOneArgument_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            filter.Validate(new string[] { "", "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneIntArgument_ReturnsTrue()
        {
            var filter = new LeadingZerosFilter();

            filter.Validate(new string[] { "3" }).Should().BeTrue();
        }

        [Fact]
        public void Resolve_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Length:Zeros+3} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 005 is in the kitchen.");
        }

        [Fact]
        public void Validate_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Validate("The {DataName:Length:Zeros+3} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}