namespace Strinken.Core
{
    /// <summary>
    /// Method of parsing.
    /// </summary>
    internal enum ParsingMethod
    {
        /// <summary>
        /// Parse string as a name (letter + '-' + '_').
        /// </summary>
        Name,

        /// <summary>
        /// Parse string as a name (letter + '-' + '_') or as a symbol defined in <see cref="Extensions.ValidAlternativeNameCharacter"/>.
        /// </summary>
        NameOrSymbol,

        /// <summary>
        /// Parse all the string.
        /// </summary>
        Full,

        /// <summary>
        /// Parse a binary number (0 or 1).
        /// </summary>
        Binary,

        /// <summary>
        /// Parse an octal number (0 to 7).
        /// </summary>
        Octal,

        /// <summary>
        /// Parse a decimal number (0 to 9).
        /// </summary>
        Decimal,

        /// <summary>
        /// Parse an hexadecimal number [0-9a-fA-F].
        /// </summary>
        Hexadecimal,
    }
}
