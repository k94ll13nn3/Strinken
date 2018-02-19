using FluentAssertions;
using Strinken.Core;
using Xunit;

namespace Strinken.Tests
{
    public class CursorTests
    {
        [Fact]
        public void ParseOutsideString_StringWithoutToken_ReturnsTheString()
        {
            const string input = "some string !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(input);
            }
        }

        [Fact]
        public void ParseOutsideString_EmptyString_ReturnsEmptyString()
        {
            const string input = "";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(input);
            }
        }

        [Fact]
        public void ParseOutsideString_OnlyTokenStartIndicator_ReturnsEmptyString()
        {
            const string input = "{";
            const string expected = "";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithEscapedTokenStartIndicatorInside_ReturnsTheStringWithOneTokenStartIndicatorInside()
        {
            const string input = "Mustache : {{ !";
            const string expected = "Mustache : { !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithEscapedTokenStartIndicatorAtEnd_ReturnsTheStringWithOneTokenStartIndicatorAtEnd()
        {
            const string input = "Mustache : {{";
            const string expected = "Mustache : {";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithEscapedTokenEndIndicatorInside_ReturnsTheStringWithOneTokenEndIndicatorInside()
        {
            const string input = "Mustache : }} !";
            const string expected = "Mustache : } !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithEscapedTokenEndIndicatorAtEnd_ReturnsTheStringWithOneTokenEndIndicatorAtEnd()
        {
            const string input = "Mustache : }}";
            const string expected = "Mustache : }";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithOneTokenStartIndicatorInside_ReturnsPartBeforeTokenStartIndicator()
        {
            const string input = "Mustache : { !";
            const string expected = "Mustache : ";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithOneTokenStartIndicatorAtEnd_ReturnsPartBeforeTokenStartIndicator()
        {
            const string input = "Mustache : {";
            const string expected = "Mustache : ";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Data.Should().Be(expected);
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithOneTokenEndIndicatorInside_ReturnsFailure()
        {
            const string input = "Mustache : } !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at position 11");
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithOneTokenEndIndicatorAtEnd_ReturnsFailure()
        {
            const string input = "Mustache : }";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at the end of the string");
            }
        }

        [Fact]
        public void ParseOutsideString_StringWithOneTokenEndIndicatorAtStart_ReturnsFailure()
        {
            const string input = "}Mustache";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at position 0");
            }
        }
    }
}