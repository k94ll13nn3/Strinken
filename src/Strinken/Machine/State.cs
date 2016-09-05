// stylecop.header

namespace Strinken.Machine
{
    /// <summary>
    /// Enumeration describing the current state of the state machine.
    /// </summary>
    internal enum State
    {
        /// <summary>
        /// The machine is outside a token.
        /// </summary>
        OutsideToken,

        /// <summary>
        /// The machine is on an open bracket.
        /// </summary>
        OnOpenBracket,

        /// <summary>
        /// The machine is inside a tag.
        /// </summary>
        InsideTag,

        /// <summary>
        /// The machine is inside a filter.
        /// </summary>
        InsideFilter,

        /// <summary>
        /// The machine is on a filter separator.
        /// </summary>
        OnFilterSeparator,

        /// <summary>
        /// The machine is on a close bracket.
        /// </summary>
        OnCloseBracket,

        /// <summary>
        /// The machine is a the end of the string.
        /// </summary>
        EndOfString,

        /// <summary>
        /// The machine is on an argument initializer or an argument separator.
        /// </summary>
        OnArgumentInitializerOrSeparator,

        /// <summary>
        /// The machine is on a argument tag indicator.
        /// </summary>
        OnArgumentTagIndicator,

        /// <summary>
        /// The machine is inside an argument.
        /// </summary>
        InsideArgument,

        /// <summary>
        /// The machine is inside an argument tag.
        /// </summary>
        InsideArgumentTag,

        /// <summary>
        /// The machine has seen an invalid string.
        /// </summary>
        InvalidString
    }
}