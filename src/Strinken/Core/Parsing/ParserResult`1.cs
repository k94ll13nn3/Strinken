// stylecop.header

namespace Strinken.Core.Parsing
{
    /// <summary>
    /// Base class for all parsing result.
    /// </summary>
    /// <typeparam name="T">The type of the parsed data.</typeparam>
    internal class ParserResult<T>
    {
        /// <summary>
        /// A failure result.
        /// </summary>
        public static readonly ParserResult<T> Failure = new ParserResult<T>(false, default(T));

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserResult{T}"/> class.
        /// </summary>
        /// <param name="result">A value indicating whether the parsing was successful.</param>
        /// <param name="value">The parsed value.</param>
        private ParserResult(bool result, T value)
        {
            this.Result = result;
            this.Value = value;
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
        /// Creates a new successful result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <returns>The result.</returns>
        public static ParserResult<T> Success(T value) => new ParserResult<T>(true, value);
    }
}