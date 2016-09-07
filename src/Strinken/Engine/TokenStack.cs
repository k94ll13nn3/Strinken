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
        /// <param name="value"></param>
        public void Append(char value)
        {
            this.token.Append(value);
        }

        /// <summary>
        /// Adds the current token to the stack.
        /// </summary>
        /// <param name="type">The type of the token.</param>
        public void Push(TokenType type)
        {
            var data = this.token.ToString();
            this.token.Length = 0;
            this.InternalPush(data, type);
        }

        /// <summary>
        /// Adds a new verbatim token to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        public void PushVerbatim(char data)
        {
            this.InternalPush(data.ToString(), TokenType.VerbatimString);
        }

        /// <summary>
        /// Adds a new verbatim token to the stack.
        /// </summary>
        /// <param name="data">The data of the token.</param>
        public void PushVerbatim(string data)
        {
            this.InternalPush(data, TokenType.VerbatimString);
        }

        /// <summary>
        /// Resolve the stack.
        /// </summary>
        /// <param name="actionOnTags">Action to perform on tags. The argument is the tag name.</param>
        /// <param name="actionOnFilters">
        ///     Action to perform on filters.
        ///     The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </param>
        /// <returns>The result of the resolution of the stack.</returns>
        public string Resolve(Func<string, string> actionOnTags, Func<string, string, string[], string> actionOnFilters)
        {
            if (this.tokenStack.Count == 1 && this.tokenStack.Peek().Type == TokenType.VerbatimString)
            {
                return this.tokenStack.Peek().Data;
            }

            var result = new StringBuilder();
            while (this.tokenStack.Count > 0)
            {
                var nextToken = this.tokenStack.Peek();
                switch (nextToken.Type)
                {
                    case TokenType.VerbatimString:
                        var currentToken = this.tokenStack.Pop();
                        result.Insert(0, currentToken.Data);
                        break;

                    default:
                        result.Insert(0, this.ResolveTagOrFilter(actionOnTags, actionOnFilters));
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
        private void InternalPush(string data, TokenType type)
        {
            // If the top token is a verbatim token and the new token is also a verbatim token, their data are cumulated.
            if (this.tokenStack.Count > 0 && type == TokenType.VerbatimString && this.tokenStack.Peek().Type == TokenType.VerbatimString)
            {
                var lastToken = this.tokenStack.Pop();
                this.tokenStack.Push(new Token(lastToken.Data + data, TokenType.VerbatimString));
            }
            else
            {
                this.tokenStack.Push(new Token(data, type));
            }
        }

        /// <summary>
        /// Resolve a tag or a filter.
        /// </summary>
        /// <param name="actionOnTags">Action to perform on tags. The argument is the tag name.</param>
        /// <param name="actionOnFilters">
        ///     Action to perform on filters.
        ///     The arguments are the filter name, the name of the tag on which the filter is applied and the arguments to pass to the filter.
        /// </param>
        /// <returns>The result of the resolution of the tag or the filter.</returns>
        private string ResolveTagOrFilter(Func<string, string> actionOnTags, Func<string, string, string[], string> actionOnFilters)
        {
            var arguments = new List<string>();

            while (this.tokenStack.Count > 0)
            {
                var currentToken = this.tokenStack.Pop();
                switch (currentToken.Type)
                {
                    case TokenType.ArgumentTag:
                        arguments.Insert(0, actionOnTags?.Invoke(currentToken.Data));
                        break;

                    case TokenType.Argument:
                        arguments.Insert(0, currentToken.Data);
                        break;

                    case TokenType.Filter:
                        var temporaryResult = this.ResolveTagOrFilter(actionOnTags, actionOnFilters);
                        return actionOnFilters?.Invoke(currentToken.Data, temporaryResult, arguments.ToArray());

                    case TokenType.Tag:
                        return actionOnTags?.Invoke(currentToken.Data);
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