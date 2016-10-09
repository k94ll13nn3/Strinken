using NUnit.Framework;
using Strinken.Parser;
using Strinken.Tests.TestsClasses;

namespace Strinken.Tests.Parser
{
    [TestFixture]
    public class ConstructorTests
    {
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
        public void Constructor_TwoParameterTagsWithSameName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<Data>().WithParameterTags(new IParameterTag[] { new DateTimeParameterTag(), new DateTimeParameterTag() }),
                Throws.ArgumentException.With.Message.EqualTo("DateTime was already registered in the parameter tag list."));
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
        public void Constructor_ParameterTagWithEmptyName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithParameterTag(new EmptyNameParameterTag()),
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
        public void Constructor_ParameterTagWithInvalidName_ThrowsArgumentException()
        {
            Assert.That(
                () => new Parser<string>().WithParameterTag(new InvalidNameParameterTag()),
                Throws.ArgumentException.With.Message.EqualTo("$ is an invalid character for a name."));
        }
    }
}