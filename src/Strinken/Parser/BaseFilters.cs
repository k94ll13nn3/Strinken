using System.Collections.ObjectModel;

namespace Strinken;

/// <summary>
/// Class that handles the base filters and (un)registration of base filters.
/// </summary>
public static class BaseFilters
{
    /// <summary>
    /// Lock object for operations on the filters list.
    /// </summary>
    private static readonly object Lock = new();

    /// <summary>
    /// The list of registered filters.
    /// </summary>
    private static readonly Dictionary<string, IFilter> InternalRegisteredFilters =
        new List<IFilter>
        {
                new UpperFilter(),
                new LengthFilter(),
                new LowerFilter(),
                new LeadingZerosFilter(),
                new NullFilter(),
                new IfEqualFilter(),
                new ReplaceFilter(),
                new RepeatFilter()
        }.ToDictionary(x => x.Name, x => x);

    /// <summary>
    /// Registers a filter that will be used as a base filter for all parser built after.
    /// </summary>
    /// <param name="filter">The filter to register.</param>
    /// <exception cref="ArgumentNullException">The filter is null.</exception>
    /// <exception cref="ArgumentException">The filter name is invalid or already present.</exception>
#if NET8_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1864:Prefer the 'IDictionary.TryAdd(TKey, TValue)' method", Justification = "Cannot just call TryAdd, this would change the behavior")]
#endif
    public static void Register(IFilter filter)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(filter);
#else
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }
#endif

        lock (Lock)
        {
            if (!InternalRegisteredFilters.ContainsKey(filter.Name))
            {
                filter.Name.ThrowIfInvalidName();
                if (!string.IsNullOrWhiteSpace(filter.AlternativeName))
                {
                    if (InternalRegisteredFilters.Values.Select(x => x.AlternativeName).Contains(filter.AlternativeName))
                    {
                        throw new ArgumentException($"A base filter already has {filter.AlternativeName} as its alternative name.");
                    }

                    filter.AlternativeName.ThrowIfInvalidAlternativeName();
                }

                InternalRegisteredFilters.Add(filter.Name, filter);
            }
            else
            {
                throw new ArgumentException($"{filter.Name} was already registered in the base filter list.");
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
            InternalRegisteredFilters.Remove(filterName);
        }
    }

    /// <summary>
    /// Gets base filters shared by all parsers.
    /// </summary>
    internal static IReadOnlyCollection<IFilter> GetRegisteredFilters()
    {
        lock (Lock)
        {
            return new ReadOnlyCollection<IFilter>([.. InternalRegisteredFilters.Values]);
        }
    }
}
