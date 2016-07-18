using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class UpperFilterTest
    {
        [Test]
        public void Resolve_Data_ReturnsDataToUpperCase()
        {
            var filter = new UpperFilter();

            Assert.That(filter.Resolve("data", null), Is.EqualTo("DATA"));
        }

        [Test]
        public void Validate_NoArguments_ReturnsTrue()
        {
            var filter = new UpperFilter();

            Assert.That(filter.Validate(null), Is.True);
            Assert.That(filter.Validate(new string[] { }), Is.True);
        }

        [Test]
        public void Validate_OneOrMoreArguments_ReturnsFalse()
        {
            var filter = new UpperFilter();

            Assert.That(filter.Validate(new string[] { "" }), Is.False);
            Assert.That(filter.Validate(new string[] { "", "" }), Is.False);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).Build();
            Assert.That(stringSolver.Resolve("The {DataName:Upper} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The LOREM is in the kitchen."));
        }
    }
}