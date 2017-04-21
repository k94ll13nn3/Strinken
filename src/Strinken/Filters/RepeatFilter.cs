using System.Linq;
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Repeats a string as many times as specified.
    /// </summary>
    internal class ReapeatFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Repeats a string as many times as specified.";

        /// <inheritdoc/>
        public string Name => "Repeat";

        /// <inheritdoc/>
        public string Usage => "{tag:Repeat+numberOfTimes}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => string.Concat(Enumerable.Repeat(value, int.Parse(arguments[0])));

        /// <inheritdoc/>
        public bool Validate(string[] arguments)
        {
            return arguments?.Length == 1 && int.TryParse(arguments[0], out var n);
        }
    }
}