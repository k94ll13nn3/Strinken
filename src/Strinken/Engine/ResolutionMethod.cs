namespace Strinken.Engine
{
    /// <summary>
    /// Method of resolution.
    /// </summary>
    internal enum ResolutionMethod
    {
        /// <summary>
        /// array[a[0]].Resolve(value)
        /// </summary>
        WithValue,

        /// <summary>
        /// array[a[0]].Resolve().
        /// </summary>
        WithoutValue,

        /// <summary>
        /// array[a[0]].Resolve(a[1], a.Skip(2).ToArray()).
        /// </summary>
        WithArguments,

        /// <summary>
        /// a[0].
        /// </summary>
        Name
    }
}