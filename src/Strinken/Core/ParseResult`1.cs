namespace Strinken.Core;

/// <summary>
/// Results of the parsing.
/// </summary>
internal sealed record ParseResult<T>(bool Result, T Value, string Message)
    where T : notnull
{
    /// <summary>
    /// Creates a new successful result.
    /// </summary>
    /// <param name="value">The parsed value.</param>
    /// <returns>The result.</returns>
    public static ParseResult<T> Success(T value)
    {
        return new ParseResult<T>(true, value, string.Empty);
    }

    /// <summary>
    /// Creates a new failure result.
    /// </summary>
    /// <param name="message">The message associated to the parsing.</param>
    /// <returns>The result.</returns>
    public static ParseResult<T> FailureWithMessage(string message)
    {
        return new ParseResult<T>(false, default!, message);
    }

    /// <summary>
    /// Defines an implicit conversion of a <see cref="ParseResult{T}"/> to a <see cref="bool"/>.
    /// </summary>
    /// <param name="parseResult">The value to convert.</param>
    public static implicit operator bool(ParseResult<T> parseResult) => parseResult.Result;
}
