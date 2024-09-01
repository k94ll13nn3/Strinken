namespace Strinken;

/// <summary>
/// Transforms the input to lowercase.
/// </summary>
internal class LowerFilter : FilterWithoutArguments
{
    /// <inheritdoc/>
    public override string Description => "Transforms the input to lowercase.";

    /// <inheritdoc/>
    public override string Name => "Lower";

    /// <inheritdoc/>
    public override string AlternativeName => string.Empty;

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Globalization",
        "CA1308:Normalize strings to uppercase",
        Justification = "It is the goal of the filter")]
#endif
    public override string Resolve(string value)
    {
        return value?.ToLowerInvariant() ?? string.Empty;
    }
}
