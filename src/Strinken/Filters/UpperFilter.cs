namespace Strinken
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
        public override string AlternativeName => string.Empty;

        /// <inheritdoc/>
        public override string Resolve(string value)
        {
            return value?.ToUpperInvariant() ?? string.Empty;
        }
    }
}
