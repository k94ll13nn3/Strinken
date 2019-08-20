// stylecop.header

namespace Strinken
{
    /// <summary>
    /// Interface describing a tag.
    /// </summary>
    /// <typeparam name="T">The type related to the tag.</typeparam>
    public interface ITag<in T> : IToken
    {
        /// <summary>
        /// Resolves the tag.
        /// </summary>
        /// <param name="value">The value used by the tag to be resolved.</param>
        /// <returns>The resulting value.</returns>
        string Resolve(T value);
    }
}
