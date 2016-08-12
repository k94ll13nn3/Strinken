// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring a stopping state.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddStop<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Defines on which state the machine should stop.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddStateOrSteppingMethod{TState, TParameter}"/> for chaining.</returns>
        ICanAddStateOrSteppingMethodOrStop<TState, TParameter> StopOn(TState state);
    }
}