// stylecop.header
using System.Collections.Generic;

namespace Strinken.Parser
{
    /// <summary>
    /// Fluent interface for adding tags or filters to the parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public interface ICanAddTagsOrFilters<T> : ICanBuild<T>, ICanAddTags<T>
    {
        /// <summary>
        /// Add a filter to the parser.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>A <see cref="ICanAddTagsOrFilters{T}"/> for chaining.</returns>
        ICanAddTagsOrFilters<T> WithFilter(IFilter filter);

        /// <summary>
        /// Add filters to the parser.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>A <see cref="ICanAddTagsOrFilters{T}"/> for chaining.</returns>
        ICanAddTagsOrFilters<T> WithFilters(IEnumerable<IFilter> filters);
    }
}