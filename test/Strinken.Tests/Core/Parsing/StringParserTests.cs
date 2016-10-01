using NUnit.Framework;
using Strinken.Core.Parsing;
using Strinken.Core.Types;
using Strinken.Engine;

namespace Strinken.Tests.Core.Parsing
{
    [TestFixture]
    public class StringParserTests
    {
        [TestCase("name")]
        [TestCase("na_me")]
        [TestCase("na-me")]
        public void ParseName_GoodName_ReturnsSuccessAndName(string input)
        {
            ParserResult<string> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseName(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.Value, Is.EqualTo(input));
        }

        [Test]
        public void ParseName_EmptyString_ReturnsFailure()
        {
            var input = string.Empty;
            ParserResult<string> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseName(cursor);
            }

            Assert.That(result.Result, Is.False);
        }

        [Test]
        public void ParseName_StringWithPeriod_ReturnsStringBeforePeriod()
        {
            const string input = "Na.me";
            ParserResult<string> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseName(cursor);
            }
            Assert.That(result.Result, Is.True);
            Assert.That(result.Value, Is.EqualTo("Na"));
        }

        [Test]
        public void ParseNameWithEnd_EndFound_ReturnsSuccessAndStringAndEnd()
        {
            var input = "name+";
            ParserResult<string, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseNameWithEnd(cursor, new int[] { '+', '`' });
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.Value, Is.EqualTo("name"));
            Assert.That(result.OptionalData, Is.EqualTo('+'));
        }

        [Test]
        public void ParseNameWithEnd_EndNotFound_ReturnsFailure()
        {
            var input = "name|";
            ParserResult<string, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseNameWithEnd(cursor, new int[] { '+', '`' });
            }

            Assert.That(result.Result, Is.False);
        }

        [Test]
        public void ParseTag_BaseTag_ReturnsSuccessAndTag()
        {
            const string input = "tag:";
            ParserResult<Token> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseTag(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.Value.Data, Is.EqualTo("tag"));
            Assert.That(result.Value.Type, Is.EqualTo(TokenType.Tag));
            Assert.That(result.Value.Subtype, Is.EqualTo(TokenSubtype.Base));
        }

        [Test]
        public void ParseTag_InvalidTag_ReturnsFailure()
        {
            const string input = "tag'";
            ParserResult<Token> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseTag(cursor);
            }

            Assert.That(result.Result, Is.False);
        }

        [TestCase("filter:", ':')]
        [TestCase("filter+", '+')]
        public void ParseFilter_BaseFilter_ReturnsSuccessAndFilterAndDelimiter(string input, int delimiter)
        {
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseFilter(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.OptionalData, Is.EqualTo(delimiter));
            Assert.That(result.Value.Data, Is.EqualTo(input.Substring(0, input.Length - 1)));
            Assert.That(result.Value.Type, Is.EqualTo(TokenType.Filter));
            Assert.That(result.Value.Subtype, Is.EqualTo(TokenSubtype.Base));
        }

        [Test]
        public void ParseFilter_InvalidFilter_ReturnsFailure()
        {
            const string input = "filter=";
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseFilter(cursor);
            }

            Assert.That(result.Result, Is.False);
        }
    }
}