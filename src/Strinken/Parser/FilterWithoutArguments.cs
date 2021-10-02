namespace Strinken;

/// <summary>
/// Base class for all filters that do not have arguments.
/// </summary>
public abstract class FilterWithoutArguments : IFilter
{
    /// <inheritdoc/>
    public abstract string Description { get; }

    /// <inheritdoc/>
    public abstract string Name { get; }

    /// <inheritdoc/>
    public abstract string AlternativeName { get; }

    /// <inheritdoc/>
    public string Usage => $"{{tag:{Name}}}{(!string.IsNullOrWhiteSpace(AlternativeName) ? $" or {{tag:{AlternativeName}}}" : string.Empty)}";

    /// <inheritdoc/>
    public string Resolve(string value, string[] arguments)
    {
        return Resolve(value);
    }

    /// <summary>
    /// Resolves the filter.
    /// </summary>
    /// <param name="value">The value on which the filter is applied.</param>
    /// <returns>The resulting value.</returns>
    public abstract string Resolve(string value);

    /// <inheritdoc/>
    public bool Validate(string[] arguments)
    {
        return arguments == null || arguments.Length == 0;
    }
}
