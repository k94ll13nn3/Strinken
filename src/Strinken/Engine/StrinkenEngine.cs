// stylecop.header
using System;
using System.Collections.Generic;
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
                stack.PushVerbatim(input);
                return new EngineResult
                {
                    Success = true,
                    Stack = stack,
                    ErrorMessage = null
                };
            }

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
                    .On(State.OnTokenStartIndicator).Do(this.ProcessToken)
                    .On(State.OnTokenEndIndicator).Do(this.ProcessOnTokenEndIndicator)
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
        /// Processes the inside of a token.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessToken()
        {
            if (this.cursor.Value == SpecialCharacter.TokenStartIndicator)
            {
                // Escaped TokenEnd
                this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenStartIndicator);
                return State.OutsideToken;
            }

            if (this.cursor.HasEnded())
            {
                this.errorMessage = string.Format(Errors.IllegalCharacterAtStringEnd, '{');
                return State.InvalidString;
            }

            var parsingResult = ParseString(this.cursor);
            if (parsingResult.Result)
            {
                foreach (var token in parsingResult.Value)
                {
                    this.tokenStack.Push(token);
                }

                return State.OutsideToken;
            }
            else
            {
                this.errorMessage = parsingResult.Message;
                return State.InvalidString;
            }
        }

        /// <summary>
        /// Processes the <see cref="State.OnTokenEndIndicator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnTokenEndIndicator()
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
                    state = this.RaiseError(string.Format(Errors.IllegalCharacter, (char)SpecialCharacter.TokenEndIndicator, this.cursor.Position - 1));
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
            State state;

            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.EndOfString;
                    break;

                case SpecialCharacter.TokenStartIndicator:
                    state = State.OnTokenStartIndicator;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    state = State.OnTokenEndIndicator;
                    break;

                default:
                    this.tokenStack.PushVerbatim((char)this.cursor.Value);
                    state = State.OutsideToken;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Sets the current error message and returns an invalid state.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns><see cref="State.InvalidString"/></returns>
        private State RaiseError(string error)
        {
            this.errorMessage = error;
            return State.InvalidString;
        }

        /// <summary>
        /// Parses a string and returns the first name in it.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="ends">A list of valid ends.</param>
        /// <param name="isValidChar">A function indicating whether a character is valid.</param>
        /// <returns>The result of the parsing.</returns>
        private static ParseResult<string> ParseName(Cursor cursor, ICollection<int> ends, Func<int, bool> isValidChar)
        {
            var builder = new StringBuilder();
            var updatedEnd = new List<int> { SpecialCharacter.TokenEndIndicator };
            foreach (var end in ends)
            {
                updatedEnd.Add(end);
            }

            while (true)
            {
                if (updatedEnd.Contains(cursor.Value))
                {
                    break;
                }

                if (cursor.HasEnded())
                {
                    return ParseResult<string>.FailureWithMessage(Errors.EndOfString);
                }

                if (!(isValidChar?.Invoke(cursor.Value) ?? false))
                {
                    return ParseResult<string>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
                }

                builder.Append(cursor.CharValue);
                cursor.Next();
            }

            var parsedName = builder.ToString();
            return !string.IsNullOrEmpty(parsedName) ? ParseResult<string>.Success(parsedName) : ParseResult<string>.FailureWithMessage(Errors.EmptyName);
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="isArgument">A value indicating whether the tag is used as an argument.</param>
        /// <returns>The result of the parsing.</returns>
        private static ParseResult<Token> ParseTag(Cursor cursor, bool isArgument = false)
        {
            var subtype = TokenSubtype.Base;

            /*
             Special character before the token can be parsed here.
             */

            var ends = new List<int> { SpecialCharacter.FilterSeparator };
            if (isArgument)
            {
                ends.Add(SpecialCharacter.ArgumentSeparator);
            }

            var result = ParseName(cursor, ends, c => !((char)c).IsInvalidTokenNameCharacter());
            if (result.Result)
            {
                return ParseResult<Token>.Success(new Token(result.Value, TokenType.Tag, subtype));
            }

            return ParseResult<Token>.FailureWithMessage(result.Message);
        }

        /// <summary>
        /// Parses a filter.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        private static ParseResult<Token> ParseFilter(Cursor cursor)
        {
            var subtype = TokenSubtype.Base;
            if (cursor.Value == SpecialCharacter.FilterSeparator)
            {
                cursor.Next();

                /*
                 Special character before the token can be parsed here.
                 */

                var ends = new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator };
                var result = ParseName(cursor, ends, c => !((char)c).IsInvalidTokenNameCharacter());
                if (result.Result)
                {
                    return ParseResult<Token>.Success(new Token(result.Value, TokenType.Filter, subtype));
                }
                else
                {
                    return ParseResult<Token>.FailureWithMessage(result.Message);
                }
            }

            return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        private static ParseResult<Token> ParseArgument(Cursor cursor)
        {
            var subtype = TokenSubtype.Base;

            if (cursor.Value == SpecialCharacter.ArgumentIndicator || cursor.Value == SpecialCharacter.ArgumentSeparator)
            {
                cursor.Next();
                if (cursor.Value == SpecialCharacter.ArgumentTagIndicator)
                {
                    // Consume ArgumentTagIndicator
                    cursor.Next();
                    var result = ParseTag(cursor, true);

                    if (result.Result)
                    {
                        if (result.Value.Subtype == TokenSubtype.Base)
                        {
                            subtype = TokenSubtype.Tag;
                        }

                        return ParseResult<Token>.Success(new Token(result.Value.Data, TokenType.Argument, subtype));
                    }
                    else
                    {
                        return ParseResult<Token>.FailureWithMessage(result.Message);
                    }
                }
                else
                {
                    var result = ParseName(cursor, new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, c => true);
                    if (result.Result)
                    {
                        return ParseResult<Token>.Success(new Token(result.Value, TokenType.Argument, subtype));
                    }
                    else
                    {
                        return ParseResult<Token>.FailureWithMessage(result.Message);
                    }
                }
            }

            return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
        }

        private static ParseResult<IList<Token>> ParseFilterAndArgument(Cursor cursor)
        {
            var tokenList = new List<Token>();
            var filterParseResult = ParseFilter(cursor);
            if (!filterParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(filterParseResult.Message != Errors.EmptyName ? filterParseResult.Message : Errors.EmptyFilter);
            }

            tokenList.Add(filterParseResult.Value);
            if (cursor.Value != SpecialCharacter.FilterSeparator && cursor.Value != SpecialCharacter.TokenEndIndicator && !cursor.HasEnded())
            {
                while (cursor.Value != SpecialCharacter.FilterSeparator && cursor.Value != SpecialCharacter.TokenEndIndicator && !cursor.HasEnded())
                {
                    var argumentParseResult = ParseArgument(cursor);
                    if (!argumentParseResult.Result)
                    {
                        return ParseResult<IList<Token>>.FailureWithMessage(argumentParseResult.Message != Errors.EmptyName ? argumentParseResult.Message : Errors.EmptyArgument);
                    }

                    tokenList.Add(argumentParseResult.Value);
                }
            }

            return ParseResult<IList<Token>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        private static ParseResult<IList<Token>> ParseString(Cursor cursor)
        {
            var tokenList = new List<Token>();
            var tagParseResult = ParseTag(cursor);
            if (!tagParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(tagParseResult.Message != Errors.EmptyName ? tagParseResult.Message : Errors.EmptyTag);
            }

            tokenList.Add(tagParseResult.Value);
            while (!cursor.HasEnded() && cursor.Value != SpecialCharacter.TokenEndIndicator)
            {
                var filterAndArgumentParseResult = ParseFilterAndArgument(cursor);

                if (filterAndArgumentParseResult.Result)
                {
                    foreach (var token in filterAndArgumentParseResult.Value)
                    {
                        tokenList.Add(token);
                    }
                }
                else
                {
                    return ParseResult<IList<Token>>.FailureWithMessage(filterAndArgumentParseResult.Message);
                }
            }

            return ParseResult<IList<Token>>.Success(tokenList);
        }
    }
}