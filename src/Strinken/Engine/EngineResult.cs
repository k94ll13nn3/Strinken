// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// Results of the engine's run.
    /// </summary>
    internal class EngineResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EngineResult"/> class.
        /// </summary>
        /// <param name="success">A value indicating whether the parsing was successful.</param>
        /// <param name="stack">The parsed stack.</param>
        /// <param name="errorMessage">The message associated to the parsing.</param>
        public EngineResult(bool success, TokenStack stack, string errorMessage)
        {
            Success = success;
            Stack = stack;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the engine's run succeed.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the resulting stack created after the run in case of success.
        /// </summary>
        public TokenStack Stack { get; set; }

        /// <summary>
        /// Gets or sets an error message describing the failure in case of failure.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}