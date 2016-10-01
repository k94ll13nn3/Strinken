// stylecop.header

namespace Strinken.Core.Parsing
{
    /// <summary>
    /// Base class for all parsing result.
    /// </summary>
    /// <typeparam name="TValue">The type of the parsed data.</typeparam>
    /// <typeparam name="TOpt">The type of the optional data.</typeparam>
    internal class ParserResult<TValue, TOpt>
    {
        /// <summary>
        /// A failure result.
        /// </summary>
        public static readonly ParserResult<TValue, TOpt> Failure = new ParserResult<TValue, TOpt>(false, default(TValue), default(TOpt));

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserResult{TValue, TOpt}"/> class.
        /// </summary>
        /// <param name="result">A value indicating whether the parsing was successful.</param>
        /// <param name="value">The parsed value.</param>
        /// <param name="optionalData">The optional data.</param>
        private ParserResult(bool result, TValue value, TOpt optionalData)
        {
            this.Result = result;
            this.Value = value;
            this.OptionalData = optionalData;
        }

        /// <summary>
        /// Gets a value indicating whether the parsing was successful.
        /// </summary>
        public bool Result { get; }

        /// <summary>
        /// Gets the parsed value.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Gets the optional data.
        /// </summary>
        public TOpt OptionalData { get; }

        /// <summary>
        /// Creates a new successful result.
        /// </summary>
        /// <param name="value">The parsed value.</param>
        /// <param name="optionalData">The optional data.</param>
        /// <returns>The result.</returns>
        public static ParserResult<TValue, TOpt> Success(TValue value, TOpt optionalData) => new ParserResult<TValue, TOpt>(true, value, optionalData);
    }
}