using System.Globalization;
using System.Linq;

namespace Strinken
{
    /// <summary>
    /// Repeats the input as many times as specified.
    /// </summary>
    internal class RepeatFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Repeats the input as many times as specified.";

        /// <inheritdoc/>
        public string Name => "Repeat";

        /// <inheritdoc/>
        public string Usage => "{tag:Repeat+numberOfTimes} or {tag:*+numberOfTimes}";

        /// <inheritdoc/>
        public string AlternativeName => "*";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments)
        {
            return string.Concat(Enumerable.Repeat(value, int.Parse(arguments[0], CultureInfo.InvariantCulture)));
        }

        /// <inheritdoc/>
        public bool Validate(string[] arguments)
        {
            return arguments?.Length == 1 && int.TryParse(arguments[0], out int _);
        }
    }
}
