using Strinken.Core;

namespace Strinken
{
    /// <summary>
    /// A compiled string for fast resolution.
    /// </summary>
    public class CompiledString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledString"/> class with the specified <see cref="TokenStack"/>.
        /// </summary>
        /// <param name="stack">The compiled stack.</param>
        internal CompiledString(TokenStack stack)
        {
            Stack = stack;
        }

        /// <summary>
        /// The compiled stack.
        /// </summary>
        internal TokenStack Stack { get; }
    }
}
