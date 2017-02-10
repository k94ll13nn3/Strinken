using System;
using FluentAssertions;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Parser
{
    public class ConstructorTests
    {
        [StrinkenTest]
        public void Constructor_IgnoreBaseFilters_ReturnsParserWithoutFilters()
        {
            var solver = new Parser<Data>(true);

            solver.Filters.Should().HaveCount(0);
        }

        [StrinkenTest]
        public void Constructor_TwoTagWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithTag(new DataNameTag()).WithTag(new DataNameTag());

            act.ShouldThrow<ArgumentException>().WithMessage("DataName was already registered in the tag list.");
        }

        [StrinkenTest]
        public void Constructor_TwoFilterWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithTag(new DataNameTag()).WithFilters(new IFilter[] { new AppendFilter(), new AppendFilter() });

            act.ShouldThrow<ArgumentException>().WithMessage("Append was already registered in the filter list.");
        }

        [StrinkenTest]
        public void Constructor_TwoParameterTagsWithSameName_ThrowsArgumentException()
        {
            Action act = () => new Parser<Data>().WithParameterTags(new IParameterTag[] { new BlueParameterTag(), new BlueParameterTag() });

            act.ShouldThrow<ArgumentException>().WithMessage("Blue was already registered in the parameter tag list.");
        }

        [StrinkenTest]
        public void Constructor_TagWithEmptyName_ThrowsArgumentException()
        {
            Action actWithTagParameters = () => new Parser<string>().WithTag("", "", s => s);
            Action actWithTag = () => new Parser<string>().WithTag(new EmptyNameTag());

            actWithTagParameters.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
            actWithTag.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [StrinkenTest]
        public void Constructor_FilterWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new EmptyNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [StrinkenTest]
        public void Constructor_ParameterTagWithEmptyName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithParameterTag(new EmptyNameParameterTag());

            act.ShouldThrow<ArgumentException>().WithMessage("A name cannot be empty.");
        }

        [StrinkenTest]
        public void Constructor_TagWithInvalidName_ThrowsArgumentException()
        {
            Action actWithTagParameters = () => new Parser<string>().WithTag("st*r", "", s => s);
            Action actWithTag = () => new Parser<string>().WithTag(new InvalidNameTag());

            actWithTagParameters.ShouldThrow<ArgumentException>().WithMessage("* is an invalid character for a name.");
            actWithTag.ShouldThrow<ArgumentException>().WithMessage("$ is an invalid character for a name.");
        }

        [StrinkenTest]
        public void Constructor_FilterWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithTag("tag", "tag", s => s).WithFilter(new InvalidNameFilter());

            act.ShouldThrow<ArgumentException>().WithMessage("! is an invalid character for a name.");
        }

        [StrinkenTest]
        public void Constructor_ParameterTagWithInvalidName_ThrowsArgumentException()
        {
            Action act = () => new Parser<string>().WithParameterTag(new InvalidNameParameterTag());

            act.ShouldThrow<ArgumentException>().WithMessage("$ is an invalid character for a name.");
        }
    }
}