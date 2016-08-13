// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action on a state.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddAction<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Action to perform on the specified state.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A <see cref="ICanAddState{TState, TParameter}"/> for chaining.</returns>
        ICanAddState<TState, TParameter> Do(Func<TParameter, TState> action);

        /// <summary>
        /// Creates a sink state (i.e. a state that stop the machine and returns a failure).
        /// </summary>
        /// <returns>A <see cref="ICanAddState{TState, TParameter}"/> for chaining.</returns>
        ICanAddState<TState, TParameter> Sink();
    }
}