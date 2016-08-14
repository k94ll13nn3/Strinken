// stylecop.header
using System;

namespace Strinken.Engine
{
    /// <summary>
    /// Results of the engine's run.
    /// </summary>
    internal class EngineResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the engine's run succeed.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the resulting string created after the run in case of success.
        /// </summary>
        public string ParsedString { get; set; }

        /// <summary>
        /// Gets or sets an error message describing the failure in case of failure.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}