using NUnit.Framework;
using Strinken.Engine;
using System;
using System.Collections.Generic;

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
            Func<string, string> actionOnTag = t =>
            {
                numberOfCall++;
                tagSeen.Add(t);
                return t;
            };

            var engine = new StrinkenEngine(actionOnTag, null);
            const string input = "lorem{ispum}tute";
            engine.Run(input);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(tagSeen.Count, Is.EqualTo(1));
            Assert.That(tagSeen[0], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_TwoTags_ActionOnTagCalledTwice()
        {
            var numberOfCall = 0;
            var tagSeen = new List<string>();
            Func<string, string> actionOnTag = t =>
            {
                numberOfCall++;
                tagSeen.Add(t);
                return t;
            };

            var engine = new StrinkenEngine(actionOnTag, null);
            const string input = "lorem{ispum}tute{belli}";
            engine.Run(input);

            Assert.That(numberOfCall, Is.EqualTo(2));
            Assert.That(tagSeen.Count, Is.EqualTo(2));
            Assert.That(tagSeen[0], Is.EqualTo("belli"));
            Assert.That(tagSeen[1], Is.EqualTo("ispum"));
        }

        [Test]
        public void Run_NoTokens_ActionOnCharactersReturnsString()
        {
            var engine = new StrinkenEngine(null, null);
            const string input = "loremtute";
            var result = engine.Run(input);

            Assert.That(result.ParsedString, Is.EqualTo("loremtute"));
        }

        [Test]
        public void Run_StringWithTokens_ActionOnCharactersReturnsStringWithoutTokens()
        {
            var engine = new StrinkenEngine(null, null);
            const string input = "lorem{ipsum}tu{ti}te";
            var result = engine.Run(input);

            Assert.That(result.ParsedString, Is.EqualTo("loremtute"));
        }

        [Test]
        public void Run_StringWithEscapedOpenBracket_ActionOnCharactersReturnsStringWithOneOpenBracket()
        {
            var engine = new StrinkenEngine(null, null);
            const string input = "lorem{{te";
            var result = engine.Run(input);

            Assert.That(result.ParsedString, Is.EqualTo("lorem{te"));
        }

        [Test]
        public void Run_StringWithEscapedCloseBracket_ActionOnCharactersReturnsStringWithOneCloseBracket()
        {
            var engine = new StrinkenEngine(null, null);
            const string input = "lorem}}te";
            var result = engine.Run(input);

            Assert.That(result.ParsedString, Is.EqualTo("lorem}te"));
        }

        [Test]
        public void Run_TagWithFilter_ActionOnFilterCalledOnce()
        {
            var numberOfCall = 0;
            var filterSeen = new List<string>();
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f);
                return f;
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ispum:belli}";
            engine.Run(input);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen.Count, Is.EqualTo(1));
            Assert.That(filterSeen[0], Is.EqualTo("belli"));
        }

        [Test]
        public void Run_TagWithFilter_ActionOnFilterCalledOnceWithTag()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, t);
                return f;
            };

            var engine = new StrinkenEngine(t => t, actionOnFilter);
            const string input = "lorem{ispum:belli}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f);
                return f;
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ispum:belli:tutti}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f);
                return f;
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ispum:belli+arg:tutti}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, t);
                return f + t;
            };

            var engine = new StrinkenEngine(t => t, actionOnFilter);
            const string input = "lorem{ispum:belli:patse}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, a);
                return f;
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ispum:belli+toto}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, a);
                return f;
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ispum:belli+toto,titi}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, a);
                return f;
            };

            Func<string, string> actionOnTag = t => t.ToUpperInvariant();

            var engine = new StrinkenEngine(actionOnTag, actionOnFilter);
            const string input = "lorem{ispum:belli+=toto}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, a);
                return f;
            };

            Func<string, string> actionOnTag = t => t.ToUpperInvariant();

            var engine = new StrinkenEngine(actionOnTag, actionOnFilter);
            const string input = "lorem{ispum:belli+=toto,tata}";
            engine.Run(input);

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
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, t);
                return f + t + string.Join("|", a);
            };

            var engine = new StrinkenEngine(t => t.ToUpperInvariant(), actionOnFilter);
            const string input = "lorem{ipsum:belli+tute:patse+paku,=malo:kuki}";
            var result = engine.Run(input);

            Assert.That(numberOfCall, Is.EqualTo(3));
            Assert.That(filterSeen, Has.Count.EqualTo(3));
            Assert.That(filterSeen, Contains.Key("patse"));
            Assert.That(filterSeen, Contains.Key("belli"));
            Assert.That(filterSeen, Contains.Key("kuki"));
            Assert.That(filterSeen["kuki"], Is.EqualTo("patsebelliIPSUMtutepaku|MALO"));
            Assert.That(filterSeen["patse"], Is.EqualTo("belliIPSUMtute"));
            Assert.That(filterSeen["belli"], Is.EqualTo("IPSUM"));
            Assert.That(result.ParsedString, Is.EqualTo("loremkukipatsebelliIPSUMtutepaku|MALO"));
        }

        [Test]
        public void Run_ActionOnTagIsNull_ActionOnFilterCalledTwiceAndFiltersChained()
        {
            var numberOfCall = 0;
            var filterSeen = new Dictionary<string, string>();
            Func<string, string, string[], string> actionOnFilter = (f, t, a) =>
            {
                numberOfCall++;
                filterSeen.Add(f, t);
                return f + t + string.Join("|", a);
            };

            var engine = new StrinkenEngine(null, actionOnFilter);
            const string input = "lorem{ipsum:patse+paku,=malo}";
            var result = engine.Run(input);

            Assert.That(numberOfCall, Is.EqualTo(1));
            Assert.That(filterSeen, Has.Count.EqualTo(1));
            Assert.That(filterSeen, Contains.Key("patse"));
            Assert.That(filterSeen["patse"], Is.Null);
            Assert.That(result.ParsedString, Is.EqualTo("lorempatsepaku|"));
        }
    }
}