using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Core engine of Strinken.
    /// </summary>
    internal class StrinkenEngine
    {
        /// <summary>
        /// Run the engine on a string.
        /// </summary>
        /// <param name="input">The string to process.</param>
        /// <returns>A <see cref="EngineResult"/> containing data about the run.</returns>
        public EngineResult Run(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                var stack = new TokenStack();
                stack.Push(new TokenDefinition(input, TokenType.None, '\0', '\0'));
                return new EngineResult
                {
                    Success = true,
                    Stack = stack,
                    ErrorMessage = null
                };
            }

            ParseResult<IEnumerable<TokenDefinition>> parsingResult;
            using (var cursor = new Cursor(input))
            {
                parsingResult = cursor.ParseString();
            }

            if (!parsingResult)
            {
                return new EngineResult
                {
                    Success = false,
                    Stack = null,
                    ErrorMessage = parsingResult.Message
                };
            }

            var tokenStack = new TokenStack();
            foreach (var token in parsingResult.Value)
            {
                tokenStack.Push(token);
            }

            return new EngineResult
            {
                Success = true,
                Stack = tokenStack,
                ErrorMessage = null
            };
        }
    }
}