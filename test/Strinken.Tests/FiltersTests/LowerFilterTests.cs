using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class LowerFilterTests
    {
        [Test]
        public void Resolve_Data_ReturnsDataToLowerCase()
        {
            var filter = new LowerFilter();

            Assert.That(filter.Resolve("DAta", null), Is.EqualTo("data"));
        }

        [Test]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new LowerFilter();

            Assert.That(filter.Validate(null), Is.True);
            Assert.That(filter.Validate(new string[] { }), Is.True);
        }

        [Test]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new LowerFilter();

            Assert.That(filter.Validate(new string[] { "" }), Is.False);
            Assert.That(filter.Validate(new string[] { "", "" }), Is.False);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            Assert.That(stringSolver.Resolve("The {DataName:Lower} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The lorem is in the kitchen."));
        }
    }
}