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
            var solver = new StrinkenEngine(t => this.tags[t].Resolve(value), (f, t, a) => this.filters[f].Resolve(t, a));
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
                t =>
                {
                    tagList.Add(t);
                    return t;
                },
                (f, t, a) =>
                {
                    filterList.Add(Tuple.Create(f, a));
                    return t;
                });

                validator.Run(input);

                var invalidParameter = tagList.FirstOrDefault(p => !this.tags.ContainsKey(p));
                if (invalidParameter != null)
                {
                    // TODO: Message = {invalidParameter} is not a valid tag
                    return false;
                }

                invalidParameter = filterList.FirstOrDefault(p => !this.filters.ContainsKey(p.Item1))?.Item1;
                if (invalidParameter != null)
                {
                    // TODO: Message = {invalidParameter} is not a valid filter
                    return false;
                }

                invalidParameter = filterList.FirstOrDefault(p => !this.filters[p.Item1].Validate(p.Item2))?.Item1;
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

            this.tags.Add(tag.Name, tag);
        }
    }
}