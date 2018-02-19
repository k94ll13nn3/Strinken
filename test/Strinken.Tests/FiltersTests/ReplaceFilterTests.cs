using FluentAssertions;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class ReplaceFilterTests
    {
        [Fact]
        public void Resolve_IsEqual_ReturnsStringWithReplacements()
        {
            var filter = new ReplaceFilter();

            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol" }).Should().Be("Merol ipsum");
            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "ipsum", "-----" }).Should().Be("Merol -----");
            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "Merol", "-----" }).Should().Be("----- ipsum");
        }

        [Fact]
        public void Validate_NoArgumentsOrOddNumberOfArguments_ReturnsFalse()
        {
            var filter = new ReplaceFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(new string[] { }).Should().BeFalse();
            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_EvenNumberOfArguments_ReturnsTrue()
        {
            var filter = new ReplaceFilter();

            filter.Validate(new string[] { "", "" }).Should().BeTrue();
            filter.Validate(new string[] { "", "", "", "" }).Should().BeTrue();
        }

        [Fact]
        public void Resolve_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Replace+Lorem,Merol} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Merol is in the kitchen.");
        }

        [Fact]
        public void Validate_ReturnsTrue()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Validate("The {DataName:Replace+Lorem,Merol} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}