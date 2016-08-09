// stylecop.header
using System;
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

            using (var parameters = new EngineParameters(input, this.actionOnTags, this.actionOnFilters))
            {
                var machine = StateMachineBuilder<State, EngineParameters>
                    .Initialize()
                    .StartOn(State.OutsideToken)
                    .StopOn(State.EndOfString)
                    .BeforeEachStep(parameters.Cursor.Next)
                    .On(State.OutsideToken).Do(ProcessOutsideToken)
                    .On(State.OnOpenBracket).Do(ProcessOnOpenBracket)
                    .On(State.InsideTag).Do(ProcessInsideTag)
                    .On(State.OnCloseBracket).Do(ProcessOnCloseBracket)
                    .On(State.OnFilterSeparator).Do(ProcessOnFilterSeparator)
                    .On(State.InsideFilter).Do(ProcessInsideFilter)
                    .On(State.OnArgumentInitializer).Do(ProcessOnArgumentInitializer)
                    .On(State.OnArgumentSeparator).Do(ProcessOnArgumentSeparator)
                    .On(State.InsideArgument).Do(ProcessInsideArgument)
                    .On(State.OnArgumentTagIndicator).Do(ProcessOnArgumentTagIndicator)
                    .On(State.InsideArgumentTag).Do(ProcessInsideArgumentTag)
                    .On(State.InvalidString).Sink()
                    .Build();

                var success = machine.Run(parameters);
                var result = new EngineResult
                {
                    Success = success,
                    ParsedString = parameters.Result.ToString(),
                    ErrorMessage = parameters.ErrorMessage
                };
                return result;
            }
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgument"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideArgument(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case ',':
                    state = State.OnArgumentSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Argument);
                    parameters.Token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Argument);
                    parameters.Token.Length = 0;
                    break;

                case '}':
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Argument);
                    parameters.Result.Append(parameters.TokenStack.Resolve());
                    parameters.Token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    state = State.InsideArgument;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgumentTag"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideArgumentTag(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case ',':
                    state = State.OnArgumentSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.ArgumentTag);
                    parameters.Token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.ArgumentTag);
                    parameters.Token.Length = 0;
                    break;

                case '}':
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.ArgumentTag);
                    parameters.Result.Append(parameters.TokenStack.Resolve());
                    parameters.Token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideArgumentTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideFilter"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideFilter(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Filter);
                    parameters.Token.Length = 0;
                    break;

                case '+':
                    state = State.OnArgumentInitializer;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Filter);
                    parameters.Token.Length = 0;
                    break;

                case '}':
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Filter);
                    parameters.Result.Append(parameters.TokenStack.Resolve());
                    parameters.Token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideFilter);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideTag"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideTag(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Tag);
                    parameters.Token.Length = 0;
                    break;

                case '}':
                    parameters.TokenStack.Push(parameters.Token.ToString(), TokenType.Tag);
                    parameters.Result.Append(parameters.TokenStack.Resolve());
                    parameters.Token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentInitializer"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentInitializer(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Empty argument";
                    break;

                case '=':
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    state = State.InsideArgument;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentSeparator"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentSeparator(EngineParameters parameters)
        {
            var state = State.InsideArgument;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Empty argument";
                    break;

                case '=':
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentTagIndicator"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentTagIndicator(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Empty argument";
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideArgumentTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnCloseBracket"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnCloseBracket(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case '}':
                    // Escaped '}'
                    parameters.Result.Append('}');
                    state = State.OutsideToken;
                    break;

                default:
                    state = State.InvalidString;
                    parameters.ErrorMessage = $"Illegal '}}' at position {parameters.Cursor.Position - 1}";
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnFilterSeparator"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnFilterSeparator(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "End of string reached while inside a token";
                    break;

                case '}':
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Empty filter";
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideFilter);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnOpenBracket"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnOpenBracket(EngineParameters parameters)
        {
            State state;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Illegal '{' at the end of the string";
                    break;
                case ':':
                case '}':
                    state = State.InvalidString;
                    parameters.ErrorMessage = "Empty tag";
                    break;

                case '{':
                    // Escaped '{'
                    parameters.Result.Append('{');
                    state = State.OutsideToken;
                    break;

                default:
                    state = ValidateCursor(parameters, State.InsideTag);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OutsideToken"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOutsideToken(EngineParameters parameters)
        {
            var state = State.OutsideToken;

            switch (parameters.Cursor.Value)
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
                    parameters.Result.Append((char)parameters.Cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Validates the current <see cref="Cursor"/> of a <see cref="EngineParameters"/>.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <param name="stateIfValid">The <see cref="State"/> to return if the cursor is valid..</param>
        /// <returns>The new state.</returns>
        private static State ValidateCursor(EngineParameters parameters, State stateIfValid)
        {
            var value = (char)parameters.Cursor.Value;
            if (value.IsInvalidTokenNameCharacter())
            {
                parameters.ErrorMessage = $"Illegal '{(char)parameters.Cursor.Value}' at position {parameters.Cursor.Position}";
                return State.InvalidString;
            }
            else
            {
                parameters.Token.Append((char)parameters.Cursor.Value);
                return stateIfValid;
            }
        }
    }
}