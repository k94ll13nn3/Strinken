// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Fluent interface for creating the machine.
    /// </summary>
    internal interface ICanBuild
    {
        /// <summary>
        /// Builds the current machine.
        /// </summary>
        /// <returns>The built machine.</returns>
        IStateMachine Build();
    }
}