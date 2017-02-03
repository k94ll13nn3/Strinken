using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    public class LengthFilterTests
    {
        [StrinkenTest]
        public void Resolve_Data_ReturnsDataLength()
        {
            var filter = new LengthFilter();

            filter.Resolve("DAta", null).Should().Be("4");
        }

        [StrinkenTest]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new LengthFilter();

            filter.Validate(null).Should().BeTrue();
            filter.Validate(new string[] { }).Should().BeTrue();
        }

        [StrinkenTest]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new LengthFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "" }).Should().BeFalse();
        }

        [StrinkenTest]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 5 is in the kitchen.");
        }
    }
}