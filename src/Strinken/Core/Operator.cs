using System.Collections.Generic;

namespace Strinken.Core
{
    /// <summary>
    /// An operator that defines a set of indicators.
    /// </summary>
    internal class Operator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operator"/> class.
        /// </summary>
        /// <param name="symbol">The set of indicators defines by the operator.</param>
        /// <param name="tokenType">The symbol linked to the operator.</param>
        /// <param name="indicators">The type of token on which the operator is used.</param>
        public Operator(char symbol, TokenType tokenType, IEnumerable<Indicator> indicators)
        {
            Symbol = symbol;
            TokenType = tokenType;
            Indicators = indicators;
        }

        /// <summary>
        /// Gets or sets the set of indicators defines by the operator.
        /// </summary>
        public IEnumerable<Indicator> Indicators { get; set; }

        /// <summary>
        /// Gets or sets the symbol linked to the operator.
        /// </summary>
        public char Symbol { get; set; }

        /// <summary>
        /// Gets or sets the type of token on which the operator is used.
        /// </summary>
        public TokenType TokenType { get; set; }
    }
}
