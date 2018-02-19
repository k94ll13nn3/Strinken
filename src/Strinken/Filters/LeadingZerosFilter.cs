namespace Strinken
{
    /// <summary>
    /// Formats the input with leading zeros until reaching the specified length.
    /// </summary>
    internal class LeadingZerosFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Formats the input with leading zeros until reaching the specified length.";

        /// <inheritdoc/>
        public string Name => "Zeros";

        /// <inheritdoc/>
        public string Usage => "{tag:Zeros+numberOfLeadingZeros}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value.PadLeft(int.Parse(arguments[0]), '0');

        /// <inheritdoc/>
        public bool Validate(string[] arguments)
        {
            return arguments?.Length == 1 && int.TryParse(arguments[0], out var n);
        }
    }
}