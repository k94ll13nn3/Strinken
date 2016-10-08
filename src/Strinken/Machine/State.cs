// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Enumeration describing the possible states of the state machine.
    /// </summary>
    internal enum State
    {
        /// <summary>
        /// The machine is outside a token.
        /// </summary>
        OutsideToken,

        /// <summary>
        /// The machine is on a token start indicator.
        /// </summary>
        OnTokenStartIndicator,

        /// <summary>
        /// The machine is on a token end indicator.
        /// </summary>
        OnTokenEndIndicator,

        /// <summary>
        /// The machine is a the end of the string.
        /// </summary>
        EndOfString,

        /// <summary>
        /// The machine has seen an invalid string.
        /// </summary>
        InvalidString
    }
}