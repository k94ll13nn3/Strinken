// stylecop.header
using System.Collections.Generic;
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Base class to share data amongst all parsers.
    /// </summary>
    internal static class FilterHelpers
    {
        /// <summary>
        /// Initializes static members of the <see cref="FilterHelpers"/> class.
        /// </summary>
        static FilterHelpers()
        {
            BaseFilters = new List<IFilter>
            {
                new UpperFilter(),
                new LengthFilter(),
                new LowerFilter(),
                new LeadingZerosFilter(),
                new NullFilter(),
                new IfEqualFilter(),
                new ReplaceFilter()
            };
        }

        /// <summary>
        /// Gets base filters shared by all parsers.
        /// </summary>
        internal static IEnumerable<IFilter> BaseFilters { get; }
    }
}