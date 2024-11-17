namespace Strinken.Core;

/// <summary>
/// Results of the engine's run.
/// </summary>
internal record EngineResult(bool Success, TokenStack Stack, string ErrorMessage);

/// <summary>
/// An operator that defines a set of indicators.
/// </summary>
internal record Operator(char Symbol, TokenType TokenType, IEnumerable<Indicator> Indicators);

/// <summary>
/// An element in the stack.
/// </summary>
internal record TokenDefinition(string Data, TokenType Type, char OperatorSymbol, char IndicatorSymbol);
