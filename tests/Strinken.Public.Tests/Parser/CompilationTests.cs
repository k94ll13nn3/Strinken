using System;
using System.Collections.Generic;
using FluentAssertions;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class CompilationTests
    {
        [Fact]
        public void Compile_ValidString_ReturnsCompiledStringNotNull()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            CompiledString compiledString = stringSolver.Compile("The {DataName} is in the kitchen.");

            compiledString.Should().NotBeNull();
        }

        [Fact]
        public void Compile_InvalidString_ThrowsFormatException()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            Action act = () => stringSolver.Compile("The {DataName:Append+} is in the kitchen.");

            act.Should().Throw<FormatException>().WithMessage("Empty argument");
        }

        [Fact]
        public void Resolve_CompiledString_ReturnsResolvedString()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            CompiledString compiledString = stringSolver.Compile("The {DataName} is in the kitchen.");

            stringSolver.Resolve(compiledString, new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [Fact]
        public void Resolve_CompiledStringResolvedTwoTimes_ReturnsResolvedStrings()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            CompiledString compiledString = stringSolver.Compile("The {DataName} is in the kitchen.");

            stringSolver.Resolve(compiledString, new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
            stringSolver.Resolve(compiledString, new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [Fact]
        public void Resolve_CompiledStringAndValues_ReturnsResolvedStrings()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            CompiledString compiledString = stringSolver.Compile("The {DataName} is in the kitchen.");

            string[] expected = new[] { "The Lorem is in the kitchen.", "The Ipsum is in the kitchen.", "The Sanctum is in the kitchen." };
            Data[] data = new[] { new Data { Name = "Lorem" }, new Data { Name = "Ipsum" }, new Data { Name = "Sanctum" } };
            stringSolver.Resolve(compiledString, data).Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Resolve_NullCompiledString_ThrowsArgumentNullException()
        {
            var stringSolver = new Parser<Data>();

            Action act = () => stringSolver.Resolve((CompiledString)null, new Data { Name = "Lorem" });

            act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "compiledString");
        }

        [Fact]
        public void Resolve_NullCompiledStringAndValues_ThrowsArgumentNullException()
        {
            var stringSolver = new Parser<Data>();

            Action act = () => stringSolver.Resolve((CompiledString)null, Array.Empty<Data>());

            act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "compiledString");
        }

        [Fact]
        public void Resolve_CompiledStringAndNullValues_ThrowsArgumentNullException()
        {
            var stringSolver = new Parser<Data>();

            CompiledString compiledString = stringSolver.Compile("The {DataName} is in the kitchen.");
            Action act = () => stringSolver.Resolve(compiledString, (IEnumerable<Data>)null);

            act.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "values");
        }
    }
}
