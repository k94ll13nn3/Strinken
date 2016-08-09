// stylecop.header
using System;
using System.Text;

namespace Strinken.Engine
{
    /// <summary>
    /// Parameters used by the engine to process a string.
    /// </summary>
    internal class EngineParameters : IDisposable
    {
        /// <summary>
        /// A value indicating whether the <see cref="Dispose()"/> method has already been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineParameters"/> class.
        /// </summary>
        /// <param name="input">The string to read.</param>
        /// <param name="actionOnTags">Action to perform on tags. The argument is the tag name.</param>
        /// <param name="actionOnFilters">
        ///     Action to perform on filters.
        ///     The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </param>
        public EngineParameters(string input, Func<string, string> actionOnTags, Func<string, string, string[], string> actionOnFilters)
        {
            this.Cursor = new Cursor(input);
            this.Result = new StringBuilder();
            this.Token = new StringBuilder();
            this.TokenStack = new TokenStack(actionOnTags, actionOnFilters);
        }

        /// <summary>
        /// Gets or sets the cursor used to read the string.
        /// </summary>
        public Cursor Cursor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StringBuilder"/> used to generate the resulting string.
        /// </summary>
        public StringBuilder Result { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="StringBuilder"/> used store the current token.
        /// </summary>
        public StringBuilder Token { get; set; }

        /// <summary>
        /// Gets or sets the stack of tokens.
        /// </summary>
        public TokenStack TokenStack { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation of the dispose method.
        /// </summary>
        /// <param name="disposing">A value indicating whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Cursor?.Dispose();
            }

            this.disposed = true;
        }
    }
}