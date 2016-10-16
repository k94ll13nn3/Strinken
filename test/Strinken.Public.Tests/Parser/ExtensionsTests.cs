using NUnit.Framework;
using Strinken.Parser;
using System.Linq;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Parser
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
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            var solver2 = solver.WithFilter(new AppendFilter());

            Assert.That(solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithFilters_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            var solver2 = solver.WithFilters(new[] { new AppendFilter() });

            Assert.That(solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithParameterTag_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            var solver2 = solver.WithParameterTag(new BlueParameterTag());

            Assert.That(solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid, Is.True);
        }

        [Test]
        public void WithParameterTags_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            var solver2 = solver.WithParameterTags(new[] { new BlueParameterTag() });

            Assert.That(solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid, Is.False);
            Assert.That(solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid, Is.True);
        }
    }
}