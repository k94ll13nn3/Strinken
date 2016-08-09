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
        /// <returns>The parsed string.</returns>
        /// <exception cref="FormatException">When the input is not valid.</exception>
        public string Run(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            using (var parameters = new EngineParameters(input, this.actionOnTags, this.actionOnFilters))
            {
                var machine = StateMachineBuilder
                    .Initialize()
                    .StartOn(State.OutsideToken)
                    .StopOn(State.EndOfString)
                    .BeforeEachStep(parameters.Cursor.Next)
                    .On(State.OutsideToken).Do(() => ProcessOutsideToken(parameters))
                    .On(State.OnOpenBracket).Do(() => ProcessOnOpenBracket(parameters))
                    .On(State.InsideTag).Do(() => ProcessInsideTag(parameters))
                    .On(State.OnCloseBracket).Do(() => ProcessOnCloseBracket(parameters))
                    .On(State.OnFilterSeparator).Do(() => ProcessOnFilterSeparator(parameters))
                    .On(State.InsideFilter).Do(() => ProcessInsideFilter(parameters))
                    .On(State.OnArgumentInitializer).Do(() => ProcessOnArgumentInitializer(parameters))
                    .On(State.OnArgumentSeparator).Do(() => ProcessOnArgumentSeparator(parameters))
                    .On(State.InsideArgument).Do(() => ProcessInsideArgument(parameters))
                    .On(State.OnArgumentTagIndicator).Do(() => ProcessOnArgumentTagIndicator(parameters))
                    .On(State.InsideArgumentTag).Do(() => ProcessInsideArgumentTag(parameters))
                    .Build();

                machine.Run();

                return parameters.Result.ToString();
            }
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgument"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideArgument(EngineParameters parameters)
        {
            var state = State.InsideArgument;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
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
            var state = State.InsideArgumentTag;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
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
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
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
            var state = State.InsideFilter;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
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
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
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
            var state = State.InsideTag;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
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
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
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
            var state = State.InsideArgument;
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");
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
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");

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
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");

                default:
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    break;
            }

            return State.InsideArgumentTag;
        }

        /// <summary>
        /// Processes the <see cref="State.OnCloseBracket"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnCloseBracket(EngineParameters parameters)
        {
            switch (parameters.Cursor.Value)
            {
                case '}':
                    // Escaped '}'
                    parameters.Result.Append('}');
                    break;

                default:
                    throw new FormatException($"Illegal '}}' at position {parameters.Cursor.Position - 1}");
            }

            return State.OutsideToken;
        }

        /// <summary>
        /// Processes the <see cref="State.OnFilterSeparator"/> state.
        /// </summary>
        /// <param name="parameters">The parameters used by the engine to process a string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnFilterSeparator(EngineParameters parameters)
        {
            switch (parameters.Cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty filter");
                default:
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    break;
            }

            return State.InsideFilter;
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
                    throw new FormatException("Illegal '{' at the end of the string");
                case ':':
                case '}':
                    throw new FormatException("Empty tag");
                case '{':
                    // Escaped '{'
                    parameters.Result.Append('{');
                    state = State.OutsideToken;
                    break;

                default:
                    ThrowIfInvalidCharacter(parameters.Cursor);
                    parameters.Token.Append((char)parameters.Cursor.Value);
                    state = State.InsideTag;
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
        /// Validates a <see cref="Cursor"/> and throws a <see cref="FormatException"/> if the data in the cursor is invalid.
        /// </summary>
        /// <param name="cursor">The <see cref="Cursor"/> to validate.</param>
        private static void ThrowIfInvalidCharacter(Cursor cursor)
        {
            var value = (char)cursor.Value;
            if (value.IsValidTokenNameCharacter())
            {
                throw new FormatException($"Illegal '{(char)cursor.Value}' at position {cursor.Position}");
            }
        }
    }
}