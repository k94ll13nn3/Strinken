// stylecop.header
using System;
using System.Collections.Generic;
using System.Text;

namespace Strinken.Engine
{
    /// <summary>
    /// Stack used by the engine to manage a list of tokens.
    /// </summary>
    internal class TokenStack
    {
        /// <summary>
        /// The <see cref="StringBuilder"/> used store the current token.
        /// </summary>
        private readonly StringBuilder token;

        /// <summary>
        /// Internal stack.
        /// </summary>
        private readonly Stack<Token> tokenStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenStack"/> class.
        /// </summary>
        public TokenStack()
        {
            this.tokenStack = new Stack<Token>();
            this.token = new StringBuilder();
        }

        /// <summary>
        /// Appends a value to the next token that will be pushed in the stack.
        /// </summary>
        /// <param name="value">The value to append.</param>
        public void Append(char value)
        {
            this.token.Append(value);
        }

        /// <summary>
        /// Adds the current token to the stack.
        /// </summary>
        /// <param name="type">The type of the token.</param>
        /// <param name="subtype">The subtype of the token.</param>
        public void Push(TokenType type, TokenSubtype subtype)
        {
            var data = this.token.ToString();
            this.token.Length = 0;
            this.InternalPush(data, type, subtype);
        }

        /// <summary>
        /// Adds a new verbatim token to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        public void PushVerbatim(char data)
        {
            this.InternalPush(data.ToString(), TokenType.None, TokenSubtype.Base);
        }

        /// <summary>
        /// Adds a new verbatim token to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        public void PushVerbatim(string data)
        {
            this.InternalPush(data, TokenType.None, TokenSubtype.Base);
        }

        /// <summary>
        /// Resolve the stack.
        /// </summary>
        /// <param name="actions">The list of different actions.</param>
        /// <returns>The result of the resolution of the stack.</returns>
        public string Resolve(ActionDictionary actions)
        {
            if (this.tokenStack.Count == 1 && this.tokenStack.Peek().Type == TokenType.None)
            {
                return this.tokenStack.Peek().Data;
            }

            var result = new StringBuilder();
            while (this.tokenStack.Count > 0)
            {
                var nextToken = this.tokenStack.Peek();
                switch (nextToken.Type)
                {
                    case TokenType.None:
                        var currentToken = this.tokenStack.Pop();
                        result.Insert(0, currentToken.Data);
                        break;

                    default:
                        result.Insert(0, this.ResolveTagOrFilter(actions));
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Pushes the provided data to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        /// <param name="type">The type of the token.</param>
        /// <param name="subtype">The subtype of the token.</param>
        private void InternalPush(string data, TokenType type, TokenSubtype subtype)
        {
            // If the top token is a verbatim token and the new token is also a verbatim token, their data are cumulated.
            if (this.tokenStack.Count > 0 && type == TokenType.None && this.tokenStack.Peek().Type == TokenType.None)
            {
                var lastToken = this.tokenStack.Pop();
                this.tokenStack.Push(new Token(lastToken.Data + data, TokenType.None, TokenSubtype.Base));
            }
            else
            {
                this.tokenStack.Push(new Token(data, type, subtype));
            }
        }

        /// <summary>
        /// Resolve a tag or a filter.
        /// </summary>
        /// <param name="actions">The list of different actions.</param>
        /// <returns>The result of the resolution of the tag or the filter.</returns>
        private string ResolveTagOrFilter(ActionDictionary actions)
        {
            var arguments = new List<string>();

            while (this.tokenStack.Count > 0)
            {
                var currentToken = this.tokenStack.Pop();
                switch (currentToken.Type)
                {
                    case TokenType.Argument:
                        arguments.Insert(0, actions?[TokenType.Argument, currentToken.Subtype]?.Invoke(new[] { currentToken.Data }));
                        break;

                    case TokenType.Filter:
                        var temporaryResult = this.ResolveTagOrFilter(actions);

                        // An array is created containing all arguments.
                        var concatenatedArguments = new string[arguments.Count + 2];
                        concatenatedArguments[0] = currentToken.Data;
                        concatenatedArguments[1] = temporaryResult;
                        arguments.CopyTo(concatenatedArguments, 2);
                        return actions?[TokenType.Filter, currentToken.Subtype]?.Invoke(concatenatedArguments);

                    case TokenType.Tag:
                        return actions?[TokenType.Tag, currentToken.Subtype]?.Invoke(new[] { currentToken.Data });
                }
            }

            throw new InvalidOperationException("Internal engine error.");
        }

        /// <summary>
        /// An element in the stack.
        /// </summary>
        private class Token
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
}