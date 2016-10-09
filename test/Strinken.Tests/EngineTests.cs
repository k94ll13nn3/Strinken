using NUnit.Framework;
using Strinken.Engine;

namespace Strinken.Tests
{
    [TestFixture]
    public class EngineTests
    {
        private StrinkenEngine engine;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.engine = new StrinkenEngine();
        }

        [Test]
        public void Run_NullInput_ReturnsNull()
        {
            var result = this.engine.Run(null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
            Assert.That(result.Stack.Resolve(null), Is.Null);
        }

        [Test]
        public void Run_EmptyInput_ReturnsEmptyString()
        {
            var result = this.engine.Run(string.Empty);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
            Assert.That(result.Stack.Resolve(null), Is.Empty);
        }

        [Test]
        public void Run_OpenBracketAtStringEnd_ThrowsFormatException()
        {
            const string input = "lorem ipsum{";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '{' at the end of the string"));
        }

        [Test]
        public void Run_EmptyTag_ThrowsFormatException()
        {
            const string input = "lorem{}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty tag"));
        }

        [Test]
        public void Run_TagNotClosed_ThrowsFormatException()
        {
            const string input = "lorem{a";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_OpenBracketInTag_ThrowsFormatException()
        {
            const string input = "lorem{test{tm";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '{' at position 10"));
        }

        [Test]
        public void Run_EmptyFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty filter"));
        }

        [Test]
        public void Run_EmptyFirstArgument_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptySecondArgument_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+arg,}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptySecondArgumentAndThreeArgument_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+someThing,,}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptyFirstAndSecond_ThrowsFormatException()
        {
            const string input = "lorem{tag:filter+,}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptyArgumentTag_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+arg,=}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_OpenBracketInsideFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:a{r}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '{' at position 12"));
        }

        [Test]
        public void Run_FilterSeparatorInsideFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:a:}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Empty filter"));
        }

        [Test]
        public void Run_TwoArgumentTagSeparator_ThrowsFormatException()
        {
            const string input = "lorem{test:a+==}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '=' at position 14"));
        }

        [Test]
        public void Run_CloseBracketOutisdeToken_ThrowsFormatException()
        {
            const string input = "lorem}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '}' at position 5"));
        }

        [Test]
        public void Run_EndOfStringOnFilterSeparator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_FilterNotClosed_ThrowsFormatException()
        {
            const string input = "lorem{ispum:abc";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [TestCase("lorem{ispum:abc+p")]
        [TestCase("lorem{ispum:abc+p,t")]
        public void Run_ArgumentNotClosed_ThrowsFormatException(string input)
        {
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentSeparator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentTagIndicator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,=";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringInsideArgumentTag_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,=a";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentInitializer_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_ArgumentAfterTag_ThrowsFormatException()
        {
            const string input = "lorem{ispum+arg}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Illegal '+' at position 11"));
        }

        [Test]
        public void Run_ValidString_DoesNotThrow()
        {
            const string input = "lorem{ispum:abc}";
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [TestCase("lorem{isp+um:abc}", '+')]
        [TestCase("lorem{isp°um:abc}", '°')]
        [TestCase("lorem{isp um:abc}", ' ')]
        public void Run_InvalidCharacterInString_ThrowsFormatException(string input, char illegalChar)
        {
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"Illegal '{illegalChar}' at position 9"));
        }

        [TestCase("lorem{ispum:abc}")]
        [TestCase("lorem{ispAum:abc+9}")]
        [TestCase("lorem{ispAum:abc+arg, 9}")]
        [TestCase("lorem{ispSIK_}")]
        [TestCase("lorem{JuF-_}")]
        [TestCase("lorem{JuF-m}09à9")]
        [TestCase("lorem{!JuF-m}09à9")]
        public void Run_ValidCharacterInString_DoesNotThrow(string input)
        {
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [TestCase("{lorem:ispum+abc}")]
        [TestCase("{lorem:ispum+abc9}")]
        [TestCase("{lorem:ispum+abc-+}")]
        public void Run_ArgumentValue_ChecksCharacters(string input)
        {
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [TestCase("{lorem:ispum+=abc9}", '9', 17)]
        [TestCase("{lorem:ispum+=abc-+}", '+', 18)]
        [TestCase("{lorem:ispum+=abc*}", '*', 17)]
        public void Run_ArgumentTagValue_ChecksCharacters(string input, char illegalChar, int position)
        {
            var result = this.engine.Run(input);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"Illegal '{illegalChar}' at position {position}"));
        }
    }
}