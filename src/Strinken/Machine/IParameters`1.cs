// stylecop.header
namespace Strinken.Machine
{
    /// <summary>
    /// Interface describing parameters used by the state machine actions.
    /// </summary>
    /// <typeparam name="TState">Type describing the states used by the state machine.</typeparam>
    public interface IParameters<TState>
    {
        /// <summary>
        /// Gets or sets the message linked to the current state.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the current state.
        /// </summary>
        TState CurrentState { get; set; }
    }
}