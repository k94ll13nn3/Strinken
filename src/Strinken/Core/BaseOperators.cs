using System.Collections.Generic;

namespace Strinken.Core
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
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.Tag },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.ParameterTag }
                }
            },
            new Operator
            {
                Symbol = '@',
                TokenType = TokenType.Tag,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Full, ResolutionMethod = ResolutionMethod.Name },
                }
            },
            new Operator
            {
                Symbol = '#',
                TokenType = TokenType.Tag,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = 'b', ParsingMethod = ParsingMethod.Binary, ResolutionMethod = ResolutionMethod.Name },
                    new Indicator{ Symbol = 'o', ParsingMethod = ParsingMethod.Octal, ResolutionMethod = ResolutionMethod.Name },
                    new Indicator{ Symbol = 'd', ParsingMethod = ParsingMethod.Decimal, ResolutionMethod = ResolutionMethod.Name },
                    new Indicator{ Symbol = 'x', ParsingMethod = ParsingMethod.Hexadecimal, ResolutionMethod = ResolutionMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Filter,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.NameOrSymbol, ResolutionMethod = ResolutionMethod.Filter }
                }
            },
            new Operator
            {
                Symbol = '\0',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Full, ResolutionMethod = ResolutionMethod.Name }
                }
            },
            new Operator
            {
                Symbol = '=',
                TokenType = TokenType.Argument,
                Indicators = new List<Indicator>
                {
                    new Indicator{ Symbol = '\0', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.Tag },
                    new Indicator{ Symbol = '!', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.ParameterTag }
                }
            }
        };
    }
}