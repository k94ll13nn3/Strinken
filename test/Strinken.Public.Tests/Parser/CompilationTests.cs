using System;
using NUnit.Framework;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Parser
{
    [TestFixture]
    public class CompilationTests
    {
        [Test]
        public void ResolveCompiledString_StringCompiled_ReturnsResolvedString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            stringSolver.Compile("The {DataName} is in the kitchen.");
            Assert.That(stringSolver.ResolveCompiledString(new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is in the kitchen."));
        }

        [Test]
        public void ResolveCompiledString_TwoStringCompiled_ReturnsResolvedStringForLastString()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            stringSolver.Compile("The {DataName} is in the kitchen.");
            stringSolver.Compile("The {DataName} is not in the kitchen.");
            Assert.That(stringSolver.ResolveCompiledString(new Data { Name = "Lorem" }), Is.EqualTo("The Lorem is not in the kitchen."));
        }

        [Test]
        public void ResolveCompiledString_NoCompiledString_ThrowsInvalidOperationException()
        {
            var stringSolver = new Parser<Data>();
            Assert.That(
               () => stringSolver.ResolveCompiledString(new Data { Name = "Lorem" }),
               Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo("No string were compiled."));
        }

        [Test]
        public void ResolveCompiledString_InvalidString_ThrowsFormatException()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());
            Assert.That(
                () => stringSolver.Compile("The {DataName:Append+} is in the kitchen."),
                Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty argument"));
        }
    }
}