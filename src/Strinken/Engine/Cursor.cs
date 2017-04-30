// stylecop.header
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private static readonly IEnumerable<Operator> Operators = new List<Operator>
        {
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Tag,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Filter,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Full }
                }
            },
            new Operator
            {
                Symbol = '=',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name }
                }
            }
        };

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
        /// Parses a string inside a token and returns the first name in it.
        /// </summary>
        /// <param name="ends">A list of valid ends.</param>
        /// <param name="tokenType">A function indicating whether a character is valid.</param>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<TokenDefinition> ParseName(ICollection<int> ends, TokenType tokenType)
        {
            var builder = new StringBuilder();
            var updatedEnd = new List<int> { SpecialCharacter.TokenEndIndicator };
            foreach (var end in ends ?? Enumerable.Empty<int>())
            {
                updatedEnd.Add(end);
            }

            var operatorDefined = Operators.FirstOrDefault(x => x.Symbol == CharValue && x.TokenType == tokenType);
            if (operatorDefined != null)
            {
                Next();
            }
            else
            {
                operatorDefined = Operators.Single(x => x.Symbol == '\0' && x.TokenType == tokenType);
            }

            var indicatorDefined = operatorDefined.Indicators.FirstOrDefault(x => x.Symbol == CharValue);
            if (indicatorDefined != null)
            {
                Next();
            }
            else
            {
                indicatorDefined = operatorDefined.Indicators.Single(x => x.Symbol == '\0');
            }

            while (true)
            {
                switch (Value)
                {
                    case int _ when updatedEnd.Contains(Value):
                        var parsedName = builder.ToString();
                        return !string.IsNullOrEmpty(parsedName) ?
                            ParseResult<TokenDefinition>.Success(new TokenDefinition(parsedName, tokenType, operatorDefined.Symbol, indicatorDefined.Symbol)) :
                            ParseResult<TokenDefinition>.FailureWithMessage(Errors.EmptyName);

                    case int _ when HasEnded():
                        return ParseResult<TokenDefinition>.FailureWithMessage(Errors.EndOfString);

                    case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Name && CharValue.IsInvalidTokenNameCharacter():
                        return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                builder.Append(CharValue);
                Next();
            }
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<TokenDefinition> ParseArgument()
        {
            var result = ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, TokenType.Argument);
            if (result)
            {
                return result;
            }

            return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyArgument);
        }

        /// <summary>
        /// Parses a filter.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<TokenDefinition> ParseFilter()
        {
            var result = ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator }, TokenType.Filter);
            if (result)
            {
                return result;
            }

            return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyFilter);
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<TokenDefinition> ParseTag()
        {
            var result = ParseName(new[] { SpecialCharacter.FilterSeparator }, TokenType.Tag);
            if (result)
            {
                return result;
            }

            return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyTag);
        }

        /// <summary>
        /// Parses a filter and its possible arguments.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IEnumerable<TokenDefinition>> ParseFilterAndArguments()
        {
            var tokenList = new List<TokenDefinition>();
            var filterParseResult = ParseFilter();
            if (!filterParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(filterParseResult.Message);
            }

            tokenList.Add(filterParseResult.Value);
            while (Value != SpecialCharacter.FilterSeparator && Value != SpecialCharacter.TokenEndIndicator && !HasEnded())
            {
                if (Value != SpecialCharacter.ArgumentIndicator && Value != SpecialCharacter.ArgumentSeparator)
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                Next();
                var argumentParseResult = ParseArgument();
                if (!argumentParseResult)
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(argumentParseResult.Message);
                }

                tokenList.Add(argumentParseResult.Value);
            }

            return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a token.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IEnumerable<TokenDefinition>> ParseToken()
        {
            var tokenList = new List<TokenDefinition>();
            var tagParseResult = ParseTag();
            if (!tagParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tagParseResult.Message);
            }

            tokenList.Add(tagParseResult.Value);
            while (!HasEnded() && Value != SpecialCharacter.TokenEndIndicator)
            {
                if (Value != SpecialCharacter.FilterSeparator)
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                Next();
                var filterAndArgumentsParseResult = ParseFilterAndArguments();
                if (filterAndArgumentsParseResult)
                {
                    tokenList.AddRange(filterAndArgumentsParseResult.Value ?? Enumerable.Empty<TokenDefinition>());
                }
                else
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(filterAndArgumentsParseResult.Message);
                }
            }

            return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
        }

        /// <summary>
        /// Parses an outside string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<TokenDefinition> ParseOutsideString()
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
                        return ParseResult<TokenDefinition>.Success(new TokenDefinition(builder.ToString(), TokenType.None, '\0', '\0'));

                    case SpecialCharacter.TokenEndIndicator when PeekIsEnd():
                        return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(Errors.IllegalCharacterAtStringEnd, CharValue));

                    case SpecialCharacter.TokenEndIndicator:
                        return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                builder.Append(CharValue);
                Next();
            }
        }

        /// <summary>
        /// Parses a token and the following outside string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IEnumerable<TokenDefinition>> ParseTokenAndOutsideString()
        {
            var tokenList = new List<TokenDefinition>();
            var tokenParseResult = ParseToken();
            if (!tokenParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tokenParseResult.Message);
            }

            tokenList.AddRange(tokenParseResult.Value ?? Enumerable.Empty<TokenDefinition>());
            if (Value != SpecialCharacter.TokenEndIndicator)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
            }

            Next();
            var outsideParseResult = ParseOutsideString();
            if (!outsideParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(outsideParseResult.Message);
            }

            tokenList.Add(outsideParseResult.Value);
            return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <returns>The result of the parsing.</returns>
        public ParseResult<IEnumerable<TokenDefinition>> ParseString()
        {
            var tokenList = new List<TokenDefinition>();
            var outsideParseResult = ParseOutsideString();
            if (!outsideParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(outsideParseResult.Message);
            }

            tokenList.Add(outsideParseResult.Value);
            while (!HasEnded())
            {
                if (Value != SpecialCharacter.TokenStartIndicator)
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(Errors.IllegalCharacter, CharValue, Position));
                }

                Next();
                var tokenParseResult = ParseTokenAndOutsideString();
                if (tokenParseResult)
                {
                    foreach (var token in tokenParseResult.Value)
                    {
                        tokenList.Add(token);
                    }
                }
                else
                {
                    return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tokenParseResult.Message);
                }
            }

            return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
        }
    }
}