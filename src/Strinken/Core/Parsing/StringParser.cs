// stylecop.header
using System.Linq;
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
        /// Parses a name.
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
        public static ParserResult<string, int> ParseNameWithEnd(Cursor cursor, int[] ends)
        {
            var result = ParseName(cursor);
            if (result.Result && ends.Contains(cursor.Value))
            {
                var foundEnd = cursor.Value;

                // Consume the end.
                cursor.Next();
                return ParserResult<string, int>.Success(result.Value, foundEnd);
            }

            return ParserResult<string, int>.Failure;
        }

        /// <summary>
        /// Parses a tag.
        /// </summary>
        /// <param name="cursor">The cursor to parse.</param>
        /// <returns>The result of the parsing.</returns>
        public static ParserResult<Token> ParseTag(Cursor cursor)
        {
            var subtype = TokenSubtype.Base;

            /*
             Special character before the token can be parsed here.
             */

            var result = ParseNameWithEnd(cursor, new[] { SpecialCharacter.FilterSeparator });
            if (result.Result)
            {
                return ParserResult<Token>.Success(new Token(result.Value, TokenType.Tag, subtype));
            }

            return ParserResult<Token>.Failure;
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
    }
}