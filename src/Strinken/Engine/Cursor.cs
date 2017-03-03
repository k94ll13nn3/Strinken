// stylecop.header
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Strinken.Common;

namespace Strinken.Engine
{
    /// <summary>
    /// Cursor used to read a string.
    /// </summary>
    internal sealed class Cursor : IDisposable
    {
        /// <summary>
        /// The reader used to read the string.
        /// </summary>
        private readonly StringReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="input">The string to read.</param>
        public Cursor(string input)
        {
            this.reader = new StringReader(input);
            this.Position = -1;
            this.Value = '\0';
        }

        /// <summary>
        /// Gets the current value of the cursor as a <see cref="char"/>.
        /// </summary>
        public char CharValue => (char)this.Value;

        /// <summary>
        /// Gets the current position of the cursor.
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// Gets the current value of the cursor.
        /// </summary>
        public int Value { get; private set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.reader?.Dispose();
        }

        /// <summary>
        /// Indicates if the cursor has reached the end.
        /// </summary>
        /// <returns>A value indicating whether the cursor as reached the end.</returns>
        public bool HasEnded() => this.Position != -1 && this.Value == -1;

        /// <summary>
        /// Moves the cursor.
        /// </summary>
        public void Next()
        {
            this.Value = this.reader.Read();
            this.Position++;
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<Token> ParseArgument()
        {
            if (this.Value != SpecialCharacter.ArgumentIndicator && this.Value != SpecialCharacter.ArgumentSeparator)
            {
                return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, this.CharValue, this.Position));
            }

            var subtype = TokenSubtype.Base;
            this.Next();
            if (this.Value == SpecialCharacter.ArgumentTagIndicator)
            {
                // Consume ArgumentTagIndicator
                this.Next();
                var result = this.ParseTag(true);
                if (result.Result)
                {
                    if (result.Value.Subtype == TokenSubtype.Base)
                    {
                        subtype = TokenSubtype.Tag;
                    }
                    else if (result.Value.Subtype == TokenSubtype.ParameterTag)
                    {
                        subtype = TokenSubtype.ParameterTag;
                    }

                    return ParseResult<Token>.Success(new Token(result.Value.Data, TokenType.Argument, subtype));
                }

                return ParseResult<Token>.FailureWithMessage(result.Message);
            }
            else
            {
                var result = this.ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, c => true);
                if (result.Result)
                {
                    return ParseResult<Token>.Success(new Token(result.Value, TokenType.Argument, subtype));
                }

                return ParseResult<Token>.FailureWithMessage(result.Message);
            }
        }

        /// <summary>
        /// Parses a filter.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<Token> ParseFilter()
        {
            if (this.Value != SpecialCharacter.FilterSeparator)
            {
                return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, this.CharValue, this.Position));
            }

            var subtype = TokenSubtype.Base;
            this.Next();

            // Special characters before the token can be parsed here.

            var ends = new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator };
            var result = this.ParseName(ends, c => !c.IsInvalidTokenNameCharacter());
            if (result.Result)
            {
                return ParseResult<Token>.Success(new Token(result.Value, TokenType.Filter, subtype));
            }

            return ParseResult<Token>.FailureWithMessage(result.Message);
        }

        /// <summary>
        /// Parses a filter and its possible arguments.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IList<Token>> ParseFilterAndArguments()
        {
            var tokenList = new List<Token>();
            var filterParseResult = this.ParseFilter();
            if (!filterParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(filterParseResult.Message != Errors.EmptyName ? filterParseResult.Message : Errors.EmptyFilter);
            }

            tokenList.Add(filterParseResult.Value);
            while (this.Value != SpecialCharacter.FilterSeparator && this.Value != SpecialCharacter.TokenEndIndicator && !this.HasEnded())
            {
                var argumentParseResult = this.ParseArgument();
                if (!argumentParseResult.Result)
                {
                    return ParseResult<IList<Token>>.FailureWithMessage(argumentParseResult.Message != Errors.EmptyName ? argumentParseResult.Message : Errors.EmptyArgument);
                }

                tokenList.Add(argumentParseResult.Value);
            }

            return ParseResult<IList<Token>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a string and returns the first name in it.
        /// </summary>
        /// <param name="ends">A list of valid ends.</param>
        /// <param name="isValidChar">A function indicating whether a character is valid.</param>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<string> ParseName(ICollection<int> ends, Func<char, bool> isValidChar)
        {
            var builder = new StringBuilder();
            var updatedEnd = new List<int> { SpecialCharacter.TokenEndIndicator };
            foreach (var end in ends)
            {
                updatedEnd.Add(end);
            }

            while (true)
            {
                if (updatedEnd.Contains(this.Value))
                {
                    break;
                }

                if (this.HasEnded())
                {
                    return ParseResult<string>.FailureWithMessage(Errors.EndOfString);
                }

                if (!isValidChar?.Invoke(this.CharValue) ?? false)
                {
                    return ParseResult<string>.FailureWithMessage(string.Format(Errors.IllegalCharacter, this.CharValue, this.Position));
                }

                builder.Append(this.CharValue);
                this.Next();
            }

            var parsedName = builder.ToString();
            return !string.IsNullOrEmpty(parsedName) ? ParseResult<string>.Success(parsedName) : ParseResult<string>.FailureWithMessage(Errors.EmptyName);
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IList<Token>> ParseString()
        {
            var tokenList = new List<Token>();
            var tagParseResult = this.ParseTag();
            if (!tagParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(tagParseResult.Message != Errors.EmptyName ? tagParseResult.Message : Errors.EmptyTag);
            }

            tokenList.Add(tagParseResult.Value);
            while (!this.HasEnded() && this.Value != SpecialCharacter.TokenEndIndicator)
            {
                var filterAndArgumentsParseResult = this.ParseFilterAndArguments();
                if (filterAndArgumentsParseResult.Result)
                {
                    foreach (var token in filterAndArgumentsParseResult.Value)
                    {
                        tokenList.Add(token);
                    }
                }
                else
                {
                    return ParseResult<IList<Token>>.FailureWithMessage(filterAndArgumentsParseResult.Message);
                }
            }

            return ParseResult<IList<Token>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <param name="isArgument">A value indicating whether the tag is used as an argument.</param>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<Token> ParseTag(bool isArgument = false)
        {
            var subtype = TokenSubtype.Base;
            if (this.Value == SpecialCharacter.ParameterTagIndicator)
            {
                subtype = TokenSubtype.ParameterTag;
                this.Next();
            }

            var ends = new List<int> { SpecialCharacter.FilterSeparator };
            if (isArgument)
            {
                ends.Add(SpecialCharacter.ArgumentSeparator);
            }

            var result = this.ParseName(ends, c => !c.IsInvalidTokenNameCharacter());
            if (result.Result)
            {
                return ParseResult<Token>.Success(new Token(result.Value, TokenType.Tag, subtype));
            }

            return ParseResult<Token>.FailureWithMessage(result.Message);
        }
    }
}