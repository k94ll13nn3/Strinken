using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Strinken.Core;
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

            const string input = "lorem{ispum}tute";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum}tute{belli}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(2);
            tagSeen.Count.Should().Be(2);
            tagSeen[0].Should().Be("belli");
            tagSeen[1].Should().Be("ispum");
        }

        [Fact]
        public void Run_NoTokens_ActionOnCharactersReturnsString()
        {
            const string input = "loremtute";
            EngineResult result = StrinkenEngine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("loremtute");
        }

        [Fact]
        public void Run_StringWithTokens_ActionOnCharactersReturnsStringWithoutTokens()
        {
            const string input = "lorem{ipsum}tu{ti}te";
            EngineResult result = StrinkenEngine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("loremtute");
        }

        [Fact]
        public void Run_StringWithEscapedOpenBracket_ActionOnCharactersReturnsStringWithOneOpenBracket()
        {
            const string input = "lorem{{te";
            EngineResult result = StrinkenEngine.Run(input);

            result.Stack.Resolve(new ActionDictionary()).Should().Be("lorem{te");
        }

        [Fact]
        public void Run_StringWithEscapedCloseBracket_ActionOnCharactersReturnsStringWithOneCloseBracket()
        {
            const string input = "lorem}}te";
            EngineResult result = StrinkenEngine.Run(input);

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

            const string input = "lorem{ispum:belli}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli:tutti}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+arg:tutti}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli:patse}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+toto}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+toto,titi}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+=toto}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+=toto,tata}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ipsum:belli+tute:patse+paku,=malo:kuki}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ipsum:patse+paku,=malo}";
            EngineResult result = StrinkenEngine.Run(input);
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
            StrinkenEngine.Run(input).Stack.Resolve(null).Should().Be("loremaku");
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

            const string input = "lorem{!ispum}tute";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "lorem{ispum:belli+=!toto}";
            EngineResult result = StrinkenEngine.Run(input);
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

            const string input = "{tag} {@som3t!me$}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("som3t!me$");
        }

        [Fact]
        public void Run_OneNumberAndBinaryTag_ActionOnValueTagCalledOnce()
        {
            var numberOfCall = 0;
            var valuesSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '#', 'b'] = a =>
                {
                    numberOfCall++;
                    valuesSeen.Add(a[0]);
                    return a[0];
                }
            };

            const string input = "{#b11010011010101}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("11010011010101");
        }

        [Fact]
        public void Run_OneNumberAndOctalTag_ActionOnValueTagCalledOnce()
        {
            var numberOfCall = 0;
            var valuesSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '#', 'o'] = a =>
                {
                    numberOfCall++;
                    valuesSeen.Add(a[0]);
                    return a[0];
                }
            };

            const string input = "{#o1463357}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("1463357");
        }

        [Fact]
        public void Run_OneNumberAndDecimalTag_ActionOnValueTagCalledOnce()
        {
            var numberOfCall = 0;
            var valuesSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '#', 'd'] = a =>
                {
                    numberOfCall++;
                    valuesSeen.Add(a[0]);
                    return a[0];
                }
            };

            const string input = "{#d457901245}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("457901245");
        }

        [Fact]
        public void Run_OneNumberAndHexadecimalTag_ActionOnValueTagCalledOnce()
        {
            var numberOfCall = 0;
            var valuesSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag, '#', 'x'] = a =>
                {
                    numberOfCall++;
                    valuesSeen.Add(a[0]);
                    return a[0];
                }
            };

            const string input = "{#x45bAe7845F}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Stack.Resolve(actions);

            numberOfCall.Should().Be(1);
            valuesSeen.Count.Should().Be(1);
            valuesSeen[0].Should().Be("45bAe7845F");
        }
    }
}