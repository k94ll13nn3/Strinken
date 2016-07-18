using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class IfEqualFilterTests
    {
        [Test]
        public void Resolve_IsEqual_ReturnsData()
        {
            var filter = new IfEqualFilter();

            Assert.That(filter.Resolve("data", new string[] { "data", "true", "false" }), Is.EqualTo("true"));
        }

        [Test]
        public void Resolve_IsNotEqual_ReturnsArgument()
        {
            var filter = new IfEqualFilter();

            Assert.That(filter.Resolve("value", new string[] { "data", "true", "false" }), Is.EqualTo("false"));
        }

        [Test]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new IfEqualFilter();

            Assert.That(filter.Validate(null), Is.False);
            Assert.That(filter.Validate(new string[] { }), Is.False);
        }

        [Test]
        public void Validate_OneArgument_ReturnsFalse()
        {
            var filter = new IfEqualFilter();

            Assert.That(filter.Validate(new string[] { "" }), Is.False);
        }

        [Test]
        public void Validate_ThreeArguments_ReturnsTrue()
        {
            var filter = new IfEqualFilter();

            Assert.That(filter.Validate(new string[] { "", "", "" }), Is.True);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).Build();
            Assert.That(stringSolver.Resolve("The {DataName:IfEqual+Lorem,T,F} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The T is in the kitchen."));
            Assert.That(stringSolver.Resolve("The {DataName:IfEqual+Ipsum,T,F} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The F is in the kitchen."));
        }
    }
}