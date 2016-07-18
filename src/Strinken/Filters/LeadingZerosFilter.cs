// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Formats a string with the leading zeros until reaching the specified length.
    /// </summary>
    internal class LeadingZerosFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Formats a string with the leading zeros until reaching the specified length.";

        /// <inheritdoc/>
        public string Name => "Zeros";

        /// <inheritdoc/>
        public string Usage => "{tag:Zeros+numberOfLeadingZeros}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value.PadLeft(int.Parse(arguments[0]), '0');

        /// <inheritdoc/>
        public bool Validate(string[] arguments)
        {
            int n;
            return arguments?.Length == 1 && int.TryParse(arguments[0], out n);
        }
    }
}