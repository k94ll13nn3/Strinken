namespace Strinken.Core
{
    /// <summary>
    /// An element in the stack.
    /// </summary>
    internal class TokenDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDefinition"/> class.
        /// </summary>
        /// <param name="data">The data related to the token.</param>
        /// <param name="type">The type of the token.</param>
        /// <param name="operatorSymbol">The operator symbol of the token.</param>
        /// <param name="indicatorSymbol">The indicator symbol of the token.</param>
        public TokenDefinition(string data, TokenType type, char operatorSymbol, char indicatorSymbol)
        {
            Data = data;
            Type = type;
            OperatorSymbol = operatorSymbol;
            IndicatorSymbol = indicatorSymbol;
        }

        /// <summary>
        /// Gets the data related to the token (value or name).
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the operator symbol of the token.
        /// </summary>
        public char OperatorSymbol { get; }

        /// <summary>
        /// Gets the indicator symbol of the token.
        /// </summary>
        public char IndicatorSymbol { get; }
    }
}