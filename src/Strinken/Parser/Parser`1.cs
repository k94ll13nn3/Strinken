// stylecop.header
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Engine;

namespace Strinken.Parser
{
    /// <summary>
    /// Base interface for a parser.
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
        /// <param name="tags">Tags used by the parser.</param>
        /// <param name="filters">Filters used by the parser.</param>
        public Parser(IDictionary<string, ITag<T>> tags, IDictionary<string, IFilter> filters)
        {
            this.tags = tags;
            this.filters = filters;
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
            return solver.Run(input);
        }

        /// <summary>
        /// Validates an input.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>A value indicating whether the input is valid.</returns>
        public ValidationResult Validate(string input)
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
            }
            catch (FormatException e)
            {
                return new ValidationResult { Message = $"The input is not correctly formatted ({e.Message}).", IsValid = false };
            }

            return new ValidationResult { Message = null, IsValid = true };
        }
    }
}