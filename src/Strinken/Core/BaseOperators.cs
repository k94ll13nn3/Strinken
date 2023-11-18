namespace Strinken.Core;

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
        new(
            '\0',
            TokenType.Tag,
            new List<Indicator>
            {
                new() { Symbol = '\0', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.Tag },
                new() { Symbol = '!', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.ParameterTag }
            }
        ),
        new(
            '@',
            TokenType.Tag,
            new List<Indicator>
            {
                new() { Symbol = '\0', ParsingMethod = ParsingMethod.Full, ResolutionMethod = ResolutionMethod.Name },
            }
        ),
        new(
            '#',
            TokenType.Tag,
            new List<Indicator>
            {
                new() { Symbol = 'b', ParsingMethod = ParsingMethod.Binary, ResolutionMethod = ResolutionMethod.Name },
                new() { Symbol = 'o', ParsingMethod = ParsingMethod.Octal, ResolutionMethod = ResolutionMethod.Name },
                new() { Symbol = 'd', ParsingMethod = ParsingMethod.Decimal, ResolutionMethod = ResolutionMethod.Name },
                new() { Symbol = 'x', ParsingMethod = ParsingMethod.Hexadecimal, ResolutionMethod = ResolutionMethod.Name }
            }
        ),
        new(
            '\0',
            TokenType.Filter,
            new List<Indicator>
            {
                new() { Symbol = '\0', ParsingMethod = ParsingMethod.NameOrSymbol, ResolutionMethod = ResolutionMethod.Filter }
            }
        ),
        new(
            '\0',
            TokenType.Argument,
            new List<Indicator>
            {
                new() { Symbol = '\0', ParsingMethod = ParsingMethod.Full, ResolutionMethod = ResolutionMethod.Name }
            }
        ),
        new(
            '=',
            TokenType.Argument,
            new List<Indicator>
            {
                new() { Symbol = '\0', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.Tag },
                new() { Symbol = '!', ParsingMethod = ParsingMethod.Name, ResolutionMethod = ResolutionMethod.ParameterTag }
            }
        )
    };
}
