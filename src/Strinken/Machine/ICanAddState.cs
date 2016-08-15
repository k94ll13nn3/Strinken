// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring a state in the machine.
    /// </summary>
    internal interface ICanAddState : ICanBuild
    {
        /// <summary>
        /// State to add to the machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddAction"/> for chaining.</returns>
        ICanAddAction On(State state);
    }
}