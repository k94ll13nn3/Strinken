// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// Type of a token.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// The token is an argument.
        /// </summary>
        Argument,

        /// <summary>
        /// The token is an argument tag.
        /// </summary>
        ArgumentTag,

        /// <summary>
        /// The token is a filter.
        /// </summary>
        Filter,

        /// <summary>
        /// The token is a tag.
        /// </summary>
        Tag
    }
}