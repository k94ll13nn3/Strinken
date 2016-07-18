// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Filter that transform the input to uppercase.
    /// </summary>
    internal class UpperFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Converts a string to uppercase.";

        /// <inheritdoc/>
        public string Name => "Upper";

        /// <inheritdoc/>
        public string Usage => "{tag:Upper}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value.ToUpperInvariant();

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
    }
}