using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Strinken.Engine;
using Xunit;

namespace Strinken.Tests
{
    public class ActionsTests
    {
        [Fact]
        public void Run_OneTag_ActionOnTagCalledOnce()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    tagSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum}tute";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            tagSeen.Count.Should().Be(1);
            tagSeen[0].Should().Be("ispum");
        }

        [Fact]
        public void Run_TwoTags_ActionOnTagCalledTwice()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    tagSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum}tute{belli}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(2);
            tagSeen.Count.Should().Be(2);
            tagSeen[0].Should().Be("belli");
            tagSeen[1].Should().Be("ispum");
        }

        [Fact]
        public void Run_NoTokens_ActionOnCharactersReturnsString()
        {
            var engine = new StrinkenEngine();
            const string input = "loremtute";
            var result = engine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("loremtute");
        }

        [Fact]
        public void Run_StringWithTokens_ActionOnCharactersReturnsStringWithoutTokens()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum}tu{ti}te";
            var result = engine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("loremtute");
        }

        [Fact]
        public void Run_StringWithEscapedOpenBracket_ActionOnCharactersReturnsStringWithOneOpenBracket()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem{{te";
            var result = engine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("lorem{te");
        }

        [Fact]
        public void Run_StringWithEscapedCloseBracket_ActionOnCharactersReturnsStringWithOneCloseBracket()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem}}te";
            var result = engine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("lorem}te");
        }

        [Fact]
        public void Run_TagWithFilter_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Count.Should().Be(1);
            filterSeen[0].Should().Be("belli");
        }

        [Fact]
        public void Run_TagWithFilter_ActionOnFilterCalledOnceWithTag()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0],
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Count.Should().Be(1);
            filterSeen.ContainsKey("belli").Should().BeTrue();
            filterSeen["belli"].Should().Be("ispum");
        }

        [Fact]
        public void Run_TagWithMultipleFilters_ActionOnFilterCalledTwice()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli:tutti}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(2);
            filterSeen.Count.Should().Be(2);
            filterSeen[0].Should().Be("belli");
            filterSeen[1].Should().Be("tutti");
        }

        [Fact]
        public void Run_TagWithMultipleFiltersAndOneArgumentOnFirstFilter_ActionOnFilterCalledTwice()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+arg:tutti}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(2);
            filterSeen.Count.Should().Be(2);
            filterSeen[0].Should().Be("belli");
            filterSeen[1].Should().Be("tutti");
        }

        [Fact]
        public void Run_TagWithMultipleFilters_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0],
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0] + a[1];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli:patse}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(2);
            filterSeen.Count.Should().Be(2);
            filterSeen.ContainsKey("patse").Should().BeTrue();
            filterSeen.ContainsKey("belli").Should().BeTrue();
            filterSeen["patse"].Should().Be("belliispum");
            filterSeen["belli"].Should().Be("ispum");
        }

        [Fact]
        public void Run_TagWithFilterAndOneArgument_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                },
                [TokenType.Argument, '\0', '\0'] = a => a[0]
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+toto}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("belli");
            filterSeen["belli"].Should().HaveCount(1);
            filterSeen["belli"][0].Should().Be("toto");
        }

        [Fact]
        public void Run_TagWithFilterAndTwoArguments_ActionOnFilterCalledOnceAndArgumentsProperlySorted()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                },
                [TokenType.Argument, '\0', '\0'] = a => a[0]
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+toto,titi}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("belli");
            filterSeen["belli"].Should().HaveCount(2);
            filterSeen["belli"][0].Should().Be("toto");
            filterSeen["belli"][1].Should().Be("titi");
        }

        [Fact]
        public void Run_TagWithFilterAndOneArgumentTag_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Argument, '=', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+=toto}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("belli");
            filterSeen["belli"].Should().HaveCount(1);
            filterSeen["belli"][0].Should().Be("TOTO");
        }

        [Fact]
        public void Run_TagWithFilterAndOneArgumentTagAndOneArgument_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Argument, '=', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                },
                [TokenType.Argument, '\0', '\0'] = a => a[0]
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+=toto,tata}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("belli");
            filterSeen["belli"].Should().HaveCount(2);
            filterSeen["belli"][0].Should().Be("TOTO");
            filterSeen["belli"][1].Should().Be("tata");
        }

        [Fact]
        public void Run_TagWithMultipleFiltersAndArguments_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Argument, '=', '\0'] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0] + a[1] + string.Join("|", a.Skip(2));
                },
                [TokenType.Argument, '\0', '\0'] = a => a[0]
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum:belli+tute:patse+paku,=malo:kuki}";
            var result = engine.Run(input);
            var parsedString = result.Stack.Resolve(actions);

            numberOfCall.Should().Be(3);
            filterSeen.Should().HaveCount(3);
            filterSeen.Should().ContainKey("patse");
            filterSeen.Should().ContainKey("belli");
            filterSeen.Should().ContainKey("kuki");
            filterSeen["kuki"].Should().Be("patsebelliIPSUMtutepaku|MALO");
            filterSeen["patse"].Should().Be("belliIPSUMtute");
            filterSeen["belli"].Should().Be("IPSUM");
            parsedString.Should().Be("loremkukipatsebelliIPSUMtutepaku|MALO");
        }

        [Fact]
        public void Run_ActionOnTagIsNull_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0] + a[1] + string.Join("|", a.Skip(2));
                },
                [TokenType.Argument, '\0', '\0'] = a => a[0]
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum:patse+paku,=malo}";
            var result = engine.Run(input);
            var parsedString = result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("patse");
            filterSeen["patse"].Should().BeNull();
            parsedString.Should().Be("lorempatsepaku|");
        }

        [Fact]
        public void Run_NoActions_ReturnsOutsideString()
        {
            const string input = "lorem{ipsum:patse+paku,=malo}aku";
            new StrinkenEngine().Run(input).Stack.Resolve(null).Should().Be("loremaku");
        }

        [Fact]
        public void Run_OneParameterTag_ActionOnParameterTagCalledOnce()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '!'] = a =>
                {
                    numberOfCall++;
                    tagSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{!ispum}tute";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            tagSeen.Count.Should().Be(1);
            tagSeen[0].Should().Be("ispum");
        }

        [Fact]
        public void Run_TagWithFilterAndOneArgumentParameterTag_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '\0', '\0'] = a => a[0],
                [TokenType.Argument, '=', '\0'] = a => a[0],
                [TokenType.Tag, '\0', '!'] = a => "KAPOUE",
                [TokenType.Argument, '=', '!'] = a => "KAPOUE",
                [TokenType.Filter, '\0', '\0'] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+=!toto}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            filterSeen.Should().HaveCount(1);
            filterSeen.Should().ContainKey("belli");
            filterSeen["belli"].Should().HaveCount(1);
            filterSeen["belli"][0].Should().Be("KAPOUE");
            filterSeen["belli"][0].Should().NotBe("toto");
        }

        [Fact]
        public void Run_OneValueTagAndOneTag_ActionOnValueTagCalledOnce()
        {
            var numberOfCall = 0;
            var valuesSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '@', '\0'] = a =>
                {
                    numberOfCall++;
                    valuesSeen.Add(a[0]);
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "{tag} {@som3t!me$}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("som3t!me$");
        }
    }
}