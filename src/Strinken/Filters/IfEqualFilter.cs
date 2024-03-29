namespace Strinken;

/// <summary>
/// Does an equality test with the input and the specified value and renders a value depending on the output of the test.
/// </summary>
internal class IfEqualFilter : IFilter
{
    /// <inheritdoc/>
    public string Description => "Does an equality test with the input and the specified value and renders a value depending on the output of the test.";

    /// <inheritdoc/>
    public string Name => "IfEqual";

    /// <inheritdoc/>
    public string Usage => "{tag:IfEqual+value,valueIfTrue,valueIfFalse} or {tag:?+value,valueIfTrue,valueIfFalse}";

    /// <inheritdoc/>
    public string AlternativeName => "?";

    /// <inheritdoc/>
    public string Resolve(string value, string[] arguments)
    {
        return value == arguments[0] ? arguments[1] : arguments[2];
    }

    /// <inheritdoc/>
    public bool Validate(string[] arguments)
    {
        return arguments?.Length == 3;
    }
}
