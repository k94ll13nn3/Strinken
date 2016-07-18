// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Filters that transforms a value into its length.
    /// </summary>
    internal class LengthFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Transforms a value into its length.";

        /// <inheritdoc/>
        public string Name => "Length";

        /// <inheritdoc/>
        public string Usage => "{tag:Length}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value.Length.ToString();

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
    }
}