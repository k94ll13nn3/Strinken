namespace Strinken;

/// <summary>
/// Transforms the input into its length.
/// </summary>
internal class LengthFilter : FilterWithoutArguments
{
    /// <inheritdoc/>
    public override string Description => "Transforms the input into its length.";

    /// <inheritdoc/>
    public override string Name => "Length";

    /// <inheritdoc/>
    public override string AlternativeName => string.Empty;

    /// <inheritdoc/>
    public override string Resolve(string value)
    {
        return $"{value?.Length ?? 0}";
    }
}
