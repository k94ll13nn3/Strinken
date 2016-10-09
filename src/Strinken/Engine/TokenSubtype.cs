// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// Subtype of a token.
    /// </summary>
    internal enum TokenSubtype
    {
        /// <summary>
        /// Base token.
        /// </summary>
        Base,

        /// <summary>
        /// The token is also a tag token.
        /// </summary>
        Tag,

        /// <summary>
        /// The token is also a parameter tag token.
        /// </summary>
        ParameterTag
    }
}