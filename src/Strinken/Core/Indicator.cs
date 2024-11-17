namespace Strinken.Core;

/// <summary>
/// An indicator that follows an operator and defines a parsing method.
/// </summary>
internal class Indicator
{
    /// <summary>
    /// Gets or sets the symbol linked to the indicator.
    /// </summary>
    public char Symbol { get; set; }

    /// <summary>
    /// Gets or sets the parsing method linked to the indicator.
    /// </summary>
    public ParsingMethod ParsingMethod { get; set; }

    /// <summary>
    /// Gets or sets the resolution method linked to the indicator.
    /// </summary>
    public ResolutionMethod ResolutionMethod { get; set; }
}
