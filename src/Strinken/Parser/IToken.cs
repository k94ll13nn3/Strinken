// stylecop.header

namespace Strinken
{
    /// <summary>
    /// Base interface describing a token.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Gets the name of the token.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the token.
        /// </summary>
        string Description { get; }
    }
}