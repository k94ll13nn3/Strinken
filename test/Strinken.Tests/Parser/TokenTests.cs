using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Strinken.Tests.Parser
{
    [TestFixture]
    public class TokenTests
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
        public void WithTag_OnTheFlyCreation_ReturnsResolvedString()
        {
            var solver = new Parser<Data>().WithTag("OTF", "OTF", a => a.Name);

            Assert.That(solver.Tags, Has.Count.EqualTo(1));
            Assert.That(solver.Tags.First().Name, Is.EqualTo("OTF"));
            Assert.That(solver.Resolve("The {OTF} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }

        [Test]
        public void WithTag_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            var solver2 = solver.WithTag("TagBis", "TagBis", a => a.Name);

            Assert.That(solver.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithFilter_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new CustomFilter());
            var solver2 = solver.WithFilter(new AppendFilter());

            Assert.That(solver.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.True);
        }
    }
}