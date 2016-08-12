// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring the starting state.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddStart<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Defines on which state the machine should start.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddStop{TState, TParameter}"/> for chaining.</returns>
        ICanAddStop<TState, TParameter> StartOn(TState state);
    }
}