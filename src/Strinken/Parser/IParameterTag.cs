// stylecop.header

namespace Strinken.Parser
{
    /// <summary>
    /// Interface describing a parameter tag.
    /// </summary>
    public interface IParameterTag : IToken
    {
        /// <summary>
        /// Resolves the parameter tag.
        /// </summary>
        /// <returns>The resulting value.</returns>
        string Resolve();
    }
}