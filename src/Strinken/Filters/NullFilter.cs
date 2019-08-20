namespace Strinken
{
    /// <summary>
    /// Returns the specified argument if the input is null.
    /// </summary>
    internal class NullFilter : IFilter
    {
        /// <inheritdoc/>
        public string Description => "Returns the specified argument if the input is null.";

        /// <inheritdoc/>
        public string Name => "Null";

        /// <inheritdoc/>
        public string Usage => "{tag:Null+valueIfNull} or {tag:??+valueIfNull}";

        /// <inheritdoc/>
        public string AlternativeName => "??";

        /// <inheritdoc/>
        public string Resolve(string value, string[] arguments) => value ?? arguments[0];

        /// <inheritdoc/>
        public bool Validate(string[] arguments) => arguments?.Length == 1;
    }
}
