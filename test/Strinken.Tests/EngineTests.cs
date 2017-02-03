using FluentAssertions;
using Strinken.Engine;
using Xunit;

namespace Strinken.Tests
{
    public class EngineTests
    {
        [StrinkenTest]
        public void Run_NullInput_ReturnsNull()
        {
            var result = new StrinkenEngine().Run(null);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            result.Stack.Resolve(null).Should().BeNull();
        }

        [StrinkenTest]
        public void Run_EmptyInput_ReturnsEmptyString()
        {
            var result = new StrinkenEngine().Run(string.Empty);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            result.Stack.Resolve(null).Should().BeEmpty();
        }

        [StrinkenTest]
        public void Run_OpenBracketAtStringEnd_ReturnsFalse()
        {
            const string input = "lorem ipsum{";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '{' at the end of the string");
        }

        [StrinkenTest]
        public void Run_EmptyTag_ReturnsFalse()
        {
            const string input = "lorem{}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty tag");
        }

        [StrinkenTest]
        public void Run_TagNotClosed_ReturnsFalse()
        {
            const string input = "lorem{a";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_OpenBracketInTag_ReturnsFalse()
        {
            const string input = "lorem{test{tm";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '{' at position 10");
        }

        [StrinkenTest]
        public void Run_EmptyFilter_ReturnsFalse()
        {
            const string input = "lorem{test:}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty filter");
        }

        [StrinkenTest]
        public void Run_EmptyFirstArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [StrinkenTest]
        public void Run_EmptySecondArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+arg,}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [StrinkenTest]
        public void Run_EmptySecondArgumentAndThreeArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+someThing,,}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [StrinkenTest]
        public void Run_EmptyFirstAndSecond_ReturnsFalse()
        {
            const string input = "lorem{tag:filter+,}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [StrinkenTest]
        public void Run_EmptyArgumentTag_ReturnsFalse()
        {
            const string input = "lorem{test:filter+arg,=}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [StrinkenTest]
        public void Run_OpenBracketInsideFilter_ReturnsFalse()
        {
            const string input = "lorem{test:a{r}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '{' at position 12");
        }

        [StrinkenTest]
        public void Run_FilterSeparatorInsideFilter_ReturnsFalse()
        {
            const string input = "lorem{test:a:}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty filter");
        }

        [StrinkenTest]
        public void Run_TwoArgumentTagSeparator_ReturnsFalse()
        {
            const string input = "lorem{test:a+==}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '=' at position 14");
        }

        [StrinkenTest]
        public void Run_CloseBracketOutisdeToken_ReturnsFalse()
        {
            const string input = "lorem}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '}' at position 5");
        }

        [StrinkenTest]
        public void Run_EndOfStringOnFilterSeparator_ReturnsFalse()
        {
            const string input = "lorem{ispum:";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_FilterNotClosed_ReturnsFalse()
        {
            const string input = "lorem{ispum:abc";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Theory]
        [InlineData("lorem{ispum:abc+p")]
        [InlineData("lorem{ispum:abc+p,t")]
        public void Run_ArgumentNotClosed_ReturnsFalse(string input)
        {
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_EndOfStringOnArgumentSeparator_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_EndOfStringOnArgumentTagIndicator_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,=";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_EndOfStringInsideArgumentTag_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,=a";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_EndOfStringOnArgumentInitializer_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [StrinkenTest]
        public void Run_ArgumentAfterTag_ReturnsFalse()
        {
            const string input = "lorem{ispum+arg}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '+' at position 11");
        }

        [StrinkenTest]
        public void Run_ValidString_DoesNotThrow()
        {
            const string input = "lorem{ispum:abc}";
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("lorem{isp+um:abc}", '+')]
        [InlineData("lorem{isp°um:abc}", '°')]
        [InlineData("lorem{isp um:abc}", ' ')]
        [InlineData("lorem{is:!fil}", '!')]
        public void Run_InvalidCharacterInString_ReturnsFalse(string input, char illegalChar)
        {
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Illegal '{illegalChar}' at position 9");
        }

        [Theory]
        [InlineData("lorem{ispum:abc}")]
        [InlineData("lorem{ispAum:abc+9}")]
        [InlineData("lorem{ispAum:abc+arg, 9}")]
        [InlineData("lorem{ispSIK_}")]
        [InlineData("lorem{JuF-_}")]
        [InlineData("lorem{JuF-m}09à9")]
        [InlineData("lorem{!JuF-m}09à9")]
        public void Run_ValidCharacterInString_DoesNotThrow(string input)
        {
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("{lorem:ispum+abc}")]
        [InlineData("{lorem:ispum+abc9}")]
        [InlineData("{lorem:ispum+abc-+}")]
        public void Run_ArgumentValue_ChecksCharacters(string input)
        {
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("{lorem:ispum+=abc9}", '9', 17)]
        [InlineData("{lorem:ispum+=abc-+}", '+', 18)]
        [InlineData("{lorem:ispum+=abc*}", '*', 17)]
        public void Run_ArgumentTagValue_ChecksCharacters(string input, char illegalChar, int position)
        {
            var result = new StrinkenEngine().Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Illegal '{illegalChar}' at position {position}");
        }
    }
}