// stylecop.header
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Common;
using Strinken.Engine;
using Strinken.Filters;

namespace Strinken.Parser
{
    /// <summary>
    /// Strinken parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public class Parser<T>
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
        /// Initializes a new instance of the <see cref="Parser{T}"/> class.
        /// </summary>
        public Parser()
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
        /// Gets the filters used by the parser.
        /// </summary>
        public IReadOnlyCollection<IFilter> Filters => new ReadOnlyCollection<IFilter>(this.filters.Values.ToList());

        /// <summary>
        /// Gets the tags used by the parser.
        /// </summary>
        public IReadOnlyCollection<ITag<T>> Tags => new ReadOnlyCollection<ITag<T>>(this.tags.Values.ToList());

        /// <summary>
        /// Resolves the input.
        /// </summary>
        /// <param name="input">The input to resolve.</param>
        /// <param name="value">The value to pass to the tags.</param>
        /// <returns>The resolved input.</returns>
        public string Resolve(string input, T value)
        {
            var solver = new StrinkenEngine(
                tagName => this.tags[tagName].Resolve(value),
                (filterName, valueToPass, arguments) => this.filters[filterName].Resolve(valueToPass, arguments));
            var runResult = solver.Run(input);
            return runResult.Success ? runResult.ParsedString : null;
        }

        /// <summary>
        /// Validates an input.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>A value indicating whether the input is valid.</returns>
        public ValidationResult Validate(string input)
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

            var runResult = validator.Run(input);
            if (!runResult.Success)
            {
                return new ValidationResult { Message = runResult.ErrorMessage, IsValid = false };
            }

            // Find the first tag that was not registered in the parser.
            var invalidParameter = tagList.FirstOrDefault(tagName => !this.tags.ContainsKey(tagName));
            if (invalidParameter != null)
            {
                return new ValidationResult { Message = $"{invalidParameter} is not a valid tag.", IsValid = false };
            }

            // Find the first filter that was not registered in the parser.
            invalidParameter = filterList.FirstOrDefault(filter => !this.filters.ContainsKey(filter.Item1))?.Item1;
            if (invalidParameter != null)
            {
                return new ValidationResult { Message = $"{invalidParameter} is not a valid filter.", IsValid = false };
            }

            // Find the first filter that has invalid arguments.
            invalidParameter = filterList.FirstOrDefault(filter => !this.filters[filter.Item1].Validate(filter.Item2))?.Item1;
            if (invalidParameter != null)
            {
                return new ValidationResult { Message = $"{invalidParameter} does not have valid arguments.", IsValid = false };
            }

            return new ValidationResult { Message = null, IsValid = true };
        }

        /// <summary>
        /// Add a filter to the list of filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddFilter(IFilter filter)
        {
            if (this.filters.ContainsKey(filter.Name))
            {
                throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
            }

            ThrowIfInvalidName(filter.Name);
            this.filters.Add(filter.Name, filter);
        }

        /// <summary>
        /// Add a tag to the list of tags.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public void AddTag(ITag<T> tag)
        {
            if (this.tags.ContainsKey(tag.Name))
            {
                throw new ArgumentException($"{tag.Name} was already registered in the tag list.");
            }

            ThrowIfInvalidName(tag.Name);
            this.tags.Add(tag.Name, tag);
        }

        /// <summary>
        /// Creates a deep copy of the current parser.
        /// </summary>
        /// <returns>A deep copy of the parser.</returns>
        internal Parser<T> DeepCopy()
        {
            var newParser = new Parser<T>();
            foreach (var tag in this.tags.Values)
            {
                newParser.AddTag(tag);
            }

            var parserOwnFilters = this.filters.Where(f => !FilterHelpers.BaseFilters.Select(bf => bf.Name).Contains(f.Value.Name));
            foreach (var filter in parserOwnFilters)
            {
                newParser.AddFilter(filter.Value);
            }

            return newParser;
        }

        /// <summary>
        /// Validates a name and throws a <see cref="ArgumentException"/> if the name is invalid.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        private static void ThrowIfInvalidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A name cannot be empty.");
            }

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i].IsValidTokenNameCharacter())
                {
                    throw new ArgumentException($"{name[i]} is an invalid character for a name.");
                }
            }
        }
    }
}