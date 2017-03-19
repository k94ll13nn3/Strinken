using FluentAssertions;
using Strinken.Engine;
using Xunit;

namespace Strinken.Tests
{
    public class CursorTests
    {
        [Fact]
        public void ParseOutsideString_StringWithoutToken_ReturnsTheString()
        {
            var input = "some string !";
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
            var input = "";
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
            var input = "{";
            var expected = "";
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
            var input = "Mustache : {{ !";
            var expected = "Mustache : { !";
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
            var input = "Mustache : {{";
            var expected = "Mustache : {";
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
            var input = "Mustache : }} !";
            var expected = "Mustache : } !";
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
            var input = "Mustache : }}";
            var expected = "Mustache : }";
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
            var input = "Mustache : { !";
            var expected = "Mustache : ";
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
            var input = "Mustache : {";
            var expected = "Mustache : ";
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
            var input = "Mustache : } !";
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
            var input = "Mustache : }";
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
            var input = "}Mustache";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at position 0");
            }
        }
    }
}