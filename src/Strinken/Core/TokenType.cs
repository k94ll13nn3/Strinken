namespace Strinken.Core;

/// <summary>
/// Type of a token.
/// </summary>
internal enum TokenType
{
    /// <summary>
    /// The token is an argument.
    /// </summary>
    Argument = 0,

    /// <summary>
    /// The token is a tag.
    /// </summary>
    Tag = 1,

    /// <summary>
    /// The token is a filter.
    /// </summary>
    Filter = 2,

    /// <summary>
    /// The token is a string that will be rendered without modifications.
    /// </summary>
    None = 3
}
