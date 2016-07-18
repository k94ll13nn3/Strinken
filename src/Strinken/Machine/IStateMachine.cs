// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Base interface for the state machine.
    /// </summary>
    internal interface IStateMachine
    {
        /// <summary>
        /// Run the machine.
        /// </summary>
        void Run();
    }
}