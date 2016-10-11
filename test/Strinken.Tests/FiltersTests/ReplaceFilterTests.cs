using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class ReplaceFilterTests
    {
        [Test]
        public void Resolve_IsEqual_ReturnsStringWithReplacements()
        {
            var filter = new ReplaceFilter();

            Assert.That(filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol" }), Is.EqualTo("Merol ipsum"));
            Assert.That(filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "ipsum", "-----" }), Is.EqualTo("Merol -----"));
            Assert.That(filter.Resolve("lorem ipsum", new string[] { "lorem", "Merol", "Merol", "-----" }), Is.EqualTo("----- ipsum"));
        }

        [Test]
        public void Validate_NoArgumentsOrOddNumberOfArguments_ReturnsFalse()
        {
            var filter = new ReplaceFilter();

            Assert.That(filter.Validate(null), Is.False);
            Assert.That(filter.Validate(new string[] { }), Is.False);
            Assert.That(filter.Validate(new string[] { "" }), Is.False);
            Assert.That(filter.Validate(new string[] { "", "", "" }), Is.False);
        }

        [Test]
        public void Validate_EvenNumberOfArguments_ReturnsTrue()
        {
            var filter = new ReplaceFilter();

            Assert.That(filter.Validate(new string[] { "", "" }), Is.True);
            Assert.That(filter.Validate(new string[] { "", "", "", "" }), Is.True);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            Assert.That(stringSolver.Resolve("The {DataName:Replace+Lorem,Merol} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Merol is in the kitchen."));
        }
    }
}