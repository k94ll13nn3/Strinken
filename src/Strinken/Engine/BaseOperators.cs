﻿using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Class that handles the base operators.
    /// </summary>
    internal static class BaseOperators
    {
        /// <summary>
        /// Gets base operators shared by all parsers.
        /// </summary>
        internal static IEnumerable<Operator> RegisteredOperators { get; } = new List<Operator>
        {
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Tag,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Filter,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Full }
                }
            },
            new Operator
            {
                Symbol = '=',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name }
                }
            }
        };
    }
}