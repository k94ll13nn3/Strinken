using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    public class LowerFilterTests
    {
        [StrinkenTest]
        public void Resolve_Data_ReturnsDataToLowerCase()
        {
            var filter = new LowerFilter();

            filter.Resolve("DAta", null).Should().Be("data");
        }

        [StrinkenTest]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new LowerFilter();

            filter.Validate(null).Should().BeTrue();
            filter.Validate(new string[] { }).Should().BeTrue();
        }

        [StrinkenTest]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new LowerFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "" }).Should().BeFalse();
        }

        [StrinkenTest]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Lower} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The lorem is in the kitchen.");
        }
    }
}