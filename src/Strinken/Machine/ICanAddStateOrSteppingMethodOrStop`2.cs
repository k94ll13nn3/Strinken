// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action to perform before each state, a stopping state or a state on which an action must be performed.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal interface ICanAddStateOrSteppingMethodOrStop<TState, TParameter> : ICanAddStateOrSteppingMethod<TState, TParameter>, ICanAddStop<TState, TParameter>
        where TParameter : IParameters<TState>
    {
    }
}