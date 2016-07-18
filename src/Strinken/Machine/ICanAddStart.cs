// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring the starting state.
    /// </summary>
    internal interface ICanAddStart
    {
        /// <summary>
        /// Defines on which state the machine should start.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddStateOrSteppingMethod"/> for chaining.</returns>
        ICanAddStop StartOn(State state);
    }
}