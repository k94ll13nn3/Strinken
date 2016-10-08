// stylecop.header
using System;
using System.Collections.Generic;
using System.Text;
using Strinken.Common;
using Strinken.Core.Types;
using Strinken.Engine;

namespace Strinken.Core.Parsing
{
    /// <summary>
    /// Class for parsing names.
    /// </summary>
    internal static class StringParser
    {
        /// <summary>
        /// Parses a string and returns the first name in it.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="ends">A list of valid ends.</param>
        /// <param name="isValidChar">A function indicating whether a character is valid.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<string> ParseName(Cursor cursor, ICollection<int> ends, Func<int, bool> isValidChar)
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
                    return ParserResult<string>.FailureWithMessage(Errors.EndOfString);
                }

                if (!(isValidChar?.Invoke(cursor.Value) ?? false))
                {
                    return ParserResult<string>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
                }

                builder.Append(cursor.CharValue);
                cursor.Next();
            }

            var parsedName = builder.ToString();
            return !string.IsNullOrEmpty(parsedName) ? ParserResult<string>.Success(parsedName) : ParserResult<string>.FailureWithMessage(Errors.EmptyName);
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="isArgument">A value indicating whether the tag is used as an argument.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token> ParseTag(Cursor cursor, bool isArgument = false)
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
                return ParserResult<Token>.Success(new Token(result.Value, TokenType.Tag, subtype));
            }

            return ParserResult<Token>.FailureWithMessage(result.Message);
        }

        /// <summary>
        /// Parses a filter.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token> ParseFilter(Cursor cursor)
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
                    return ParserResult<Token>.Success(new Token(result.Value, TokenType.Filter, subtype));
                }
                else
                {
                    return ParserResult<Token>.FailureWithMessage(result.Message);
                }
            }

            return ParserResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token> ParseArgument(Cursor cursor)
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

                        return ParserResult<Token>.Success(new Token(result.Value.Data, TokenType.Argument, subtype));
                    }
                    else
                    {
                        return ParserResult<Token>.FailureWithMessage(result.Message);
                    }
                }
                else
                {
                    var result = ParseName(cursor, new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, c => true);
                    if (result.Result)
                    {
                        return ParserResult<Token>.Success(new Token(result.Value, TokenType.Argument, subtype));
                    }
                    else
                    {
                        return ParserResult<Token>.FailureWithMessage(result.Message);
                    }
                }
            }

            return ParserResult<Token>.FailureWithMessage(string.Format(Errors.IllegalCharacter, cursor.CharValue, cursor.Position));
        }

        /// <summary>
        /// Parses a string.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<IList<Token>> ParseString(Cursor cursor)
        {
            var tokenStack = new List<Token>();
            var tag = ParseTag(cursor);
            if (!tag.Result)
            {
                return ParserResult<IList<Token>>.FailureWithMessage(tag.Message != Errors.EmptyName ? tag.Message : Errors.EmptyTag);
            }

            tokenStack.Add(tag.Value);
            var lastEnd = cursor.Value;
            while (!cursor.HasEnded() && lastEnd != SpecialCharacter.TokenEndIndicator)
            {
                var filter = ParseFilter(cursor);
                if (!filter.Result)
                {
                    return ParserResult<IList<Token>>.FailureWithMessage(filter.Message != Errors.EmptyName ? filter.Message : Errors.EmptyFilter);
                }

                lastEnd = cursor.Value;
                tokenStack.Add(filter.Value);
                if (cursor.Value != SpecialCharacter.FilterSeparator && cursor.Value != SpecialCharacter.TokenEndIndicator && !cursor.HasEnded())
                {
                    while (cursor.Value != SpecialCharacter.FilterSeparator && cursor.Value != SpecialCharacter.TokenEndIndicator && !cursor.HasEnded())
                    {
                        var arg = ParseArgument(cursor);
                        if (!arg.Result)
                        {
                            return ParserResult<IList<Token>>.FailureWithMessage(arg.Message != Errors.EmptyName ? arg.Message : Errors.EmptyArgument);
                        }

                        tokenStack.Add(arg.Value);
                        lastEnd = cursor.Value;
                    }
                }
            }

            return ParserResult<IList<Token>>.Success(tokenStack);
        }
    }
}