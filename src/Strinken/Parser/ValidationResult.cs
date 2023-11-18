namespace Strinken;

/// <summary>
/// Class representing the result of the validation process.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationResult"/> class.
/// </remarks>
/// <param name="message">A message describing why the input is not valid.</param>
/// <param name="isValid">A value indicating whether the input was valid.</param>
public sealed class ValidationResult(string message, bool isValid)
{
    /// <summary>
    /// Gets or sets a message describing why the input is not valid.
    /// </summary>
    public string Message { get; set; } = message;

    /// <summary>
    /// Gets or sets a value indicating whether the input was valid.
    /// </summary>
    public bool IsValid { get; set; } = isValid;
}
