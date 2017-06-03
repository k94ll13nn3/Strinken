using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// An operator that defines a set of indicators.
    /// </summary>
    internal class Operator
    {
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