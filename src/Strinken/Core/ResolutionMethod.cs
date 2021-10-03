namespace Strinken.Core;

/// <summary>
/// Method of resolution.
/// </summary>
internal enum ResolutionMethod
{
    /// <summary>
    /// array[a[0]].Resolve(value)
    /// </summary>
    Tag = 0,

    /// <summary>
    /// array[a[0]].Resolve().
    /// </summary>
    ParameterTag = 1,

    /// <summary>
    /// array[a[0]].Resolve(a[1], a.Skip(2).ToArray()).
    /// </summary>
    Filter = 2,

    /// <summary>
    /// a[0].
    /// </summary>
    Name = 3
}
