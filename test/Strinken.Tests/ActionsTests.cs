using NUnit.Framework;
using Strinken.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Strinken.Tests
{
    [TestFixture]
    public class ActionsTests
    {
        [Test]
        public void Run_OneTag_ActionOnTagCalledOnce()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(tagSeen.Count, Is.EqualTo(1));
            Assert.That(tagSeen[0], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_TwoTags_ActionOnTagCalledTwice()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(2));
            Assert.That(tagSeen.Count, Is.EqualTo(2));
            Assert.That(tagSeen[0], Is.EqualTo("belli"));
            Assert.That(tagSeen[1], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_NoTokens_ActionOnCharactersReturnsString()
        {
            var engine = new StrinkenEngine();
            const string input = "loremtute";
            var result = engine.Run(input);

            Assert.That(result.Stack.Resolve(new ActionDictionary()), Is.EqualTo("loremtute"));
        }

        [Test]
        public void Run_StringWithTokens_ActionOnCharactersReturnsStringWithoutTokens()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum}tu{ti}te";
            var result = engine.Run(input);

            Assert.That(result.Stack.Resolve(new ActionDictionary()), Is.EqualTo("loremtute"));
        }

        [Test]
        public void Run_StringWithEscapedOpenBracket_ActionOnCharactersReturnsStringWithOneOpenBracket()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem{{te";
            var result = engine.Run(input);

            Assert.That(result.Stack.Resolve(new ActionDictionary()), Is.EqualTo("lorem{te"));
        }

        [Test]
        public void Run_StringWithEscapedCloseBracket_ActionOnCharactersReturnsStringWithOneCloseBracket()
        {
            var engine = new StrinkenEngine();
            const string input = "lorem}}te";
            var result = engine.Run(input);

            Assert.That(result.Stack.Resolve(new ActionDictionary()), Is.EqualTo("lorem}te"));
        }

        [Test]
        public void Run_TagWithFilter_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen.Count, Is.EqualTo(1));
            Assert.That(filterSeen[0], Is.EqualTo("belli"));
        }

        [Test]
        public void Run_TagWithFilter_ActionOnFilterCalledOnceWithTag()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a => a[0],
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen.Count, Is.EqualTo(1));
            Assert.That(filterSeen.ContainsKey("belli"), Is.True);
            Assert.That(filterSeen["belli"], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_TagWithMultipleFilters_ActionOnFilterCalledTwice()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(2));
            Assert.That(filterSeen.Count, Is.EqualTo(2));
            Assert.That(filterSeen[0], Is.EqualTo("belli"));
            Assert.That(filterSeen[1], Is.EqualTo("tutti"));
        }

        [Test]
        public void Run_TagWithMultipleFiltersAndOneArgumentOnFirstFilter_ActionOnFilterCalledTwice()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(2));
            Assert.That(filterSeen.Count, Is.EqualTo(2));
            Assert.That(filterSeen[0], Is.EqualTo("belli"));
            Assert.That(filterSeen[1], Is.EqualTo("tutti"));
        }

        [Test]
        public void Run_TagWithMultipleFilters_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a => a[0],
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(2));
            Assert.That(filterSeen.Count, Is.EqualTo(2));
            Assert.That(filterSeen.ContainsKey("patse"), Is.True);
            Assert.That(filterSeen.ContainsKey("belli"), Is.True);
            Assert.That(filterSeen["patse"], Is.EqualTo("belliispum"));
            Assert.That(filterSeen["belli"], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_TagWithFilterAndOneArgument_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+toto}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen["belli"], Has.Length.EqualTo(1));
            Assert.That(filterSeen["belli"][0], Is.EqualTo("toto"));
        }

        [Test]
        public void Run_TagWithFilterAndTwoArguments_ActionOnFilterCalledOnceAndArgumentsProperlySorted()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+toto,titi}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen["belli"], Has.Length.EqualTo(2));
            Assert.That(filterSeen["belli"][0], Is.EqualTo("toto"));
            Assert.That(filterSeen["belli"][1], Is.EqualTo("titi"));
        }

        [Test]
        public void Run_TagWithFilterAndOneArgumentTag_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter] = a =>
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

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen["belli"], Has.Length.EqualTo(1));
            Assert.That(filterSeen["belli"][0], Is.EqualTo("TOTO"));
        }

        [Test]
        public void Run_TagWithFilterAndOneArgumentTagAndOneArgument_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string[]>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a.Skip(2).ToArray());
                    return a[0];
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ispum:belli+=toto,tata}";
            var result = engine.Run(input);
            result.Stack.Resolve(actions);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen["belli"], Has.Length.EqualTo(2));
            Assert.That(filterSeen["belli"][0], Is.EqualTo("TOTO"));
            Assert.That(filterSeen["belli"][1], Is.EqualTo("tata"));
        }

        [Test]
        public void Run_TagWithMultipleFiltersAndArguments_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Tag] = a => a[0].ToUpperInvariant(),
                [TokenType.Filter] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0] + a[1] + string.Join("|", a.Skip(2));
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum:belli+tute:patse+paku,=malo:kuki}";
            var result = engine.Run(input);
            var parsedString = result.Stack.Resolve(actions);

            Assert.That(numberOfCall, Is.EqualTo(3));
            Assert.That(filterSeen, Has.Count.EqualTo(3));
            Assert.That(filterSeen, Contains.Key("patse"));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen, Contains.Key("kuki"));
            Assert.That(filterSeen["kuki"], Is.EqualTo("patsebelliIPSUMtutepaku|MALO"));
            Assert.That(filterSeen["patse"], Is.EqualTo("belliIPSUMtute"));
            Assert.That(filterSeen["belli"], Is.EqualTo("IPSUM"));
            Assert.That(parsedString, Is.EqualTo("loremkukipatsebelliIPSUMtutepaku|MALO"));
        }

        [Test]
        public void Run_ActionOnTagIsNull_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            var actions = new ActionDictionary
            {
                [TokenType.Filter] = a =>
                {
                    numberOfCall++;
                    filterSeen.Add(a[0], a[1]);
                    return a[0] + a[1] + string.Join("|", a.Skip(2));
                }
            };

            var engine = new StrinkenEngine();
            const string input = "lorem{ipsum:patse+paku,=malo}";
            var result = engine.Run(input);
            var parsedString = result.Stack.Resolve(actions);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("patse"));
            Assert.That(filterSeen["patse"], Is.Null);
            Assert.That(parsedString, Is.EqualTo("lorempatsepaku|"));
        }

        [Test]
        public void Run_NoActions_ReturnsOutsideString()
        {
            const string input = "lorem{ipsum:patse+paku,=malo}";
            Assert.That(new StrinkenEngine().Run(input).Stack.Resolve(null), Is.EqualTo("lorem"));
        }
    }
}