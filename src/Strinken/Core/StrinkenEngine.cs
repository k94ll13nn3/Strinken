using System.Collections.Generic;

namespace Strinken.Core
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
            EngineResult result;
            if (string.IsNullOrWhiteSpace(input))
            {
                var stack = new TokenStack();
                stack.Push(new TokenDefinition(input, TokenType.None, '\0', '\0'));
                result = new EngineResult(true, stack, null);
            }
            else
            {
                using (var cursor = new Cursor(input))
                {
                    ParseResult<IEnumerable<TokenDefinition>> parsingResult = cursor.ParseString();
                    if (!parsingResult)
                    {
                        result = new EngineResult(false, null, parsingResult.Message);
                    }
                    else
                    {
                        var tokenStack = new TokenStack();
                        tokenStack.PushRange(parsingResult.Value);
                        result = new EngineResult(true, tokenStack, null);
                    }
                }
            }

            return result;
        }
    }
}