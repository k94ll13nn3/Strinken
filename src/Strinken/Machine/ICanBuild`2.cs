// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for creating the machine.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanBuild<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Builds the current machine.
        /// </summary>
        /// <returns>The built machine.</returns>
        StateMachine<TState, TParameter> Build();
    }
}