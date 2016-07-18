// stylecop.header

namespace Strinken.Parser
{
    /// <summary>
    /// Fluent interface for creating the parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public interface ICanBuild<in T>
    {
        /// <summary>
        /// Builds the current parser.
        /// </summary>
        /// <returns>The built parser.</returns>
        IParser<T> Build();
    }
}