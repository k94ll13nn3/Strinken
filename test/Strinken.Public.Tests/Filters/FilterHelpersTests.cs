using NUnit.Framework;
using Strinken.Filters;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Filters
{
    [TestFixture]
    public class FilterHelpersTests
    {
        [Test]
        public void Validate_FilterRegistered_ReturnsTrue()
        {
            FilterHelpers.Register(new RegisterFilter());
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:Register} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.True);

            FilterHelpers.UnRegister(new RegisterFilter());
        }

        [Test]
        public void Validate_FilterRegisteredAfterValidation_ReturnsFalse()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            FilterHelpers.Register(new RegisterFilter());
            var validationResult = stringSolver.Validate("The {DataName:Register} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.False);

            FilterHelpers.UnRegister(new RegisterFilter());
        }

        [Test]
        public void Validate_FilterRegisteredAndThenUnRegistered_ReturnsTrueAndThenFalse()
        {
            FilterHelpers.Register(new RegisterFilter());
            FilterHelpers.UnRegister(new RegisterFilter());
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            var validationResult = stringSolver.Validate("The {DataName:Register} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.False);
        }
    }
}