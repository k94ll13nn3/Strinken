// stylecop.header
namespace Strinken
{
    /// <summary>
    /// Class representing the result of the validation process.
    /// </summary>
    public sealed class ValidationResult
    {
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
