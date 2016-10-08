// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// Class that contains error messages.
    /// </summary>
    internal static class Errors
    {
        /// <summary>
        /// Error: End of string reached while inside a token
        /// </summary>
        public static readonly string EndOfString = "End of string reached while inside a token";

        /// <summary>
        /// Error: Empty argument
        /// </summary>
        public static readonly string EmptyArgument = "Empty argument";

        /// <summary>
        /// Error: Empty filter
        /// </summary>
        public static readonly string EmptyFilter = "Empty filter";

        /// <summary>
        /// Error: Empty tag
        /// </summary>
        public static readonly string EmptyTag = "Empty tag";

        /// <summary>
        /// Error: Empty name
        /// </summary>
        public static readonly string EmptyName = "Empty name";

        /// <summary>
        /// Error: Illegal '{0}' at position {1}
        /// </summary>
        public static readonly string IllegalCharacter = "Illegal '{0}' at position {1}";

        /// <summary>
        /// Error: Illegal '{0}' at the end of the string
        /// </summary>
        public static readonly string IllegalCharacterAtStringEnd = "Illegal '{0}' at the end of the string";
    }
}