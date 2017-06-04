// stylecop.header

namespace Strinken.Filters
{
    /// <summary>
    /// Filters that transforms a value into its length.
    /// </summary>
    internal class LengthFilter : FilterWithoutArguments
    {
        /// <inheritdoc/>
        public override string Description => "Transforms a value into its length.";

        /// <inheritdoc/>
        public override string Name => "Length";

        /// <inheritdoc/>
        public override string Resolve(string value) => value.Length.ToString();
    }
}