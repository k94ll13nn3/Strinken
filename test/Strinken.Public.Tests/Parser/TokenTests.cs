using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Strinken.Parser;
using Strinken.Public.Tests.TestsClasses;
using Xunit;

namespace Strinken.Public.Tests.Parser
{
    public class TokenTests
    {
        [Fact]
        public void Tags_Get_ReturnsReadOnlyCollection()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            (stringSolver.Tags as List<ITag<Data>>).Should().BeNull();
            (stringSolver.Tags as ReadOnlyCollection<ITag<Data>>).Should().NotBeNull();
        }

        [Fact]
        public void Tags_Get_ReturnsOneTag()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Tags.Should().HaveCount(1);
            stringSolver.Tags.First().Name.Should().Be("DataName");
        }

        [Fact]
        public void Filters_Get_ReturnsReadOnlyCollection()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            (stringSolver.Filters as List<IFilter>).Should().BeNull();
            (stringSolver.Filters as ReadOnlyCollection<IFilter>).Should().NotBeNull();
        }

        [Fact]
        public void Filters_Get_ReturnsEightFilters()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.Filters.Should().HaveCount(8);
        }

        [Fact]
        public void ParameterTags_Get_ReturnsReadOnlyCollection()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            (stringSolver.ParameterTags as List<IParameterTag>).Should().BeNull();
            (stringSolver.ParameterTags as ReadOnlyCollection<IParameterTag>).Should().NotBeNull();
        }

        [Fact]
        public void ParameterTags_Get_ReturnsOneParameterTag()
        {
            var stringSolver = new Parser<Data>().WithTag(new DataNameTag()).WithFilter(new AppendFilter()).WithParameterTag(new BlueParameterTag());

            stringSolver.ParameterTags.Should().HaveCount(1);
            stringSolver.ParameterTags.First().Name.Should().Be("Blue");
        }
    }
}