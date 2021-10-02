namespace Strinken;

/// <summary>
/// Takes some couples of arguments, and replace each occurrence of each first argument by the second.
/// </summary>
internal class ReplaceFilter : IFilter
{
    /// <inheritdoc/>
    public string Description => "Takes some couples of arguments, and replace each occurrence of each first argument by the second.";

    /// <inheritdoc/>
    public string Name => "Replace";

    /// <inheritdoc/>
    public string Usage => "{tag:Replace+value1,replaceValue1,value2,replaceValue2...}";

    /// <inheritdoc/>
    public string AlternativeName => string.Empty;

    /// <inheritdoc/>
    public string Resolve(string value, string[] arguments)
    {
        string newValue = value;
        if (value is not null)
        {
            for (int i = 0; i < arguments.Length / 2; i++)
            {
                newValue = newValue.Replace(arguments[i * 2], arguments[(i * 2) + 1]);
            }
        }

        return newValue;
    }

    /// <inheritdoc/>
    public bool Validate(string[] arguments)
    {
        return arguments?.Length > 0 && arguments?.Length % 2 == 0;
    }
}
