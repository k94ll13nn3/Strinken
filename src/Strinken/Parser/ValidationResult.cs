namespace Strinken
{
    /// <summary>
    /// Class representing the result of the validation process.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="message">A message describing why the input is not valid.</param>
        /// <param name="isValid">A value indicating whether the input was valid.</param>
        public ValidationResult(string message, bool isValid)
        {
            Message = message;
            IsValid = isValid;
        }

        /// <summary>
        /// Gets or sets a message describing why the input is not valid.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the input was valid.
        /// </summary>
        public bool IsValid { get; set; }
    }
}
