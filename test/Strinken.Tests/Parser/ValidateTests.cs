using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.Parser
{
    [TestFixture]
    public class ValidateTests
    {
        private Parser<Data> stringSolver;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new MachineNameParameterTag());
        }

        [Test]
        public void Validate_AllFiltersKnown_ReturnsTrue()
        {
            var validationResult = stringSolver.Validate("The {DataName:Upper} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Message, Is.Null);
        }

        [Test]
        public void Validate_AllFiltersWithGoodArguments_ReturnsTrue()
        {
            var validationResult = stringSolver.Validate("The {DataName:Length} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Message, Is.Null);
        }

        [Test]
        public void Validate_AllTagsKnown_ReturnsTrue()
        {
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Message, Is.Null);
        }

        [Test]
        public void Validate_AllParameterTagsKnown_ReturnsTrue()
        {
            var validationResult = stringSolver.Validate("The {!MachineName} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.True);
            Assert.That(validationResult.Message, Is.Null);
        }

        [Test]
        public void Validate_FilterWithWrongArguments_ReturnsFalse()
        {
            var validationResult = stringSolver.Validate("The {DataName:Length+Arg} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Length does not have valid arguments."));
        }

        [Test]
        public void Validate_InvalidInput_ReturnsFalse()
        {
            var validationResult = stringSolver.Validate("The {DataName:} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Empty filter"));
        }

        [Test]
        public void Validate_UnknownFilter_ReturnsFalse()
        {
            var validationResult = stringSolver.Validate("The {DataName:Bryan} is in the kitchen.");
            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("Bryan is not a valid filter."));
        }

        [Test]
        public void Validate_UnknownTag_ReturnsFalse()
        {
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {SomeTag}).");
            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("SomeTag is not a valid tag."));
        }

        [Test]
        public void Validate_UnknownParameterTag_ReturnsFalse()
        {
            var validationResult = stringSolver.Validate("The {DataName} is in the kitchen (size {!SomeTag}).");
            Assert.That(validationResult.IsValid, Is.False);
            Assert.That(validationResult.Message, Is.EqualTo("SomeTag is not a valid parameter tag."));
        }
    }
}