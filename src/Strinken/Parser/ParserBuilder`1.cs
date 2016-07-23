// stylecop.header
using System;
using System.Collections.Generic;
using Strinken.Filters;

namespace Strinken.Parser
{
    /// <summary>
    /// Base implementation of a parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public class ParserBuilder<T> : ICanAddTagsOrFilters<T>
    {
        /// <summary>
        /// Filters used by the parser.
        /// </summary>
        private readonly IDictionary<string, IFilter> filters;

        /// <summary>
        /// Tags used by the parser.
        /// </summary>
        private readonly IDictionary<string, ITag<T>> tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserBuilder{T}"/> class.
        /// </summary>
        private ParserBuilder()
        {
            this.tags = new Dictionary<string, ITag<T>>();
            this.filters = new Dictionary<string, IFilter>();

            foreach (var filter in FilterHelpers.BaseFilters)
            {
                if (this.filters.ContainsKey(filter.Name))
                {
                    throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
                }

                this.filters.Add(filter.Name, filter);
            }
        }

        /// <summary>
        /// Initializes the parser.
        /// </summary>
        /// <returns>A <see cref="ICanAddTags{T}"/> for chaining.</returns>
        public static ICanAddTags<T> Initialize() => new ParserBuilder<T>();

        /// <inheritdoc/>
        public Parser<T> Build() => new Parser<T>(this.tags, this.filters);

        /// <inheritdoc/>
        public ICanAddTagsOrFilters<T> WithFilter(IFilter filter)
        {
            this.AddFilter(filter);

            return this;
        }

        /// <inheritdoc/>
        public ICanAddTagsOrFilters<T> WithFilters(IEnumerable<IFilter> filters)
        {
            foreach (var filter in filters)
            {
                this.AddFilter(filter);
            }

            return this;
        }

        /// <inheritdoc/>
        public ICanAddTagsOrFilters<T> WithTag(ITag<T> tag)
        {
            this.AddTag(tag);

            return this;
        }

        /// <inheritdoc/>
        public ICanAddTagsOrFilters<T> WithTag(string tagName, string tagDescription, Func<T, string> resolveAction)
        {
            var tag = TagFactory.Create(tagName, tagDescription, resolveAction);

            this.AddTag(tag);

            return this;
        }

        /// <inheritdoc/>
        public ICanAddTagsOrFilters<T> WithTags(IEnumerable<ITag<T>> tags)
        {
            foreach (var tag in tags)
            {
                this.AddTag(tag);
            }

            return this;
        }

        /// <summary>
        /// Add a filter to the list of filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        private void AddFilter(IFilter filter)
        {
            if (this.filters.ContainsKey(filter.Name))
            {
                throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
            }

            if (string.IsNullOrWhiteSpace(filter.Name))
            {
                throw new ArgumentException("A filter cannot have an empty name.");
            }

            this.filters.Add(filter.Name, filter);
        }

        /// <summary>
        /// Add a tag to the list of tags.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        private void AddTag(ITag<T> tag)
        {
            if (this.tags.ContainsKey(tag.Name))
            {
                throw new ArgumentException($"{tag.Name} was already registered in the tag list.");
            }

            if (string.IsNullOrWhiteSpace(tag.Name))
            {
                throw new ArgumentException("A tag cannot have an empty name.");
            }

            this.tags.Add(tag.Name, tag);
        }
    }
}