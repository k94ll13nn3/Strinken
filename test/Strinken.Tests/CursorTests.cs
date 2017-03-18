using FluentAssertions;
using Strinken.Engine;

namespace Strinken.Tests
{
    public class CursorTests
    {
        [StrinkenTest]
        public void ParseOutsideString_StringWithoutToken_ReturnsTheString()
        {
            var input = "some string !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(input);
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithEscapedTokenStartIndicator_ReturnsTheStringWithOneTokenStartIndicator()
        {
            var input = "Mustache : {{ !";
            var expected = "Mustache : { !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithEscapedTokenStartIndicatorAtEnd_ReturnsTheStringWithOneTokenStartIndicatorAtEnd()
        {
            var input = "Mustache : {{";
            var expected = "Mustache : {";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithEscapedTokenEndIndicator_ReturnsTheStringWithOneTokenEndIndicator()
        {
            var input = "Mustache : }} !";
            var expected = "Mustache : } !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }
        [StrinkenTest]
        public void ParseOutsideString_StringWithEscapedTokenEndIndicatorAtEnd_ReturnsTheStringWithOneTokenEndIndicatorAtEnd()
        {
            var input = "Mustache : }}";
            var expected = "Mustache : }";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }


        [StrinkenTest]
        public void ParseOutsideString_StringWithOneTokenStartIndicator_ReturnsPartBeforeTokenStartIndicator()
        {
            var input = "Mustache : { !";
            var expected = "Mustache : ";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithOneTokenStartIndicatorAtEnd_ReturnsPartBeforeTokenStartIndicator()
        {
            var input = "Mustache : {";
            var expected = "Mustache : ";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeTrue();
                parsedStringResult.Value.Should().Be(expected);
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithOneTokenEndIndicator_ReturnsFailure()
        {
            var input = "Mustache : } !";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at position 11");
            }
        }

        [StrinkenTest]
        public void ParseOutsideString_StringWithOneTokenStartIndicatorAtEnd_ReturnsFailure()
        {
            var input = "Mustache : }";
            using (var cursor = new Cursor(input))
            {
                var parsedStringResult = cursor.ParseOutsideString();

                parsedStringResult.Result.Should().BeFalse();
                parsedStringResult.Message.Should().Be("Illegal '}' at the end of the string");
            }
        }
    }
}