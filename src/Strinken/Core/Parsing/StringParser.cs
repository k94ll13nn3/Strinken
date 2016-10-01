// stylecop.header
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
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<string> ParseName(Cursor cursor)
        {
            if (cursor.Value == -1)
            {
                return ParserResult<string>.Failure;
            }

            var builder = new StringBuilder();
            while (cursor.Value != -1)
            {
                if (((char)cursor.Value).IsInvalidTokenNameCharacter())
                {
                    break;
                }

                builder.Append((char)cursor.Value);
                cursor.Next();
            }

            return ParserResult<string>.Success(builder.ToString());
        }

        /// <summary>
        /// Parses a name with a specified end.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <param name="ends">A list of valid ends.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<string, int> ParseNameWithEnd(Cursor cursor, ICollection<int> ends)
        {
            var result = ParseName(cursor);
            if (result.Result && ends.Contains(cursor.Value))
            {
                var foundEnd = cursor.Value;

                // Consume end.
                cursor.Next();
                return ParserResult<string, int>.Success(result.Value, foundEnd);
            }

            return ParserResult<string, int>.Failure;
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

            var result = ParseNameWithEnd(cursor, ends);
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

            /*
             Special character before the token can be parsed here.
             */
            var result = ParseNameWithEnd(cursor, new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator });
            if (result.Result)
            {
                return ParserResult<Token, int>.Success(new Token(result.Value, TokenType.Filter, subtype), result.OptionalData);
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
                var result = ParseNameWithEnd(cursor, new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator });
                if (result.Result)
                {
                    return ParserResult<Token, int>.Success(new Token(result.Value, TokenType.Argument, subtype), result.OptionalData);
                }
            }

            return ParserResult<Token, int>.Failure;
        }
    }
}