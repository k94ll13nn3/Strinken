// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Stack used by the engine to manage a list of tokens.
    /// </summary>
    internal class TokenStack
    {
        /// <summary>
        /// Action to perform on filters. The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </summary>
        private readonly Func<string, string, string[], string> actionOnFilters;

        /// <summary>
        /// Action to perform on tags. The argument is the tag name.
        /// </summary>
        private readonly Func<string, string> actionOnTags;

        /// <summary>
        /// Internal stack.
        /// </summary>
        private readonly Stack<Token> tokenStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenStack"/> class.
        /// </summary>
        /// <param name="actionOnTags">Action to perform on tags. The argument is the tag name.</param>
        /// <param name="actionOnFilters">
        ///     Action to perform on filters.
        ///     The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </param>
        public TokenStack(Func<string, string> actionOnTags, Func<string, string, string[], string> actionOnFilters)
        {
            this.actionOnTags = actionOnTags;
            this.actionOnFilters = actionOnFilters;
            this.tokenStack = new Stack<Token>();
        }

        /// <summary>
        /// Add a new token to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        /// <param name="type">The type of the token.</param>
        public void Push(string data, TokenType type)
        {
            this.tokenStack.Push(new Token(data, type));
        }

        /// <summary>
        /// Resolve the stack.
        /// </summary>
        /// <returns>The result of the resolution of the stack.</returns>
        public string Resolve()
        {
            var arguments = new List<string>();

            while (this.tokenStack.Count > 0)
            {
                var currentToken = this.tokenStack.Pop();
                switch (currentToken.Type)
                {
                    case TokenType.ArgumentTag:
                        arguments.Insert(0, this.actionOnTags?.Invoke(currentToken.Data));
                        break;

                    case TokenType.Argument:
                        arguments.Insert(0, currentToken.Data);
                        break;

                    case TokenType.Filter:
                        var temporaryResult = this.Resolve();
                        return this.actionOnFilters?.Invoke(currentToken.Data, temporaryResult, arguments.ToArray());

                    case TokenType.Tag:
                        return this.actionOnTags?.Invoke(currentToken.Data);
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
            public Token(string data, TokenType type)
            {
                this.Data = data;
                this.Type = type;
            }

            /// <summary>
            /// Gets the data related to the token (value or name).
            /// </summary>
            public string Data { get; }

            /// <summary>
            /// Gets the type of the token.
            /// </summary>
            public TokenType Type { get; }
        }
    }
}