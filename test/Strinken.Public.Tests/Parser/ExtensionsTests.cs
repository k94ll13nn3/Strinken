using System.Linq;
using FluentAssertions;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;

namespace Strinken.Public.Tests.Parser
{
    public class ExtensionsTests
    {
        [StrinkenTest]
        public void WithTag_OnTheFlyCreation_ReturnsResolvedString()
        {
            var solver = new Parser<Data>().WithTag("OTF", "OTF", a => a.Name);

            solver.Tags.Should().HaveCount(1);
            solver.Tags.First().Name.Should().Be("OTF");
            solver.Resolve("The {OTF} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [StrinkenTest]
        public void WithTag_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            var solver2 = solver.WithTag("TagBis", "TagBis", a => a.Name);

            solver.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [StrinkenTest]
        public void WithTags_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            var solver2 = solver.WithTags(new[] { new DataNameTag() });

            solver.Validate("The {DataName} {Tag} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {DataName} {Tag} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [StrinkenTest]
        public void WithFilter_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            var solver2 = solver.WithFilter(new AppendFilter());

            solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [StrinkenTest]
        public void WithFilters_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            var solver2 = solver.WithFilters(new[] { new AppendFilter() });

            solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [StrinkenTest]
        public void WithParameterTag_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            var solver2 = solver.WithParameterTag(new BlueParameterTag());

            solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [StrinkenTest]
        public void WithParameterTags_Called_DoesNotModifyCallingInstance()
        {
            var solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            var solver2 = solver.WithParameterTags(new[] { new BlueParameterTag() });

            solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}