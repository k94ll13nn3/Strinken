// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring a stopping state.
    /// </summary>
    internal interface ICanAddStop
    {
        /// <summary>
        /// Defines on which state the machine should stop.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddStateOrSteppingMethod"/> for chaining.</returns>
        ICanAddStateOrSteppingMethodOrStop StopOn(State state);
    }
}