using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Core;

namespace Strinken
{
    /// <summary>
    /// Class that handles the base filters and (un)registration of base filters.
    /// </summary>
    public static class BaseFilters
    {
        /// <summary>
        /// Lock object for operations on the filters list.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The list of registered filters.
        /// </summary>
        private static readonly IDictionary<string, IFilter> InternalRegisteredFilters;

        /// <summary>
        /// Initializes static members of the <see cref="BaseFilters"/> class.
        /// </summary>
        static BaseFilters()
        {
            var baseFilters = new List<IFilter>
            {
                new UpperFilter(),
                new LengthFilter(),
                new LowerFilter(),
                new LeadingZerosFilter(),
                new NullFilter(),
                new IfEqualFilter(),
                new ReplaceFilter(),
                new ReapeatFilter()
            };

            InternalRegisteredFilters = baseFilters.ToDictionary(x => x.Name, x => x);
        }

        /// <summary>
        /// Gets base filters shared by all parsers.
        /// </summary>
        internal static IReadOnlyCollection<IFilter> RegisteredFilters
        {
            get
            {
                lock (Lock)
                {
                    return new ReadOnlyCollection<IFilter>(InternalRegisteredFilters.Values.ToList());
                }
            }
        }

        /// <summary>
        /// Registers a filter that will be used as a base filter for all parser built after.
        /// </summary>
        /// <param name="filter">The filter to register.</param>
        /// <exception cref="ArgumentNullException">The filter is null.</exception>
        /// <exception cref="ArgumentException">The filter name is invalid or already present.</exception>
        public static void Register(IFilter filter)
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            lock (Lock)
            {
                if (!InternalRegisteredFilters.ContainsKey(filter.Name))
                {
                    filter.Name.ThrowIfInvalidName();
                    InternalRegisteredFilters.Add(filter.Name, filter);
                }
                else
                {
                    throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
                }
            }
        }

        /// <summary>
        /// Unregisters a filter from the base filters.
        /// </summary>
        /// <param name="filterName">The name of the filter to unregister.</param>
        public static void Unregister(string filterName)
        {
            lock (Lock)
            {
                if (InternalRegisteredFilters.ContainsKey(filterName))
                {
                    InternalRegisteredFilters.Remove(filterName);
                }
            }
        }
    }
}