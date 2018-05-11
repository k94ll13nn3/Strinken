using FluentAssertions;
using Strinken.Core;
using Xunit;

namespace Strinken.Tests
{
    public class EngineTests
    {
        [Fact]
        public void Run_NullInput_ReturnsNull()
        {
            EngineResult result = StrinkenEngine.Run(null);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            result.Stack.Resolve(null).Should().BeNull();
        }

        [Fact]
        public void Run_EmptyInput_ReturnsEmptyString()
        {
            EngineResult result = StrinkenEngine.Run(string.Empty);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
            result.Stack.Resolve(null).Should().BeEmpty();
        }

        [Fact]
        public void Run_OpenBracketAtStringEnd_ReturnsFalse()
        {
            const string input = "lorem ipsum{";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_EmptyTag_ReturnsFalse()
        {
            const string input = "lorem{}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty tag");
        }

        [Fact]
        public void Run_TagNotClosed_ReturnsFalse()
        {
            const string input = "lorem{a";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_OpenBracketInTag_ReturnsFalse()
        {
            const string input = "lorem{test{tm";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '{' at position 10");
        }

        [Fact]
        public void Run_EmptyFilter_ReturnsFalse()
        {
            const string input = "lorem{test:}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty filter");
        }

        [Fact]
        public void Run_EmptyFirstArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [Fact]
        public void Run_EmptySecondArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+arg,}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [Fact]
        public void Run_EmptySecondArgumentAndThreeArgument_ReturnsFalse()
        {
            const string input = "lorem{test:filter+someThing,,}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [Fact]
        public void Run_EmptyFirstAndSecond_ReturnsFalse()
        {
            const string input = "lorem{tag:filter+,}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [Fact]
        public void Run_EmptyArgumentTag_ReturnsFalse()
        {
            const string input = "lorem{test:filter+arg,=}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty argument");
        }

        [Fact]
        public void Run_OpenBracketInsideFilter_ReturnsFalse()
        {
            const string input = "lorem{test:a{r}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '{' at position 12");
        }

        [Fact]
        public void Run_FilterSeparatorInsideFilter_ReturnsFalse()
        {
            const string input = "lorem{test:a:}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Empty filter");
        }

        [Fact]
        public void Run_TwoArgumentTagSeparator_ReturnsFalse()
        {
            const string input = "lorem{test:a+==}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '=' at position 14");
        }

        [Fact]
        public void Run_CloseBracketOutisdeToken_ReturnsFalse()
        {
            const string input = "lorem}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '}' at the end of the string");
        }

        [Fact]
        public void Run_EndOfStringOnFilterSeparator_ReturnsFalse()
        {
            const string input = "lorem{ispum:";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_FilterNotClosed_ReturnsFalse()
        {
            const string input = "lorem{ispum:abc";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Theory]
        [InlineData("lorem{ispum:abc+p")]
        [InlineData("lorem{ispum:abc+p,t")]
        public void Run_ArgumentNotClosed_ReturnsFalse(string input)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_EndOfStringOnArgumentSeparator_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_EndOfStringOnArgumentTagIndicator_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,=";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_EndOfStringInsideArgumentTag_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+h,=a";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_EndOfStringOnArgumentInitializer_ReturnsFalse()
        {
            const string input = "lorem{ispum:tot+";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("End of string reached while inside a token");
        }

        [Fact]
        public void Run_ArgumentAfterTag_ReturnsFalse()
        {
            const string input = "lorem{ispum+arg}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '+' at position 11");
        }

        [Fact]
        public void Run_ValidString_DoesNotThrow()
        {
            const string input = "lorem{ispum:abc}";
            EngineResult result = StrinkenEngine.Run(input);
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
            EngineResult result = StrinkenEngine.Run(input);
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
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("{ipsum:filt*er}", '*', 11)]
        [InlineData("{ipsum:fi!}", '!', 9)]
        [InlineData("{ipsum:?f}", '?', 7)]
        [InlineData("{ipsum:!!µ}", 'µ', 9)]
        public void Run_InvalidCharacterInFilterName_ReturnsFalse(string input, char illegalChar, int position)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Illegal '{illegalChar}' at position {position}");
        }

        [Theory]
        [InlineData("{ipsum:test}")]
        [InlineData("{ipsum:Fil1r}")]
        [InlineData("{ipsum:Soe_fi}")]
        [InlineData("{ipsum:%!*}")]
        [InlineData("{ipsum:??}")]
        [InlineData("{ipsum:?}")]
        [InlineData("{ipsum:#!*$}")]
        public void Run_ValidCharacterInFilterName_DoesNotThrow(string input)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("{lorem:ispum+abc}")]
        [InlineData("{lorem:ispum+abc9}")]
        [InlineData("{lorem:ispum+abc-+}")]
        public void Run_ArgumentValue_ChecksCharacters(string input)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Theory]
        [InlineData("{lorem:ispum+=abc9}", '9', 17)]
        [InlineData("{lorem:ispum+=abc-+}", '+', 18)]
        [InlineData("{lorem:ispum+=abc*}", '*', 17)]
        public void Run_ArgumentTagValue_ChecksCharacters(string input, char illegalChar, int position)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be($"Illegal '{illegalChar}' at position {position}");
        }

        [Fact]
        public void Run_NumberTagBinaryFormatInvalidCharacter_ReturnsFalse()
        {
            const string input = "lorem{#b100105}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '5' at position 13");
        }

        [Fact]
        public void Run_NumberTagOctalFormatInvalidCharacter_ReturnsFalse()
        {
            const string input = "lorem{#o1224563945}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal '9' at position 15");
        }

        [Fact]
        public void Run_NumberTagDecimalFormatInvalidCharacter_ReturnsFalse()
        {
            const string input = "lorem{#d154A452}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal 'A' at position 11");
        }

        [Fact]
        public void Run_NumberTagHexadecimalFormatInvalidCharacter_ReturnsFalse()
        {
            const string input = "lorem{#x45Aee1f45GH5}";
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Illegal 'G' at position 17");
        }

        [Theory]
        [InlineData("{@sfklgjfg=:ispum}")]
        [InlineData("{@sdf,sfg:ispum}")]
        [InlineData("{@!sfgf:ispum}")]
        public void Run_ValueTag_ChecksCharacters(string input)
        {
            EngineResult result = StrinkenEngine.Run(input);
            result.Success.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }
    }
}