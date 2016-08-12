// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring a state in the machine.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddState<TState, TParameter> : ICanBuild<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// State to add to the machine.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>A <see cref="ICanAddAction{TState, TParameter}"/> for chaining.</returns>
        ICanAddAction<TState, TParameter> On(TState state);
    }
}