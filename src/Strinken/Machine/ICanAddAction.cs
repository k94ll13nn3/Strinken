// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action on a state.
    /// </summary>
    internal interface ICanAddAction
    {
        /// <summary>
        /// Action to perform on the specified state.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A <see cref="ICanAddState"/> for chaining.</returns>
        ICanAddState Do(Func<State> action);
    }
}