// stylecop.header
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Engine;
using Strinken.Filters;

namespace Strinken.Parser
{
    /// <summary>
    /// Base implementation of a parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public class ParserBuilder<T> : IParser<T>, ICanAddTagsOrFilters<T>
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

        /// <inheritdoc/>
        public IReadOnlyCollection<IFilter> Filters => new ReadOnlyCollection<IFilter>(this.filters.Values.ToList());

        /// <inheritdoc/>
        public IReadOnlyCollection<ITag<T>> Tags => new ReadOnlyCollection<ITag<T>>(this.tags.Values.ToList());

        /// <summary>
        /// Initializes the parser.
        /// </summary>
        /// <returns>A <see cref="ICanAddTags{T}"/> for chaining.</returns>
        public static ICanAddTags<T> Initialize() => new ParserBuilder<T>();

        /// <inheritdoc/>
        public IParser<T> Build() => this;

        /// <inheritdoc/>
        public string Resolve(string input, T value)
        {
            var solver = new StrinkenEngine(
                tagName => this.tags[tagName].Resolve(value),
                (filterName, valueToPass, arguments) => this.filters[filterName].Resolve(valueToPass, arguments));
            return solver.Run(input);
        }

        /// <inheritdoc/>
        public bool Validate(string input)
        {
            try
            {
                var tagList = new List<string>();
                var filterList = new List<Tuple<string, string[]>>();
                var validator = new StrinkenEngine(
                tagName =>
                {
                    tagList.Add(tagName);
                    return string.Empty;
                },
                (filterName, valueToPass, arguments) =>
                {
                    filterList.Add(Tuple.Create(filterName, arguments));
                    return string.Empty;
                });

                validator.Run(input);

                // Find the first tag that was not registered in the parser.
                var invalidParameter = tagList.FirstOrDefault(tagName => !this.tags.ContainsKey(tagName));
                if (invalidParameter != null)
                {
                    // TODO: Message = {invalidParameter} is not a valid tag
                    return false;
                }

                // Find the first filter that was not registered in the parser.
                invalidParameter = filterList.FirstOrDefault(filter => !this.filters.ContainsKey(filter.Item1))?.Item1;
                if (invalidParameter != null)
                {
                    // TODO: Message = {invalidParameter} is not a valid filter
                    return false;
                }

                // Find the first filter that has invalid arguments.
                invalidParameter = filterList.FirstOrDefault(filter => !this.filters[filter.Item1].Validate(filter.Item2))?.Item1;
                if (invalidParameter != null)
                {
                    // TODO: Message = {invalidParameter} does not have valid arguments
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

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