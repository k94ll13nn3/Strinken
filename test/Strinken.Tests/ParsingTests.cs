using NUnit.Framework;
using Strinken.Engine;
using System;

namespace Strinken.Tests
{
    [TestFixture]
    public class ParsingTests
    {
        private StrinkenEngine engine;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.engine = new StrinkenEngine(null, null);
        }

        [Test]
        public void Run_NullInput_ThrowsArgumentNullException()
        {
            Assert.That(() => this.engine.Run(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Run_EmptyInput_ThrowsArgumentNullException()
        {
            Assert.That(() => this.engine.Run(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void Run_OpenBracketAtStringEnd_ThrowsFormatException()
        {
            const string input = "lorem ipsum{";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '{' at the end of the string"));
        }

        [Test]
        public void Run_EmptyTag_ThrowsFormatException()
        {
            const string input = "lorem{}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty tag"));
        }

        [Test]
        public void Run_TagNotClosed_ThrowsFormatException()
        {
            const string input = "lorem{a";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_OpenBracketInTag_ThrowsFormatException()
        {
            const string input = "lorem{test{tm";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '{' at position 10"));
        }

        [Test]
        public void Run_EmptyFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty filter"));
        }

        [Test]
        public void Run_EmptyFirstArgument_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptySecondArgument_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+arg,}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_EmptyArgumentTag_ThrowsFormatException()
        {
            const string input = "lorem{test:filter+arg,=}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty argument"));
        }

        [Test]
        public void Run_OpenBracketInsideFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:a{r}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '{' at position 12"));
        }

        [Test]
        public void Run_FilterSeparatorInsideFilter_ThrowsFormatException()
        {
            const string input = "lorem{test:a:}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Empty filter"));
        }

        [Test]
        public void Run_TwoArgumentTagSeparator_ThrowsFormatException()
        {
            const string input = "lorem{test:a+==}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '=' at position 14"));
        }

        [Test]
        public void Run_CloseBracketOutisdeToken_ThrowsFormatException()
        {
            const string input = "lorem}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '}' at position 5"));
        }

        [Test]
        public void Run_EndOfStringOnFilterSeparator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_FilterNotClosed_ThrowsFormatException()
        {
            const string input = "lorem{ispum:abc";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_ArgumentNotClosed_ThrowsFormatException()
        {
            Assert.That(() => this.engine.Run("lorem{ispum:abc+p"), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
            Assert.That(() => this.engine.Run("lorem{ispum:abc+p,t"), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentSeparator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentTagIndicator_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,=";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringInsideArgumentTag_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+h,=a";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_EndOfStringOnArgumentInitializer_ThrowsFormatException()
        {
            const string input = "lorem{ispum:tot+";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("End of string reached while inside a token"));
        }

        [Test]
        public void Run_ArgumentAfterTag_ThrowsFormatException()
        {
            const string input = "lorem{ispum+arg}";
            Assert.That(() => this.engine.Run(input), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '+' at position 11"));
        }

        [Test]
        public void Run_ValidString_DoesNotThrow()
        {
            const string input = "lorem{ispum:abc}";
            Assert.That(() => this.engine.Run(input), Throws.Nothing);
        }

        [Test]
        public void Run_InvalidCharacterInString_ThrowsFormatException()
        {
            Assert.That(() => this.engine.Run("lorem{isp+um:abc}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '+' at position 9"));
            Assert.That(() => this.engine.Run("lorem{isp°um:abc}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '°' at position 9"));
            Assert.That(() => this.engine.Run("lorem{isp um:abc}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal ' ' at position 9"));
        }

        [TestCase("lorem{ispum:abc}")]
        [TestCase("lorem{ispAum:abc+9}")]
        [TestCase("lorem{ispAum:abc+arg, 9}")]
        [TestCase("lorem{ispSIK_}")]
        [TestCase("lorem{JuF-_}")]
        [TestCase("lorem{JuF-m}09à9")]
        public void Run_ValidCharacterInString_DoesNotThrow(string value)
        {
            Assert.That(() => this.engine.Run(value), Throws.Nothing);
        }

        [TestCase("{lorem:ispum+abc}")]
        [TestCase("{lorem:ispum+abc9}")]
        [TestCase("{lorem:ispum+abc-+}")]
        public void Run_ArgumentValue_ChecksCharacters(string value)
        {
            Assert.That(() => this.engine.Run(value), Throws.Nothing);
        }

        [Test]
        public void Run_ArgumentTagValue_ChecksCharacters()
        {
            Assert.That(() => this.engine.Run("{lorem:ispum+=abc}"), Throws.Nothing);
            Assert.That(() => this.engine.Run("{lorem:ispum+=abc9}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '9' at position 17"));
            Assert.That(() => this.engine.Run("{lorem:ispum+=abc-+}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '+' at position 18"));
            Assert.That(() => this.engine.Run("{lorem:ispum+=abc*}"), Throws.TypeOf<FormatException>().With.Message.EqualTo("Illegal '*' at position 17"));
        }
    }
}