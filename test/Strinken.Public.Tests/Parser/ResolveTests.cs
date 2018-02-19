using System;
using FluentAssertions;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class ResolveTests
    {
        [Fact]
        public void Resolve_OneTag_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneParameterTag_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {!Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Blue is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneTagAndOneFilter_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName:Upper} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LOREM is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneTagAndOneFilterAndOneArgument_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 5 is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneTagAndTwoFilters_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName:Append+One,Two:Length} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The 11 is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneTagAndOneFilterWithTagAsArgument_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName:Append+=DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremLorem is in the kitchen.");
            stringSolver.Resolve("The {DataName:Append+DataName} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremDataName is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneTagAndOneFilterWithParameterTagAsArgument_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {DataName:Append+=!Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremBlue is in the kitchen.");
            stringSolver.Resolve("The {DataName:Append+Blue} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The LoremBlue is in the kitchen.");
        }

        [Fact]
        public void Resolve_InvalidString_ThrowsFormatException()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            Action act = () => stringSolver.Resolve("The {DataName:Append+} is in the kitchen.", new Data { Name = "Lorem" });

            act.ShouldThrow<FormatException>().WithMessage("Empty argument");
        }

        [Fact]
        public void Resolve_OneValueTag_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("The {@som3th!ng$} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The som3th!ng$ is in the kitchen.");
        }

        [Fact]
        public void Resolve_OneValueTagWithFilter_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Resolve("{@som3th!ng$:Length}", new Data { Name = "Lorem" }).Should().Be("10");
        }

        [Fact]
        public void Resolve_OneNumberTagBinaryFormat_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>();

            stringSolver.Resolve("{#b1101100}", new Data { Name = "Lorem" }).Should().Be("1101100");
        }

        [Fact]
        public void Resolve_OneNumberTagOctalFormat_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>();

            stringSolver.Resolve("{#o4012546}", new Data { Name = "Lorem" }).Should().Be("4012546");
        }

        [Fact]
        public void Resolve_OneNumberTagDecimalFormat_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>();

            stringSolver.Resolve("{#d487902654}", new Data { Name = "Lorem" }).Should().Be("487902654");
        }

        [Fact]
        public void Resolve_OneNumberTagHexadecimalFormat_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>();

            stringSolver.Resolve("{#x787AEEF454601BB}", new Data { Name = "Lorem" }).Should().Be("787AEEF454601BB");
        }
    }
}