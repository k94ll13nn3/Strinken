// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action to perform before each state.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddStateOrSteppingMethod<TState, TParameter> : ICanAddState<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Defines the action to perform before each state of the machine.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A <see cref="ICanAddState{TState, TParameter}"/> for chaining.</returns>
        ICanAddState<TState, TParameter> BeforeEachStep(Action action);
    }
}