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
            this.stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new DateTimeParameterTag());
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
        public void Filters_Get_ReturnsEightFilters()
        {
            Assert.That(this.stringSolver.Filters, Has.Count.EqualTo(8));
        }

        [Test]
        public void ParameterTags_Get_ReturnsReadOnlyCollection()
        {
            Assert.That(this.stringSolver.ParameterTags as List<IParameterTag>, Is.Null);
            Assert.That(this.stringSolver.ParameterTags as ReadOnlyCollection<IParameterTag>, Is.Not.Null);
        }

        [Test]
        public void ParameterTags_Get_ReturnsOneParameterTag()
        {
            Assert.That(this.stringSolver.ParameterTags, Has.Count.EqualTo(1));
            Assert.That(this.stringSolver.ParameterTags.First().Name, Is.EqualTo("DateTime"));
        }
    }
}