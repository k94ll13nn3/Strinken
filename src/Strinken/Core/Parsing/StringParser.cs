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
        public static ParserResult<string, int> ParseName(Cursor cursor, ICollection<int> ends, Func<int, bool> isValidChar)
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

                if (cursor.HasEnded() || !(isValidChar?.Invoke(cursor.Value) ?? false))
                {
                    return ParserResult<string, int>.Failure;
                }

                builder.Append((char)cursor.Value);
                cursor.Next();
            }

            var parsedName = builder.ToString();
            return !string.IsNullOrEmpty(parsedName) ? ParserResult<string, int>.Success(parsedName, cursor.Value) : ParserResult<string, int>.Failure;
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="isArgument">A value indicating whether the tag is used as an argument.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token, int> ParseTag(Cursor cursor, bool isArgument = false)
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
                return ParserResult<Token, int>.Success(new Token(result.Value, TokenType.Tag, subtype), result.OptionalData);
            }

            return ParserResult<Token, int>.Failure;
        }

        /// <summary>
        /// Parses a filter.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token, int> ParseFilter(Cursor cursor)
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
                    return ParserResult<Token, int>.Success(new Token(result.Value, TokenType.Filter, subtype), result.OptionalData);
                }
            }

            return ParserResult<Token, int>.Failure;
        }

        /// <summary>
        /// Parses an argument.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token, int> ParseArgument(Cursor cursor)
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

                        return ParserResult<Token, int>.Success(new Token(result.Value.Data, TokenType.Argument, subtype), result.OptionalData);
                    }
                }
                else
                {
                    var result = ParseName(cursor, new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, c => c != SpecialCharacter.ArgumentIndicator);
                    if (result.Result)
                    {
                        return ParserResult<Token, int>.Success(new Token(result.Value, TokenType.Argument, subtype), result.OptionalData);
                    }
                }
            }

            return ParserResult<Token, int>.Failure;
        }

        public static ParserResult<IList<Token>, string> ParseString(Cursor cursor)
        {
            var tokenStack = new List<Token>();
            var tag = ParseTag(cursor);
            if (!tag.Result)
            {
                return ParserResult<IList<Token>, string>.Failure;
            }

            tokenStack.Add(tag.Value);
            var lastEnd = tag.OptionalData;
            while (cursor.Value != -1 && lastEnd != SpecialCharacter.TokenEndIndicator)
            {
                var filter = ParseFilter(cursor);
                if (!filter.Result)
                {
                    return ParserResult<IList<Token>, string>.Failure;
                }

                lastEnd = filter.OptionalData;
                tokenStack.Add(filter.Value);
                if (filter.OptionalData != SpecialCharacter.FilterSeparator && filter.OptionalData != SpecialCharacter.TokenEndIndicator && cursor.Value != -1)
                {
                    var arg = ParserResult<Token, int>.Failure;
                    while (arg.OptionalData != SpecialCharacter.FilterSeparator && arg.OptionalData != SpecialCharacter.TokenEndIndicator && cursor.Value != -1)
                    {
                        arg = ParseArgument(cursor);
                        if (!arg.Result)
                        {
                            return ParserResult<IList<Token>, string>.Failure;
                        }
                        tokenStack.Add(arg.Value);
                        lastEnd = arg.OptionalData;
                    }
                }
            }

            return ParserResult<IList<Token>, string>.Success(tokenStack, "");
        }
    }
}