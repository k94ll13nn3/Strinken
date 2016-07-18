// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Returns the specified argument if the input value is null.
    /// </summary>
    internal class NullFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Returns the specified argument if the input value is null.";

        /// <inheritdoc/>
        public string Name => "Null";

        /// <inheritdoc/>
        public string Usage => "{tag:Null+valueIfNull}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value ?? arguments[0];

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments?.Length == 1;
    }
}