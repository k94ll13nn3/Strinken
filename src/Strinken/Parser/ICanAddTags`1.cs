// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Parser
{
    /// <summary>
    /// Fluent interface for adding tags to the parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public interface ICanAddTags<T>
    {
        /// <summary>
        /// Add a tag to the parser.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <returns>A <see cref="ICanAddTagsOrFilters{T}"/> for chaining.</returns>
        ICanAddTagsOrFilters<T> WithTag(ITag<T> tag);

        /// <summary>
        /// Add tags to the parser.
        /// </summary>
        /// <param name="tags">The tags to add.</param>
        /// <returns>A <see cref="ICanAddTagsOrFilters{T}"/> for chaining.</returns>
        ICanAddTagsOrFilters<T> WithTags(IEnumerable<ITag<T>> tags);

        /// <summary>
        /// Add a tag to the parser.
        /// </summary>
        /// <param name="tagName">The description of the tag.</param>
        /// <param name="tagDescription">The name of the tag.</param>
        /// <param name="resolveAction">The action linked to the tag.</param>
        /// <returns>A <see cref="ICanAddTagsOrFilters{T}"/> for chaining.</returns>
        ICanAddTagsOrFilters<T> WithTag(string tagName, string tagDescription, Func<T, string> resolveAction);
    }
}