// stylecop.header
using Strinken.Parser;

namespace Strinken.Filters
{
    /// <summary>
    /// Do an equality tests and render a value depending on the output of the test.
    /// </summary>
    internal class IfEqualFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Do an equality tests and render a value depending on the output of the test.";

        /// <inheritdoc/>
        public string Name => "IfEqual";

        /// <inheritdoc/>
        public string Usage => "{tag:IfEqual+value,valueIfTrue,valueIfFalse}";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value == arguments[0] ? arguments[1] : arguments[2];

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments?.Length == 3;
    }
}