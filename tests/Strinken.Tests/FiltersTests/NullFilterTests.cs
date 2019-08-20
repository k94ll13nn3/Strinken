using System;
using FluentAssertions;
using Strinken.Tests.TestsClasses;
using Xunit;

namespace Strinken.Tests.FiltersTests
{
    public class NullFilterTests
    {
        [Fact]
        public void Resolve_Data_ReturnsData()
        {
            var filter = new NullFilter();

            filter.Resolve("value", new string[] { "data" }).Should().Be("value");
        }

        [Fact]
        public void Resolve_NullData_ReturnsArgument()
        {
            var filter = new NullFilter();

            filter.Resolve(null, new string[] { "data" }).Should().Be("data");
        }

        [Fact]
        public void Validate_NoArguments_ReturnsFalse()
        {
            var filter = new NullFilter();

            filter.Validate(null).Should().BeFalse();
            filter.Validate(Array.Empty<string>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_MoreThanOneArgument_ReturnsFalse()
        {
            var filter = new NullFilter();

            filter.Validate(new string[] { "", "", "" }).Should().BeFalse();
        }

        [Fact]
        public void Validate_OneArgument_ReturnsTrue()
        {
            var filter = new NullFilter();

            filter.Validate(new string[] { "data" }).Should().BeTrue();
        }

        [Fact]
        public void Resolve_ReturnsResolvedString()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:Null+Ipsum} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
            stringSolver.Resolve("The {DataName:Null+Ipsum} is in the kitchen.", new Data { Name = null }).Should().Be("The Ipsum is in the kitchen.");
        }

        [Fact]
        public void Resolve_WithAlternativeName_ReturnsResolvedString()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Resolve("The {DataName:??+Ipsum} is in the kitchen.", new Data { Name = "Lorem" }).Should().Be("The Lorem is in the kitchen.");
            stringSolver.Resolve("The {DataName:??+Ipsum} is in the kitchen.", new Data { Name = null }).Should().Be("The Ipsum is in the kitchen.");
        }

        [Fact]
        public void Validate_ReturnsTrue()
        {
            Parser<Data> stringSolver = new Parser<Data>().WithTag(new DataNameTag());
            stringSolver.Validate("The {DataName:Null+Ipsum} is in the kitchen.").IsValid.Should().BeTrue();
            stringSolver.Validate("The {DataName:Null+Ipsum} is in the kitchen.").IsValid.Should().BeTrue();
        }
    }
}
