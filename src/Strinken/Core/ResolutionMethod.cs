namespace Strinken.Core
{
    /// <summary>
    /// Method of resolution.
    /// </summary>
    internal enum ResolutionMethod
    {
        /// <summary>
        /// array[a[0]].Resolve(value)
        /// </summary>
        Tag,

        /// <summary>
        /// array[a[0]].Resolve().
        /// </summary>
        ParameterTag,

        /// <summary>
        /// array[a[0]].Resolve(a[1], a.Skip(2).ToArray()).
        /// </summary>
        Filter,

        /// <summary>
        /// a[0].
        /// </summary>
        Name
    }
}
