using System;
using FluentAssertions;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class CompilationTests
    {
        [Fact]
        public void ResolveCompiledString_StringCompiled_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Compile("The {DataName} is in the kitchen.");

            stringSolver.ResolveCompiledString(new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [Fact]
        public void ResolveCompiledString_TwoStringCompiled_ReturnsResolvedStringForLastString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Compile("The {DataName} is in the kitchen.");
            stringSolver.Compile("The {DataName} is not in the kitchen.");

            stringSolver.ResolveCompiledString(new Data { Name = "Lorem" }).Should().Be("The Lorem is not in the kitchen.");
        }

        [Fact]
        public void ResolveCompiledString_NoCompiledString_ThrowsInvalidOperationException()
        {
            var stringSolver = new Parser<Data>();

            Action act = () => stringSolver.ResolveCompiledString(new Data { Name = "Lorem" });

            act.Should().Throw<InvalidOperationException>().WithMessage("No string were compiled.");
        }

        [Fact]
        public void ResolveCompiledString_InvalidString_ThrowsFormatException()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            Action act = () => stringSolver.Compile("The {DataName:Append+} is in the kitchen.");

            act.Should().Throw<FormatException>().WithMessage("Empty argument");
        }
    }
}