// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Filter that transform the input to lowercase.
    /// </summary>
    internal class LowerFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Converts a string to lowercase.";

        /// <inheritdoc/>
        public string Name => "Lower";

        /// <inheritdoc/>
        public string Usage => "{tag:Lower}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value.ToLowerInvariant();

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
    }
}