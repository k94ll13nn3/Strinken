// stylecop.header

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
                stack.PushVerbatim(input);
                return new EngineResult
                {
                    Success = true,
                    Stack = stack,
                    ErrorMessage = null
                };
            }

            var tokenStack = new TokenStack();
            using (var cursor = new Cursor(input))
            {
                var parsingResult = cursor.ParseString();
                if (!parsingResult.Result)
                {
                    return new EngineResult
                    {
                        Success = false,
                        Stack = null,
                        ErrorMessage = parsingResult.Message
                    };
                }

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
}