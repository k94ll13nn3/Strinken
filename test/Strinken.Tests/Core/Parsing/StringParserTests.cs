using NUnit.Framework;
using Strinken.Common;
using Strinken.Core.Parsing;
using Strinken.Core.Types;
using Strinken.Engine;

namespace Strinken.Tests.Core.Parsing
{
    [TestFixture]
    public class StringParserTests
    {
        [Test]
        public void ParseName_EndFound_ReturnsSuccessAndStringAndEnd()
        {
            const string input = "name+";
            ParserResult<string, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseName(cursor, new int[] { '+', '`' }, c => !((char)c).IsInvalidTokenNameCharacter());
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.Value, Is.EqualTo("name"));
            Assert.That(result.OptionalData, Is.EqualTo('+'));
        }

        [Test]
        public void ParseName_EndNotFound_ReturnsFailure()
        {
            const string input = "name|";
            ParserResult<string, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseName(cursor, new int[] { '+', '`' }, c => !((char)c).IsInvalidTokenNameCharacter());
            }

            Assert.That(result.Result, Is.False);
        }

        [Test]
        public void ParseTag_BaseTag_ReturnsSuccessAndTag()
        {
            const string input = "tag:";
            ParserResult<Token, int> result;
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
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseTag(cursor);
            }

            Assert.That(result.Result, Is.False);
        }

        [TestCase(":filter:", ':')]
        [TestCase(":filter+", '+')]
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
            Assert.That(result.Value.Data, Is.EqualTo(input.Substring(1, input.Length - 2)));
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

        [Test]
        public void ParseArgument_LastArgument_ReturnsSuccessAndFilterDelimiter()
        {
            const string input = ",lor#em:";
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseArgument(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.OptionalData, Is.EqualTo(':'));
            Assert.That(result.Value.Data, Is.EqualTo("lor#em"));
            Assert.That(result.Value.Type, Is.EqualTo(TokenType.Argument));
            Assert.That(result.Value.Subtype, Is.EqualTo(TokenSubtype.Base));
        }

        [Test]
        public void ParseArgument_Argument_ReturnsSuccessAndArgumentDelimiter()
        {
            const string input = "+lorem|,";
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseArgument(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.OptionalData, Is.EqualTo(','));
            Assert.That(result.Value.Data, Is.EqualTo("lorem|"));
            Assert.That(result.Value.Type, Is.EqualTo(TokenType.Argument));
            Assert.That(result.Value.Subtype, Is.EqualTo(TokenSubtype.Base));
        }

        [Test]
        public void ParseArgument_ArgumentTag_ReturnsSuccessAndArgumentDelimiter()
        {
            const string input = ",=lorem,";
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseArgument(cursor);
            }

            Assert.That(result.Result, Is.True);
            Assert.That(result.OptionalData, Is.EqualTo(','));
            Assert.That(result.Value.Data, Is.EqualTo("lorem"));
            Assert.That(result.Value.Type, Is.EqualTo(TokenType.Argument));
            Assert.That(result.Value.Subtype, Is.EqualTo(TokenSubtype.Tag));
        }

        [Test]
        public void ParseArgument_InvalidArgument_ReturnsFailure()
        {
            const string input = "arg:";
            ParserResult<Token, int> result;
            using (var cursor = new Cursor(input))
            {
                cursor.Next();
                result = StringParser.ParseArgument(cursor);
            }

            Assert.That(result.Result, Is.False);
        }
    }
}