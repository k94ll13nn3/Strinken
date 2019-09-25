namespace Strinken.Core
{
    /// <summary>
    /// Base class for all parsing result.
    /// </summary>
    /// <typeparam name="T">The type of the parsed data.</typeparam>
    internal sealed class ParseResult<T> where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseResult{T}"/> class.
        /// </summary>
        /// <param name="result">A value indicating whether the parsing was successful.</param>
        /// <param name="value">The parsed value.</param>
        /// <param name="message">The message associated to the parsing.</param>
        private ParseResult(bool result, T value, string message)
        {
            Result = result;
            Value = value;
            Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether the parsing was successful.
        /// </summary>
        public bool Result { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the message associated to the parsing.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>The result.</returns>
        public static ParseResult<T> Success(T value) => new ParseResult<T>(true, value, string.Empty);

        /// <summary>
        /// Creates a new failure result.
        /// </summary>
        /// <param name="message">The message associated to the parsing.</param>
        /// <returns>The result.</returns>
        public static ParseResult<T> FailureWithMessage(string message) => new ParseResult<T>(false, default!, message);

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ParseResult{T}"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="parseResult">The value to convert.</param>
        public static implicit operator bool(ParseResult<T> parseResult) => parseResult.Result;
    }
}
