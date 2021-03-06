namespace Strinken
{
    /// <summary>
    /// Transforms the input to lowercase.
    /// </summary>
    internal class LowerFilter : FilterWithoutArguments
    {
        /// <inheritdoc/>
        public override string Description => "Transforms the input to lowercase.";

        /// <inheritdoc/>
        public override string Name => "Lower";

        /// <inheritdoc/>
        public override string AlternativeName => string.Empty;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308", Justification = "That is the point of the filter to transform to lower")]
        public override string Resolve(string value)
        {
            return value?.ToLowerInvariant() ?? string.Empty;
        }
    }
}
