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

            using (var cursor = new Cursor(input))
            {
                var result = new StringBuilder();
                var token = new StringBuilder();
                var tokenStack = new TokenStack(this.actionOnTags, this.actionOnFilters);

                var machine = StateMachineBuilder
                    .Initialize()
                    .StartOn(State.OutsideToken)
                    .StopOn(State.EndOfString)
                    .BeforeEachStep(cursor.Next)
                    .On(State.OutsideToken).Do(() => ProcessOutsideToken(cursor, result))
                    .On(State.OnOpenBracket).Do(() => ProcessOnOpenBracket(cursor, result, token))
                    .On(State.InsideTag).Do(() => ProcessInsideTag(cursor, result, token, tokenStack))
                    .On(State.OnCloseBracket).Do(() => ProcessOnCloseBracket(cursor, result))
                    .On(State.OnFilterSeparator).Do(() => ProcessOnFilterSeparator(cursor, token))
                    .On(State.InsideFilter).Do(() => ProcessInsideFilter(cursor, result, token, tokenStack))
                    .On(State.OnArgumentInitializer).Do(() => ProcessOnArgumentInitializer(cursor, token))
                    .On(State.OnArgumentSeparator).Do(() => ProcessOnArgumentSeparator(cursor, token))
                    .On(State.InsideArgument).Do(() => ProcessInsideArgument(cursor, result, token, tokenStack))
                    .On(State.OnArgumentTagIndicator).Do(() => ProcessOnArgumentTagIndicator(cursor, token))
                    .On(State.InsideArgumentTag).Do(() => ProcessInsideArgumentTag(cursor, result, token, tokenStack))
                    .Build();

                machine.Run();

                return result.ToString();
            }
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgument"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <param name="tokenStack">The stack of tokens.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideArgument(Cursor cursor, StringBuilder result, StringBuilder token, TokenStack tokenStack)
        {
            var state = State.InsideArgument;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case ',':
                    state = State.OnArgumentSeparator;
                    tokenStack.Push(token.ToString(), TokenType.Argument);
                    token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    tokenStack.Push(token.ToString(), TokenType.Argument);
                    token.Length = 0;
                    break;

                case '}':
                    tokenStack.Push(token.ToString(), TokenType.Argument);
                    result.Append(tokenStack.Resolve());
                    token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideArgumentTag"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <param name="tokenStack">The stack of tokens.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideArgumentTag(Cursor cursor, StringBuilder result, StringBuilder token, TokenStack tokenStack)
        {
            var state = State.InsideArgumentTag;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case ',':
                    state = State.OnArgumentSeparator;
                    tokenStack.Push(token.ToString(), TokenType.ArgumentTag);
                    token.Length = 0;
                    break;

                case ':':
                    state = State.OnFilterSeparator;
                    tokenStack.Push(token.ToString(), TokenType.ArgumentTag);
                    token.Length = 0;
                    break;

                case '}':
                    tokenStack.Push(token.ToString(), TokenType.ArgumentTag);
                    result.Append(tokenStack.Resolve());
                    token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideFilter"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <param name="tokenStack">The stack of tokens.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideFilter(Cursor cursor, StringBuilder result, StringBuilder token, TokenStack tokenStack)
        {
            var state = State.InsideFilter;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case ':':
                    state = State.OnFilterSeparator;
                    tokenStack.Push(token.ToString(), TokenType.Filter);
                    token.Length = 0;
                    break;

                case '+':
                    state = State.OnArgumentInitializer;
                    tokenStack.Push(token.ToString(), TokenType.Filter);
                    token.Length = 0;
                    break;

                case '}':
                    tokenStack.Push(token.ToString(), TokenType.Filter);
                    result.Append(tokenStack.Resolve());
                    token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.InsideTag"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <param name="tokenStack">The stack of tokens.</param>
        /// <returns>The new state.</returns>
        private static State ProcessInsideTag(Cursor cursor, StringBuilder result, StringBuilder token, TokenStack tokenStack)
        {
            var state = State.InsideTag;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case ':':
                    state = State.OnFilterSeparator;
                    tokenStack.Push(token.ToString(), TokenType.Tag);
                    token.Length = 0;
                    break;

                case '}':
                    tokenStack.Push(token.ToString(), TokenType.Tag);
                    result.Append(tokenStack.Resolve());
                    token.Length = 0;
                    state = State.OutsideToken;
                    break;

                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentInitializer"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentInitializer(Cursor cursor, StringBuilder token)
        {
            var state = State.InsideArgument;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");
                case '=':
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentSeparator"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentSeparator(Cursor cursor, StringBuilder token)
        {
            var state = State.InsideArgument;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");

                case '=':
                    state = State.OnArgumentTagIndicator;
                    break;

                default:
                    token.Append((char)cursor.Value);
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OnArgumentTagIndicator"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnArgumentTagIndicator(Cursor cursor, StringBuilder token)
        {
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty argument");

                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    break;
            }

            return State.InsideArgumentTag;
        }

        /// <summary>
        /// Processes the <see cref="State.OnCloseBracket"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnCloseBracket(Cursor cursor, StringBuilder result)
        {
            switch (cursor.Value)
            {
                case '}':
                    // Escaped '}'
                    result.Append('}');
                    break;

                default:
                    throw new FormatException($"Illegal '}}' at position {cursor.Position - 1}");
            }

            return State.OutsideToken;
        }

        /// <summary>
        /// Processes the <see cref="State.OnFilterSeparator"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnFilterSeparator(Cursor cursor, StringBuilder token)
        {
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("End of string reached while inside a token");
                case '}':
                    throw new FormatException("Empty filter");
                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    break;
            }

            return State.InsideFilter;
        }

        /// <summary>
        /// Processes the <see cref="State.OnOpenBracket"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <param name="token">The <see cref="StringBuilder"/> used store the current token.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOnOpenBracket(Cursor cursor, StringBuilder result, StringBuilder token)
        {
            State state;
            switch (cursor.Value)
            {
                case -1:
                    throw new FormatException("Illegal '{' at the end of the string");
                case ':':
                case '}':
                    throw new FormatException("Empty tag");
                case '{':
                    // Escaped '{'
                    result.Append('{');
                    state = State.OutsideToken;
                    break;

                default:
                    ThrowIfInvalidCharacter(cursor);
                    token.Append((char)cursor.Value);
                    state = State.InsideTag;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the <see cref="State.OutsideToken"/> state.
        /// </summary>
        /// <param name="cursor">The cursor used to read the string.</param>
        /// <param name="result">The <see cref="StringBuilder"/> used to generate the resulting string.</param>
        /// <returns>The new state.</returns>
        private static State ProcessOutsideToken(Cursor cursor, StringBuilder result)
        {
            var state = State.OutsideToken;

            switch (cursor.Value)
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
                    result.Append((char)cursor.Value);
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
            if (value.IsInvalidTokenNameCharacter())
            {
                throw new FormatException($"Illegal '{(char)cursor.Value}' at position {cursor.Position}");
            }
        }
    }
}