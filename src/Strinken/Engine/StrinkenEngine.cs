// stylecop.header
using System.Text;
using Strinken.Common;
using Strinken.Machine;

namespace Strinken.Engine
{
    /// <summary>
    /// Core engine of Strinken.
    /// </summary>
    internal class StrinkenEngine
    {
        /// <summary>
        /// The cursor used to read the string.
        /// </summary>
        private Cursor cursor;

        /// <summary>
        /// The error message in case of failure.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// The <see cref="StringBuilder"/> used store the current token.
        /// </summary>
        private StringBuilder token;

        /// <summary>
        /// The stack of tokens.
        /// </summary>
        private TokenStack tokenStack;

        /// <summary>
        /// Run the engine on a string.
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <returns>A <see cref="EngineResult"/> containing data about the run.</returns>
        public EngineResult Run(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                var stack = new TokenStack();
                stack.Push(input, TokenType.VerbatimString);
                return new EngineResult
                {
                    Success = true,
                    Stack = stack,
                    ErrorMessage = null
                };
            }

            this.token = new StringBuilder();
            this.tokenStack = new TokenStack();
            this.errorMessage = null;

            using (this.cursor = new Cursor(input))
            {
                var machine = StateMachineBuilder
                    .Initialize()
                    .StartOn(State.OutsideToken)
                    .StopOn(State.EndOfString)
                    .BeforeEachStep(this.cursor.Next)
                    .On(State.OutsideToken).Do(this.ProcessOutsideToken)
                    .On(State.OnOpenBracket).Do(this.ProcessOnOpenBracket)
                    .On(State.InsideTag).Do(this.ProcessInsideTag)
                    .On(State.OnCloseBracket).Do(this.ProcessOnCloseBracket)
                    .On(State.OnFilterSeparator).Do(this.ProcessOnFilterSeparator)
                    .On(State.InsideFilter).Do(this.ProcessInsideFilter)
                    .On(State.OnArgumentInitializer).Do(this.ProcessOnArgumentInitializer)
                    .On(State.OnArgumentSeparator).Do(this.ProcessOnArgumentSeparator)
                    .On(State.InsideArgument).Do(this.ProcessInsideArgument)
                    .On(State.OnArgumentTagIndicator).Do(this.ProcessOnArgumentTagIndicator)
                    .On(State.InsideArgumentTag).Do(this.ProcessInsideArgumentTag)
                    .On(State.InvalidString).Sink()
                    .Build();

                var success = machine.Run();
                return new EngineResult
                {
                    Success = success,
                    Stack = success ? this.tokenStack : null,
                    ErrorMessage = this.errorMessage
                };
            }
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgument"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessInsideArgument()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.ArgumentSeparator:
                    state = State.OnArgumentSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Argument);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.FilterSeparator:
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Argument);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    this.tokenStack.Push(this.token.ToString(), TokenType.Argument);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    this.token.Append((char)this.cursor.Value);
                    state = State.InsideArgument;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgumentTag"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessInsideArgumentTag()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.ArgumentSeparator:
                    state = State.OnArgumentSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.FilterSeparator:
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideArgumentTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideFilter"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessInsideFilter()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.FilterSeparator:
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.ArgumentIndicator:
                    state = State.OnArgumentInitializer;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideFilter);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideTag"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessInsideTag()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.FilterSeparator:
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Tag);
                    this.token.Length = 0;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    this.tokenStack.Push(this.token.ToString(), TokenType.Tag);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentInitializer"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnArgumentInitializer()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                case SpecialCharacter.ArgumentSeparator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EmptyArgument;
                    break;

                case SpecialCharacter.ArgumentTagIndicator:
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    this.token.Append((char)this.cursor.Value);
                    state = State.InsideArgument;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentSeparator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnArgumentSeparator()
        {
            var state = State.InsideArgument;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                case SpecialCharacter.ArgumentSeparator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EmptyArgument;
                    break;

                case SpecialCharacter.ArgumentTagIndicator:
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    this.token.Append((char)this.cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentTagIndicator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnArgumentTagIndicator()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EmptyArgument;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideArgumentTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnCloseBracket"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnCloseBracket()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.TokenEndIndicator:
                    // Escaped TokenEnd
                    this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenEndIndicator);
                    state = State.OutsideToken;
                    break;

                default:
                    state = State.InvalidString;
                    this.errorMessage = string.Format(Errors.IllegalCharacter, (char)SpecialCharacter.TokenEndIndicator, this.cursor.Position - 1);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnFilterSeparator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnFilterSeparator()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EndOfString;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EmptyFilter;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideFilter);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnOpenBracket"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnOpenBracket()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.InvalidString;
                    this.errorMessage = string.Format(Errors.IllegalCharacterAtStringEnd, (char)SpecialCharacter.TokenStartIndicator);
                    break;

                case SpecialCharacter.FilterSeparator:
                case SpecialCharacter.TokenEndIndicator:
                    state = State.InvalidString;
                    this.errorMessage = Errors.EmptyTag;
                    break;

                case SpecialCharacter.TokenStartIndicator:
                    // Escaped TokenStart
                    this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenStartIndicator);
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateAndAppend(State.InsideTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OutsideToken"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOutsideToken()
        {
            var state = State.OutsideToken;

            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.EndOfString;
                    break;

                case SpecialCharacter.TokenStartIndicator:
                    state = State.OnOpenBracket;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    state = State.OnCloseBracket;
                    break;

                default:
                    this.tokenStack.PushVerbatim((char)this.cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Validates the current <see cref="Cursor"/> and append its value to the current token if valid.
        /// </summary>
        /// <param name="stateIfValid">The <see cref="State"/> to return if the cursor is valid..</param>
        /// <returns>The new state.</returns>
        private State ValidateAndAppend(State stateIfValid)
        {
            var value = (char)this.cursor.Value;
            if (value.IsInvalidTokenNameCharacter())
            {
                this.errorMessage = string.Format(Errors.IllegalCharacter, (char)this.cursor.Value, this.cursor.Position);
                return State.InvalidString;
            }
            else
            {
                this.token.Append((char)this.cursor.Value);
                return stateIfValid;
            }
        }
    }
}