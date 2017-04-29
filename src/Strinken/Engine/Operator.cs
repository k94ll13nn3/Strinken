using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// An operator that defines a set of indicators.
    /// </summary>
    internal class Operator
    {
        public IEnumerable<Indicator> Indicators { get; set; }
        public char Symbol { get; set; }
    }
}