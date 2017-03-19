using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class IfEqualFilterTests
    {
        [Fact]
        public void Resolve_IsEqual_ReturnsData()
        {
            var filter = new IfEqualFilter();

            filter.Resolve("data", new string[] { "data", "true", "false" }).Should().Be("true");
        }

        [Fact]
        public void Resolve_IsNotEqual_ReturnsArgument()
        {
            var filter = new IfEqualFilter();

            filter.Resolve("value", new string[] { "data", "true", "false" }).Should().Be("false");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new IfEqualFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(new string[] { }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneArgument_ReturnsFalse()
        {
            var filter = new IfEqualFilter();

            filter.Validate(new string[] { "" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_ThreeArguments_ReturnsTrue()
        {
            var filter = new IfEqualFilter();

            filter.Validate(new string[] { "", "", "" }).Should().BeTrue();
        }

        [Fact]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:IfEqual+Lorem,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The T is in the kitchen.");
            stringSolver.Resolve("The {DataName:IfEqual+Ipsum,T,F} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The F is in the kitchen.");
        }
    }
}