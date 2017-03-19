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
            reader = new StringReader(input);
            Position = -1;
            Value = '\0';

            Next();
        }

        /// <summary>
        /// Gets the current value of the cursor as a <see cref="char"/>.
        /// </summary>
        public char CharValue => (char)Value;

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
            reader?.Dispose();
        }

        /// <summary>
        /// Indicates if the cursor has reached the end.
        /// </summary>
        /// <returns>A value indicating whether the cursor as reached the end.</returns>
        public bool HasEnded() => Position != -1 && Value == -1;

        /// <summary>
        /// Moves the cursor.
        /// </summary>
        public void Next()
        {
            Value = reader.Read();
            Position++;
        }

        /// <summary>
        /// Peeks the next character of the cursor.
        /// </summary>
        /// <returns>The next character of the cursor.</returns>
        public int Peek() => reader.Peek();

        /// <summary>
        /// Indicates if the next character is the end.
        /// </summary>
        /// <returns>A value indicating whether the next character is the end.</returns>
        public bool PeekIsEnd() => Position != -1 && Peek() == -1;

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<Token> ParseArgument()
        {
            if (Value != SpecialCharacter.ArgumentIndicator && Value != SpecialCharacter.ArgumentSeparator)
            {
                return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
            }

            var subtype = TokenSubtype.Base;
            Next();
            if (Value == SpecialCharacter.ArgumentTagIndicator)
            {
                // Consume ArgumentTagIndicator
                Next();
                var result = ParseTag(true);
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
                var result = ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, c => true);
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
            if (Value != SpecialCharacter.FilterSeparator)
            {
                return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
            }

            var subtype = TokenSubtype.Base;
            Next();

            /*
             * Special characters before the token can be parsed here.
             */

            var ends = new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator };
            var result = ParseName(ends, c => !c.IsInvalidTokenNameCharacter());
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
            var filterParseResult = ParseFilter();
            if (!filterParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(filterParseResult.Message != Errors.EmptyName ? filterParseResult.Message : Errors.EmptyFilter);
            }

            tokenList.Add(filterParseResult.Value);
            while (Value != SpecialCharacter.FilterSeparator && Value != SpecialCharacter.TokenEndIndicator && !HasEnded())
            {
                var argumentParseResult = ParseArgument();
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
                switch (Value)
                {
                    case int _ when updatedEnd.Contains(Value):
                        var parsedName = builder.ToString();
                        return !string.IsNullOrEmpty(parsedName) ? ParseResult<string>.Success(parsedName) : ParseResult<string>.FailureWithMessage(Errors.EmptyName);

                    case int _ when HasEnded():
                        return ParseResult<string>.FailureWithMessage(Errors.EndOfString);

                    case int _ when !isValidChar?.Invoke(CharValue) ?? false:
                        return ParseResult<string>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));

                    default:
                        break;
                }

                builder.Append(CharValue);
                Next();
            }
        }

        /// <summary>
        /// Parses a token.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IList<Token>> ParseToken()
        {
            if (Value != SpecialCharacter.TokenStartIndicator)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
            }

            Next();

            var tokenList = new List<Token>();
            var tagParseResult = ParseTag();
            if (!tagParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(tagParseResult.Message != Errors.EmptyName ? tagParseResult.Message : Errors.EmptyTag);
            }

            tokenList.Add(tagParseResult.Value);
            while (!HasEnded() && Value != SpecialCharacter.TokenEndIndicator)
            {
                var filterAndArgumentsParseResult = ParseFilterAndArguments();
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
            if (Value == SpecialCharacter.ParameterTagIndicator)
            {
                subtype = TokenSubtype.ParameterTag;
                Next();
            }

            var ends = new List<int> { SpecialCharacter.FilterSeparator };
            if (isArgument)
            {
                ends.Add(SpecialCharacter.ArgumentSeparator);
            }

            var result = ParseName(ends, c => !c.IsInvalidTokenNameCharacter());
            if (result.Result)
            {
                return ParseResult<Token>.Success(new Token(result.Value, TokenType.Tag, subtype));
            }

            return ParseResult<Token>.FailureWithMessage(result.Message);
        }

        /// <summary>
        /// Parses an outside string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<Token> ParseOutsideString()
        {
            var builder = new StringBuilder();
            while (true)
            {
                switch (Value)
                {
                    // Escaped indicator
                    case SpecialCharacter.TokenStartIndicator when Peek() == SpecialCharacter.TokenStartIndicator:
                    case SpecialCharacter.TokenEndIndicator when Peek() == SpecialCharacter.TokenEndIndicator:
                        Next();
                        break;

                    // Start of token or end of string
                    case SpecialCharacter.TokenStartIndicator:
                    case int _ when HasEnded():
                        return ParseResult<Token>.Success(new Token(builder.ToString(), TokenType.None, TokenSubtype.Base));

                    case SpecialCharacter.TokenEndIndicator when PeekIsEnd():
                        return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacterAtStringEnd, CharValue));

                    case SpecialCharacter.TokenEndIndicator:
                        return ParseResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                builder.Append(CharValue);
                Next();
            }
        }

        /// <summary>
        /// Parses a token and the following outside string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IList<Token>> ParseTokenAndOutsideString()
        {
            var tokenList = new List<Token>();
            var tokenParseResult = ParseToken();
            if (!tokenParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(tokenParseResult.Message != Errors.EmptyName ? tokenParseResult.Message : Errors.EmptyFilter);
            }

            tokenList.AddRange(tokenParseResult.Value);

            if (Value != SpecialCharacter.TokenEndIndicator)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
            }

            Next();

            var outsideParseResult = ParseOutsideString();
            if (!outsideParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(outsideParseResult.Message);
            }

            tokenList.Add(outsideParseResult.Value);

            return ParseResult<IList<Token>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IList<Token>> ParseString()
        {
            var tokenList = new List<Token>();
            var outsideParseResult = ParseOutsideString();
            if (!outsideParseResult.Result)
            {
                return ParseResult<IList<Token>>.FailureWithMessage(outsideParseResult.Message);
            }

            tokenList.Add(outsideParseResult.Value);
            while (!HasEnded())
            {
                var tokenParseResult = ParseTokenAndOutsideString();
                if (tokenParseResult.Result)
                {
                    foreach (var token in tokenParseResult.Value)
                    {
                        tokenList.Add(token);
                    }
                }
                else
                {
                    return ParseResult<IList<Token>>.FailureWithMessage(tokenParseResult.Message);
                }
            }

            return ParseResult<IList<Token>>.Success(tokenList);
        }
    }
}