using FluentAssertions;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    public class ReplaceFilterTests
    {
        [StrinkenTest]
        public void Resolve_IsEqual_ReturnsStringWithReplacements()
        {
            var filter = new ReplaceFilter();

            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol" }).Should().Be("Merol ipsum");
            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "ipsum", "-----" }).Should().Be("Merol -----");
            filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "Merol", "-----" }).Should().Be("----- ipsum");
        }

        [StrinkenTest]
        public void Validate_NoArgumentsOrOddNumberOfArguments_ReturnsFalse()
        {
            var filter = new ReplaceFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(new string[] { }).Should().BeFalse();
            filter.Validate(new string[] { "" }).Should().BeFalse();
            filter.Validate(new string[] { "", "", "" }).Should().BeFalse();
        }

        [StrinkenTest]
        public void Validate_EvenNumberOfArguments_ReturnsTrue()
        {
            var filter = new ReplaceFilter();

            filter.Validate(new string[] { "", "" }).Should().BeTrue();
            filter.Validate(new string[] { "", "", "", "" }).Should().BeTrue();
        }

        [StrinkenTest]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Replace+Lorem,Merol} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Merol is in the kitchen.");
        }
    }
}