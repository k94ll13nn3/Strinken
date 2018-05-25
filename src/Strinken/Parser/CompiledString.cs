using Strinken.Core;

namespace Strinken
{
    /// <summary>
    /// A compiled string for fast resolution.
    /// </summary>
    public class CompiledString
    {
        internal CompiledString(TokenStack stack)
        {
            Stack = stack;
        }

        internal TokenStack Stack { get; }
    }
}