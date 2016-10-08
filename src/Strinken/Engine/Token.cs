// stylecop.header

namespace Strinken.Engine
{
    /// <summary>
    /// An element in the stack.
    /// </summary>
    internal class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="data">The data related to the token.</param>
        /// <param name="type">The type of the token.</param>
        /// <param name="subtype">The subtype of the token.</param>
        public Token(string data, TokenType type, TokenSubtype subtype)
        {
            this.Data = data;
            this.Type = type;
            this.Subtype = subtype;
        }

        /// <summary>
        /// Gets the data related to the token (value or name).
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// Gets the subtype of the token.
        /// </summary>
        public TokenSubtype Subtype { get; }
    }
}