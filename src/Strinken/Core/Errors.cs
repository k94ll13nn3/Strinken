namespace Strinken.Core;

/// <summary>
/// Class that contains error messages.
/// </summary>
internal static class Errors
{
    /// <summary>
    /// Error: End of string reached while inside a token
    /// </summary>
    public const string EndOfString = "End of string reached while inside a token";

    /// <summary>
    /// Error: Empty argument
    /// </summary>
    public const string EmptyArgument = "Empty argument";

    /// <summary>
    /// Error: Empty filter
    /// </summary>
    public const string EmptyFilter = "Empty filter";

    /// <summary>
    /// Error: Empty tag
    /// </summary>
    public const string EmptyTag = "Empty tag";

    /// <summary>
    /// Error: Empty name
    /// </summary>
    public const string EmptyName = "Empty name";

    /// <summary>
    /// Error: Illegal '{0}' at position {1}
    /// </summary>
    public const string IllegalCharacter = "Illegal '{0}' at position {1}";

    /// <summary>
    /// Error: Illegal '{0}' at the end of the string
    /// </summary>
    public const string IllegalCharacterAtStringEnd = "Illegal '{0}' at the end of the string";
}
