using System.Linq;
using FluentAssertions;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class ExtensionsTests
    {
        [Fact]
        public void WithTag_OnTheFlyCreation_ReturnsResolvedString()
        {
            Parser<Data> solver = new Parser<Data>().WithTag("OTF", "OTF", a => a.Name);

            solver.GetTags().Should().HaveCount(1);
            solver.GetTags().First().Name.Should().Be("OTF");
            solver.Resolve("The {OTF} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
        }

        [Fact]
        public void WithTag_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            Parser<Data> solver2 = solver.WithTag("TagBis", "TagBis", a => a.Name);

            solver.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {TagBis} {Tag} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithTags_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name);
            Parser<Data> solver2 = solver.WithTags(new[] { new DataNameTag() });

            solver.Validate("The {DataName} {Tag} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {DataName} {Tag} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithFilter_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            Parser<Data> solver2 = solver.WithFilter(new AppendFilter());

            solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithFilters_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithTag("Tag", "Tag", a => a.Name).WithFilter(new SomeFilter());
            Parser<Data> solver2 = solver.WithFilters(new[] { new AppendFilter() });

            solver.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {Tag:Append+One:Some} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithParameterTag_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            Parser<Data> solver2 = solver.WithParameterTag(new BlueParameterTag());

            solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeTrue();
        }

        [Fact]
        public void WithParameterTags_Called_DoesNotModifyCallingInstance()
        {
            Parser<Data> solver = new Parser<Data>().WithParameterTag(new RedParameterTag());
            Parser<Data> solver2 = solver.WithParameterTags(new[] { new BlueParameterTag() });

            solver.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeFalse();
            solver2.Validate("The {!Red} {!Blue} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}
