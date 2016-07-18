// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action to perform before each state.
    /// </summary>
    internal interface ICanAddStateOrSteppingMethod : ICanAddState
    {
        /// <summary>
        /// Defines the action to perform before each state of the machine.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>A <see cref="ICanAddState"/> for chaining.</returns>
        ICanAddState BeforeEachStep(Action action);
    }
}