namespace Strinken.Core
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
        /// The token is a tag.
        /// </summary>
        Tag,

        /// <summary>
        /// The token is a filter.
        /// </summary>
        Filter,

        /// <summary>
        /// The token is a string that will be rendered without modifications.
        /// </summary>
        None
    }
}
