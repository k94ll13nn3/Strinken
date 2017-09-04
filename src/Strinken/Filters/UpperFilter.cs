// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Transforms the input to uppercase.
    /// </summary>
    internal class UpperFilter : FilterWithoutArguments
    {
        /// <inheritdoc/>
        public override string Description => "Transforms the input to uppercase.";

        /// <inheritdoc/>
        public override string Name => "Upper";

        /// <inheritdoc/>
        public override string Resolve(string value) => value.ToUpperInvariant();
    }
}