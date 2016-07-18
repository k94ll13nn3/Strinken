// stylecop.header
using System.Collections.Generic;

namespace Strinken.Parser
{
    /// <summary>
    /// Base interface for a parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public interface IParser<in T>
    {
        /// <summary>
        /// Gets the filters used by the parser.
        /// </summary>
        IReadOnlyCollection<IFilter> Filters { get; }

        /// <summary>
        /// Gets the tags used by the parser.
        /// </summary>
        IReadOnlyCollection<ITag<T>> Tags { get; }

        /// <summary>
        /// Resolves the input.
        /// </summary>
        /// <param name="input">The input to resolve.</param>
        /// <param name="value">The value to pass to the tags.</param>
        /// <returns>The resolved input.</returns>
        string Resolve(string input, T value);

        /// <summary>
        /// Validates an input.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>A value indicating whether the input is valid.</returns>
        bool Validate(string input);
    }
}