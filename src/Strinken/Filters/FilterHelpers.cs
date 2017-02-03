// stylecop.header
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Common;
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Base class to share data amongst all parsers.
    /// </summary>
    public static class FilterHelpers
    {
        /// <summary>
        /// Lock object for operations on the filters list.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// The list of registered filters.
        /// </summary>
        private static readonly IDictionary<string, IFilter> RegisteredFilters;

        /// <summary>
        /// Initializes static members of the <see cref="FilterHelpers"/> class.
        /// </summary>
        static FilterHelpers()
        {
            var baseFilters = new List<IFilter>
            {
                new UpperFilter(),
                new LengthFilter(),
                new LowerFilter(),
                new LeadingZerosFilter(),
                new NullFilter(),
                new IfEqualFilter(),
                new ReplaceFilter()
            };

            RegisteredFilters = baseFilters.ToDictionary(x => x.Name, x => x);
        }

        /// <summary>
        /// Gets base filters shared by all parsers.
        /// </summary>
        internal static IReadOnlyCollection<IFilter> BaseFilters
        {
            get
            {
                lock (Lock)
                {
                    return new ReadOnlyCollection<IFilter>(RegisteredFilters.Values.ToList());
                }
            }
        }

        /// <summary>
        /// Registers a filter that will be used as a base filter for all parser built after.
        /// </summary>
        /// <typeparam name="T">The type of the filter.</typeparam>
        /// <param name="filter">The filter to register.</param>
        public static void Register<T>(T filter)
            where T : IFilter
        {
            lock (Lock)
            {
                if (!RegisteredFilters.ContainsKey(filter.Name))
                {
                    if (string.IsNullOrWhiteSpace(filter.Name))
                    {
                        throw new ArgumentException("A name cannot be empty.");
                    }

                    for (int i = 0; i < filter.Name.Length; i++)
                    {
                        if (filter.Name[i].IsInvalidTokenNameCharacter())
                        {
                            throw new ArgumentException($"{filter.Name[i]} is an invalid character for a name.");
                        }
                    }

                    RegisteredFilters.Add(filter.Name, filter);
                }
            }
        }

        /// <summary>
        /// Unregisters a filter from the base filters.
        /// </summary>
        /// <typeparam name="T">The type of the filter.</typeparam>
        /// <param name="filter">The filter to unregister.</param>
        public static void UnRegister<T>(T filter)
            where T : IFilter
        {
            lock (Lock)
            {
                if (RegisteredFilters.ContainsKey(filter.Name))
                {
                    RegisteredFilters.Remove(filter.Name);
                }
            }
        }
    }
}