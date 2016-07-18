using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class NullFilterTests
    {
        [Test]
        public void Resolve_Data_ReturnsData()
        {
            var filter = new NullFilter();

            Assert.That(filter.Resolve("value", new string[] { "data" }), Is.EqualTo("value"));
        }

        [Test]
        public void Resolve_NullData_ReturnsArgument()
        {
            var filter = new NullFilter();

            Assert.That(filter.Resolve(null, new string[] { "data" }), Is.EqualTo("data"));
        }

        [Test]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new NullFilter();

            Assert.That(filter.Validate(null), Is.False);
            Assert.That(filter.Validate(new string[] { }), Is.False);
        }

        [Test]
        public void Validate_MoreThanOneArgument_ReturnsFalse()
        {
            var filter = new NullFilter();

            Assert.That(filter.Validate(new string[] { "", "", "" }), Is.False);
        }

        [Test]
        public void Validate_OneArgument_ReturnsTrue()
        {
            var filter = new NullFilter();

            Assert.That(filter.Validate(new string[] { "data" }), Is.True);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).Build();
            Assert.That(stringSolver.Resolve("The {DataName:Null+Ipsum} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
            Assert.That(stringSolver.Resolve("The {DataName:Null+Ipsum} is in the kitchen.", new Data { Name = null }), Is.EqualTo("The Ipsum is in the kitchen."));
        }
    }
}