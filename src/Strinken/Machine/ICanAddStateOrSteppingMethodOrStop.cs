// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for declaring an action to perform before each state, a stopping state or a state on which an action must be performed.
    /// </summary>
    internal interface ICanAddStateOrSteppingMethodOrStop : ICanAddStateOrSteppingMethod, ICanAddStop
    {
    }
}