// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Filter that transform the input to uppercase.
    /// </summary>
    internal class UpperFilter : FilterWithoutArguments
    {
        /// <inheritdoc/>
        public override string Description => "Converts a string to uppercase.";

        /// <inheritdoc/>
        public override string Name => "Upper";

        /// <inheritdoc/>
        public override string Resolve(string value) => value.ToUpperInvariant();
    }
}