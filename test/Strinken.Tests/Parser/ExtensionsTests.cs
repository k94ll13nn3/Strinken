using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;
using System.Linq;

namespace Strinken.Tests.Parser
{
    [TestFixture]
    public class ExtensionsTests
    {
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
        public void WithTags_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            var solver2 = solver.WithTags(new[] { new DataNameTag() });

            Assert.That(solver.Validate("The {DataName} {Tag} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {DataName} {Tag} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithFilter_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new CustomFilter());
            var solver2 = solver.WithFilter(new AppendFilter());

            Assert.That(solver.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithFilters_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new CustomFilter());
            var solver2 = solver.WithFilters(new[] { new AppendFilter() });

            Assert.That(solver.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {Tag:Append+One:Custom} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithParameterTag_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new DateTimeParameterTag());
            var solver2 = solver.WithParameterTag(new MachineNameParameterTag());

            Assert.That(solver.Validate("The {!DateTime} {!MachineName} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {!DateTime} {!MachineName} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithParameterTags_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new DateTimeParameterTag());
            var solver2 = solver.WithParameterTags(new[] { new MachineNameParameterTag() });

            Assert.That(solver.Validate("The {!DateTime} {!MachineName} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {!DateTime} {!MachineName} is in the kitchen.").IsValid, Is.True);
        }
    }
}