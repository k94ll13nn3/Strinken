using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using System;

namespace Strinken.Tests.Parser
{
    [TestFixture]
    public class ResolveTests
    {
        private Parser<Data> stringSolver;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new MachineNameParameterTag());
        }

        [Test]
        public void Resolve_OneTag_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {DataName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }

        [Test]
        public void Resolve_OneParameterTag_ReturnsResolvedString()
        {
            Assert.That(stringSolver.Resolve("The {!MachineName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo($"The {Environment.MachineName} is in the kitchen."));
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
        public void Resolve_OneTagAndOneFilterWithParameterTagAsArgument_ReturnsResolvedString()
        {
            Assert.That(
                stringSolver.Resolve("The {DataName:Append+=!MachineName} is in the kitchen.", new Data { Name = "Lorem" }),
                Is.EqualTo($"The Lorem{Environment.MachineName} is in the kitchen."));
            Assert.That(stringSolver.Resolve("The {DataName:Append+MachineName} is in the kitchen.", new Data { Name = "Lorem" }), Is.EqualTo("The LoremMachineName is in the kitchen."));
        }

        [Test]
        public void Resolve_InvalidString_ThrowsFormatException()
        {
            Assert.That(
                () => stringSolver.Resolve("The {DataName:Append+} is in the kitchen.", new Data { Name = "Lorem" }),
                Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty argument"));
        }
    }
}