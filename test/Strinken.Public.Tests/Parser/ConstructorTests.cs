using System;
using FluentAssertions;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class ConstructorTests
    {
        [Fact]
        public void Constructor_IgnoreBaseFilters_ReturnsParserWithoutFilters()
        {
            var solver = new Parser<Data>(true);

            solver.Filters.Should().HaveCount(0);
        }

        [Fact]
        public void Constructor_TwoTagWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithTag(new DataNameTag()).WithTag(new DataNameTag());

            act.Should().Throw<ArgumentException>().WithMessage("DataName was already registered in the tag list.");
        }

        [Fact]
        public void Constructor_TwoFilterWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithTag(new DataNameTag()).WithFilters(new IFilter[] { new AppendFilter(), new AppendFilter() });

            act.Should().Throw<ArgumentException>().WithMessage("Append was already registered in the filter list.");
        }

        [Fact]
        public void Constructor_TwoParameterTagsWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithParameterTags(new IParameterTag[] { new BlueParameterTag(), new BlueParameterTag() });

            act.Should().Throw<ArgumentException>().WithMessage("Blue was already registered in the parameter tag list.");
        }

        [Fact]
        public void Constructor_TagWithEmptyName_ThrowsArgumentException()
        {
            Action actWithTagParameters = () => new Parser<string>().WithTag("", "", s => s);
            Action actWithTag = () => new Parser<string>().WithTag(new EmptyNameTag());

            actWithTagParameters.Should().Throw<ArgumentException>().WithMessage("A name cannot be empty.");
            actWithTag.Should().Throw<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [Fact]
        public void Constructor_FilterWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new EmptyNameFilter());

            act.Should().Throw<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [Fact]
        public void Constructor_ParameterTagWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithParameterTag(new EmptyNameParameterTag());

            act.Should().Throw<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [Fact]
        public void Constructor_TagWithInvalidName_ThrowsArgumentException()
        {
            Action actWithTagParameters = () => new Parser<string>().WithTag("st*r", "", s => s);
            Action actWithTag = () => new Parser<string>().WithTag(new InvalidNameTag());

            actWithTagParameters.Should().Throw<ArgumentException>().WithMessage("* is an invalid character for a name.");
            actWithTag.Should().Throw<ArgumentException>().WithMessage("$ is an invalid character for a name.");
        }

        [Fact]
        public void Constructor_FilterWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new InvalidNameFilter());

            act.Should().Throw<ArgumentException>().WithMessage("! is an invalid character for a name.");
        }

        [Fact]
        public void Constructor_ParameterTagWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithParameterTag(new InvalidNameParameterTag());

            act.Should().Throw<ArgumentException>().WithMessage("$ is an invalid character for a name.");
        }
    }
}