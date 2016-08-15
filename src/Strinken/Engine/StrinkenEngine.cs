// stylecop.header
using System;
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
        /// Action to perform on filters. The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </summary>
        private readonly Func<string, string, string[], string> actionOnFilters;

        /// <summary>
        /// Action to perform on tags. The argument is the tag name.
        /// </summary>
        private readonly Func<string, string> actionOnTags;

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
        /// Initializes a new instance of the <see cref="StrinkenEngine"/> class.
        /// </summary>
        /// <param name="actionOnTags">Action to perform on tags. The argument is the tag name.</param>
        /// <param name="actionOnFilters">
        ///     Action to perform on filters.
        ///     The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </param>
        public StrinkenEngine(Func<string, string> actionOnTags, Func<string, string, string[], string> actionOnFilters)
        {
            this.actionOnTags = actionOnTags;
            this.actionOnFilters = actionOnFilters;
        }

        /// <summary>
        /// Run the engine on a string.
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <returns>A <see cref="EngineResult"/> containing data about the run.</returns>
        /// <exception cref="FormatException">When the input is not valid.</exception>
        public EngineResult Run(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new EngineResult
                {
                    Success = true,
                    ParsedString = input,
                    ErrorMessage = null
                };
            }

            this.token = new StringBuilder();
            this.tokenStack = new TokenStack(this.actionOnTags, this.actionOnFilters);
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
                    ParsedString = this.tokenStack.Resolve(),
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case ',':
                    state = State.OnArgumentSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Argument);
                    this.token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Argument);
                    this.token.Length = 0;
                    break;

                case '}':
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case ',':
                    state = State.OnArgumentSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    break;

                case '}':
                    this.tokenStack.Push(this.token.ToString(), TokenType.ArgumentTag);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateCursor(State.InsideArgumentTag);
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    break;

                case '+':
                    state = State.OnArgumentInitializer;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    break;

                case '}':
                    this.tokenStack.Push(this.token.ToString(), TokenType.Filter);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateCursor(State.InsideFilter);
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    this.tokenStack.Push(this.token.ToString(), TokenType.Tag);
                    this.token.Length = 0;
                    break;

                case '}':
                    this.tokenStack.Push(this.token.ToString(), TokenType.Tag);
                    this.token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateCursor(State.InsideTag);
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    this.errorMessage = "Empty argument";
                    break;

                case '=':
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    this.errorMessage = "Empty argument";
                    break;

                case '=':
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    this.errorMessage = "Empty argument";
                    break;

                default:
                    state = this.ValidateCursor(State.InsideArgumentTag);
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
                case '}':
                    // Escaped '}'
                    this.tokenStack.Push("}", TokenType.VerbatimString);
                    state = State.OutsideToken;
                    break;

                default:
                    state = State.InvalidString;
                    this.errorMessage = $"Illegal '}}' at position {this.cursor.Position - 1}";
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    this.errorMessage = "Empty filter";
                    break;

                default:
                    state = this.ValidateCursor(State.InsideFilter);
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
                case -1:
                    state = State.InvalidString;
                    this.errorMessage = "Illegal '{' at the end of the string";
                    break;

                case ':':
                case '}':
                    state = State.InvalidString;
                    this.errorMessage = "Empty tag";
                    break;

                case '{':
                    // Escaped '{'
                    this.tokenStack.Push("{", TokenType.VerbatimString);
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.ValidateCursor(State.InsideTag);
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
                case -1:
                    state = State.EndOfString;
                    break;

                case '{':
                    state = State.OnOpenBracket;
                    break;

                case '}':
                    state = State.OnCloseBracket;
                    break;

                default:
                    this.tokenStack.Push(((char)this.cursor.Value).ToString(), TokenType.VerbatimString);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Validates the current <see cref="Cursor"/>.
        /// </summary>
        /// <param name="stateIfValid">The <see cref="State"/> to return if the cursor is valid..</param>
        /// <returns>The new state.</returns>
        private State ValidateCursor(State stateIfValid)
        {
            var value = (char)this.cursor.Value;
            if (value.IsInvalidTokenNameCharacter())
            {
                this.errorMessage = $"Illegal '{(char)this.cursor.Value}' at position {this.cursor.Position}";
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