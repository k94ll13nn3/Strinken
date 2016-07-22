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
        private IParser<Data> stringSolver;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.stringSolver = ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).Build();
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
            Assert.That(stringSolver.Validate("The {DataName:Upper} is in the kitchen."), Is.True);
        }

        [Test]
        public void Validate_AllFiltersWithGoodArguments_ReturnsTrue()
        {
            Assert.That(stringSolver.Validate("The {DataName:Length} is in the kitchen."), Is.True);
        }

        [Test]
        public void Validate_AllTagsKnown_ReturnsTrue()
        {
            Assert.That(stringSolver.Validate("The {DataName} is in the kitchen."), Is.True);
        }

        [Test]
        public void Validate_FilterWithWrongArguments_ReturnsFalse()
        {
            Assert.That(stringSolver.Validate("The {DataName:Length+Arg} is in the kitchen."), Is.False);
        }

        [Test]
        public void Validate_InvalidInput_ReturnsFalse()
        {
            Assert.That(stringSolver.Validate("The {DataName:} is in the kitchen."), Is.False);
        }

        [Test]
        public void Validate_UnknownFilter_ReturnsFalse()
        {
            Assert.That(stringSolver.Validate("The {DataName:Bryan} is in the kitchen."), Is.False);
        }

        [Test]
        public void Validate_UnknownTag_ReturnsFalse()
        {
            Assert.That(stringSolver.Validate("The {DataName} is in the kitchen (size {SomeTag})."), Is.False);
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
                () => ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).WithTag(new DataNameTag()),
                Throws.ArgumentException.With.Message.EqualTo("DataName was already registered in the tag list."));
        }

        [Test]
        public void Constructor_TwoFilterWithSameName_ThrowsArgumentException()
        {
            Assert.That(
                () => ParserBuilder<Data>.Initialize().WithTag(new DataNameTag()).WithFilters(new IFilter[] { new AppendFilter(), new AppendFilter() }),
                Throws.ArgumentException.With.Message.EqualTo("Append was already registered in the filter list."));
        }

        [Test]
        public void Constructor_TagWithEmptyName_ThrowsArgumentException()
        {
            Assert.That(
                () => ParserBuilder<string>.Initialize().WithTag("", "", s => s),
                Throws.ArgumentException.With.Message.EqualTo("A tag cannot have an empty name."));
            Assert.That(
                () => ParserBuilder<string>.Initialize().WithTag(new EmptyNameTag()),
                Throws.ArgumentException.With.Message.EqualTo("A tag cannot have an empty name."));
        }

        [Test]
        public void Constructor_FilterWithEmptyName_ThrowsArgumentException()
        {
            Assert.That(
                () => ParserBuilder<string>.Initialize().WithTag("tag", "tag", s => s).WithFilter(new EmptyNameFilter()),
                Throws.ArgumentException.With.Message.EqualTo("A filter cannot have an empty name."));
        }

        [Test]
        public void WithTag_OnTheFlyCreation_ReturnsResolvedString()
        {
            var solver = ParserBuilder<Data>.Initialize().WithTag("OTF", "OTF", a => a.Name).Build();

            Assert.That(solver.Tags, Has.Count.EqualTo(1));
            Assert.That(solver.Tags.First().Name, Is.EqualTo("OTF"));
            Assert.That(solver.Resolve("The {OTF} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }
    }
}