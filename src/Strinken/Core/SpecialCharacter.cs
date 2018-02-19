namespace Strinken.Core
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
    }
}