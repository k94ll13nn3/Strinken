using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Strinken.Tests
{
    [TestFixture]
    public class ParserTests
    {
        private Parser<Data> stringSolver;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter());
        }

        [Test]
        public void Tags_Get_ReturnsReadOnlyCollection()
        {
            Assert.That(this.stringSolver.Tags as List<ITag<Data>>, Is.Null);
            Assert.That(this.stringSolver.Tags as ReadOnlyCollection<ITag<Data>>, Is.Not.Null);
        }

        [Test]
        public void Tags_Get_ReturnsOneTag()
        {
            Assert.That(this.stringSolver.Tags, Has.Count.EqualTo(1));
            Assert.That(this.stringSolver.Tags.First().Name, Is.EqualTo("DataName"));
        }

        [Test]
        public void Filters_Get_ReturnsReadOnlyCollection()
        {
            Assert.That(this.stringSolver.Filters as List<IFilter>, Is.Null);
            Assert.That(this.stringSolver.Filters as ReadOnlyCollection<IFilter>, Is.Not.Null);
        }

        [Test]
        public void Filters_Get_ReturnsSevenFilters()
        {
            Assert.That(this.stringSolver.Filters, Has.Count.EqualTo(7));
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
            Assert.That(validationResult.Message, Is.EqualTo("The input is not correctly formatted (Empty filter)."));
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
        public void Resolve_OneTag_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }

        [Test]
        public void Resolve_OneTagAndOneFilter_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName:Upper} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The LOREM is in the kitchen."));
        }

        [Test]
        public void Resolve_OneTagAndOneFilterAndOneArgument_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The 5 is in the kitchen."));
        }

        [Test]
        public void Resolve_OneTagAndTwoFilters_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName:Append+One,Two:Length} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The 11 is in the kitchen."));
        }

        [Test]
        public void Resolve_OneTagAndOneFilterWithTagAsArgument_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName:Append+=DataName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The LoremLorem is in the kitchen."));
            Assert.That(stringSolver.Resolve("The {DataName:Append+DataName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The LoremDataName is in the kitchen."));
        }

        [Test]
        public void Constructor_TwoTagWithSameName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<Data>().WithTag(new DataNameTag()).WithTag(new DataNameTag()),
                Throws.ArgumentException.With.Message.EqualTo("DataName was already registered in the tag list."));
        }

        [Test]
        public void Constructor_TwoFilterWithSameName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<Data>().WithTag(new DataNameTag()).WithFilters(new IFilter[] { new AppendFilter(), new AppendFilter() }),
                Throws.ArgumentException.With.Message.EqualTo("Append was already registered in the filter list."));
        }

        [Test]
        public void Constructor_TagWithEmptyName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithTag("", "", s => s),
                Throws.ArgumentException.With.Message.EqualTo("A name cannot be empty."));
            Assert.That(
                () => new Parser<string>().WithTag(new EmptyNameTag()),
                Throws.ArgumentException.With.Message.EqualTo("A name cannot be empty."));
        }

        [Test]
        public void Constructor_FilterWithEmptyName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new EmptyNameFilter()),
                Throws.ArgumentException.With.Message.EqualTo("A name cannot be empty."));
        }

        [Test]
        public void Constructor_TagWithInvalidName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithTag("st*r", "", s => s),
                Throws.ArgumentException.With.Message.EqualTo("* is an invalid character for a name."));
            Assert.That(
                () => new Parser<string>().WithTag(new InvalidNameTag()),
                Throws.ArgumentException.With.Message.EqualTo("$ is an invalid character for a name."));
        }

        [Test]
        public void Constructor_FilterWithInvalidName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new InvalidNameFilter()),
                Throws.ArgumentException.With.Message.EqualTo("! is an invalid character for a name."));
        }

        [Test]
        public void WithTag_OnTheFlyCreation_ReturnsResolvedString()
        {
            var solver = new Parser<Data>().WithTag("OTF", "OTF", a => a.Name);

            Assert.That(solver.Tags, Has.Count.EqualTo(1));
            Assert.That(solver.Tags.First().Name, Is.EqualTo("OTF"));
            Assert.That(solver.Resolve("The {OTF} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }
    }
}