// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// Class that contains the different possible separators and indicators.
    /// </summary>
    internal static class SpecialCharacter
    {
        /// <summary>
        /// Separator that indicates the start of an argument list.
        /// </summary>
        public const int ArgumentIndicator = '+';

        /// <summary>
        /// Separator that separates arguments.
        /// </summary>
        public const int ArgumentSeparator = ',';

        /// <summary>
        /// Separator that indicates the start of an argument tag.
        /// </summary>
        public const int ArgumentTagIndicator = '=';

        /// <summary>
        /// Separator that indicates the end of the string.
        /// </summary>
        public const int EndOfStringIndicator = -1;

        /// <summary>
        /// Separator that separates filters.
        /// </summary>
        public const int FilterSeparator = ':';

        /// <summary>
        /// Separator that indicates the end of a token.
        /// </summary>
        public const int TokenEndIndicator = '}';

        /// <summary>
        /// Separator that indicates the start of a token.
        /// </summary>
        public const int TokenStartIndicator = '{';

        /// <summary>
        /// Separator that indicates the start of a parameter tag.
        /// </summary>
        public const int ParameterTagIndicator = '!';
    }
}