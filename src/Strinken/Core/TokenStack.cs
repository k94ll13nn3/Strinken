﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Strinken.Core
{
    /// <summary>
    /// Stack used by the engine to manage a list of tokens.
    /// </summary>
    internal class TokenStack
    {
        /// <summary>
        /// Internal stack.
        /// </summary>
        private readonly Stack<TokenDefinition> tokenStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenStack"/> class.
        /// </summary>
        public TokenStack()
        {
            tokenStack = new Stack<TokenDefinition>();
        }

        /// <summary>
        /// Pushes a token to the stack.
        /// </summary>
        /// <param name="token">The token to push.</param>
        public void Push(TokenDefinition token)
        {
            tokenStack.Push(token);
        }

        /// <summary>
        /// Pushes the elements of the specified collection of tokens to the stack.
        /// </summary>
        /// <param name="tokens">The tokens to push.</param>
        public void PushRange(IEnumerable<TokenDefinition> tokens)
        {
            foreach (var token in tokens)
            {
                tokenStack.Push(token);
            }
        }

        /// <summary>
        /// Resolve the stack.
        /// </summary>
        /// <param name="actions">The list of different actions.</param>
        /// <returns>The result of the resolution of the stack.</returns>
        public string Resolve(ActionDictionary actions)
        {
            if (tokenStack.Count == 1 && tokenStack.Peek().Type == TokenType.None)
            {
                return tokenStack.Peek().Data;
            }

            var result = new StringBuilder();
            while (tokenStack.Count > 0)
            {
                var nextToken = tokenStack.Peek();
                switch (nextToken.Type)
                {
                    case TokenType.None:
                        var currentToken = tokenStack.Pop();
                        result.Insert(0, currentToken.Data);
                        break;

                    default:
                        result.Insert(0, ResolveTagOrFilter(actions));
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Resolve a tag or a filter.
        /// </summary>
        /// <param name="actions">The list of different actions.</param>
        /// <returns>The result of the resolution of the tag or the filter.</returns>
        private string ResolveTagOrFilter(ActionDictionary actions)
        {
            var arguments = new List<string>();

            while (tokenStack.Count > 0)
            {
                var currentToken = tokenStack.Pop();
                switch (currentToken.Type)
                {
                    case TokenType.Argument:
                        arguments.Insert(0, actions?[TokenType.Argument, currentToken.OperatorSymbol, currentToken.IndicatorSymbol]?.Invoke(new[] { currentToken.Data }));
                        break;

                    case TokenType.Filter:
                        var temporaryResult = ResolveTagOrFilter(actions);

                        // An array is created containing all arguments.
                        var concatenatedArguments = new string[arguments.Count + 2];
                        concatenatedArguments[0] = currentToken.Data;
                        concatenatedArguments[1] = temporaryResult;
                        arguments.CopyTo(concatenatedArguments, 2);
                        return actions?[TokenType.Filter, currentToken.OperatorSymbol, currentToken.IndicatorSymbol]?.Invoke(concatenatedArguments);

                    case TokenType.Tag:
                        return actions?[TokenType.Tag, currentToken.OperatorSymbol, currentToken.IndicatorSymbol]?.Invoke(new[] { currentToken.Data });
                }
            }

            throw new InvalidOperationException("Internal engine error.");
        }
    }
}