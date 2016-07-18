using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.FiltersTests
{
    [TestFixture]
    public class LeadingZerosFilterTests
    {
        [Test]
        public void Resolve_Data_ReturnsDataWithLeadingZeros()
        {
            var filter = new LeadingZerosFilter();

            Assert.That(filter.Resolve("3", new string[] { "4" }), Is.EqualTo("0003"));
            Assert.That(filter.Resolve("3000", new string[] { "4" }), Is.EqualTo("3000"));
            Assert.That(filter.Resolve("30000", new string[] { "4" }), Is.EqualTo("30000"));
        }

        [Test]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            Assert.That(filter.Validate(null), Is.False);
            Assert.That(filter.Validate(new string[] { }), Is.False);
        }

        [Test]
        public void Validate_OneArgumentButNotInt_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            Assert.That(filter.Validate(new string[] { "t" }), Is.False);
        }

        [Test]
        public void Validate_MoreThanOneArgument_ReturnsFalse()
        {
            var filter = new LeadingZerosFilter();

            Assert.That(filter.Validate(new string[] { "", "", "" }), Is.False);
        }

        [Test]
        public void Validate_OneIntArgument_ReturnsTrue()
        {
            var filter = new LeadingZerosFilter();

            Assert.That(filter.Validate(new string[] { "3" }), Is.True);
        }

        [Test]
        public void Resolve__ReturnsResolvedString()
        {
            var stringSolver = ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).Build();
            Assert.That(stringSolver.Resolve("The {DataName:Length:Zeros+3} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The 005 is in the kitchen."));
        }
    }
}