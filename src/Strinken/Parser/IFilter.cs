// stylecop.header

namespace Strinken
{
    /// <summary>
    /// Interface describing a filter.
    /// </summary>
    public interface IFilter : IToken
    {
        /// <summary>
        /// Gets the usage of the filter.
        /// </summary>
        string Usage { get; }

        /// <summary>
        /// Resolves the filter.
        /// </summary>
        /// <param name="value">The value on which the filter is applied.</param>
        /// <param name="arguments">Arguments passed to the filter.</param>
        /// <returns>The resulting value.</returns>
        string Resolve(string value, string[] arguments);

        /// <summary>
        /// Validates the arguments that will be passed to the filter.
        /// </summary>
        /// <param name="arguments">Arguments passed to the filter.</param>
        /// <returns>A value indicating whether the arguments are valid.</returns>
        bool Validate(string[] arguments);
    }
}