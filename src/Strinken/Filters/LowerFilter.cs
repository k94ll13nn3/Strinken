// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Filter that transform the input to lowercase.
    /// </summary>
    internal class LowerFilter : FilterWithoutArguments
    {
        /// <inheritdoc/>
        public override string Description => "Converts a string to lowercase.";

        /// <inheritdoc/>
        public override string Name => "Lower";

        /// <inheritdoc/>
        public override string Resolve(string value) => value.ToLowerInvariant();
    }
}