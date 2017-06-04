﻿using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Base class for all filters that do not have arguments.
    /// </summary>
    public abstract class FilterWithoutArguments : IFilter
    {
        /// <inheritdoc/>
        public abstract string Description { get; }

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <inheritdoc/>
        public string Usage => $"{{tag:{Name}}}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => Resolve(value);

        /// <summary>
        /// Resolves the filter.
        /// </summary>
        /// <param name="value">The value on which the filter is applied.</param>
        /// <returns>The resulting value.</returns>
        public abstract string Resolve(string value);

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
    }
}