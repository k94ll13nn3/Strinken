// stylecop.header
using Strinken.Machine;

namespace Strinken.Engine
{
    /// <summary>
    /// Core engine of Strinken.
    /// </summary>
    internal class StrinkenEngine
    {
        /// <summary>
        /// The cursor used to read the string.
        /// </summary>
        private Cursor cursor;

        /// <summary>
        /// The error message in case of failure.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// The stack of tokens.
        /// </summary>
        private TokenStack tokenStack;

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

            this.tokenStack = new TokenStack();
            this.errorMessage = null;

            using (this.cursor = new Cursor(input))
            {
                var machine = StateMachineBuilder
                    .Initialize()
                    .StartOn(State.OutsideToken)
                    .StopOn(State.EndOfString)
                    .BeforeEachStep(this.cursor.Next)
                    .On(State.OutsideToken).Do(this.ProcessOutsideToken)
                    .On(State.OnTokenStartIndicator).Do(this.ProcessToken)
                    .On(State.OnTokenEndIndicator).Do(this.ProcessOnTokenEndIndicator)
                    .On(State.InvalidString).Sink()
                    .Build();

                var success = machine.Run();
                return new EngineResult
                {
                    Success = success,
                    Stack = success ? this.tokenStack : null,
                    ErrorMessage = this.errorMessage
                };
            }
        }

        /// <summary>
        /// Processes the <see cref="State.OnTokenEndIndicator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnTokenEndIndicator()
        {
            // Escaped TokenEnd
            if (this.cursor.Value == SpecialCharacter.TokenEndIndicator)
            {
                this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenEndIndicator);
                return State.OutsideToken;
            }

            this.errorMessage = string.Format(Errors.IllegalCharacter, (char)SpecialCharacter.TokenEndIndicator, this.cursor.Position - 1);
            return State.InvalidString;
        }

        /// <summary>
        /// Processes the <see cref="State.OutsideToken"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOutsideToken()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.EndOfStringIndicator:
                    state = State.EndOfString;
                    break;

                case SpecialCharacter.TokenStartIndicator:
                    state = State.OnTokenStartIndicator;
                    break;

                case SpecialCharacter.TokenEndIndicator:
                    state = State.OnTokenEndIndicator;
                    break;

                default:
                    this.tokenStack.PushVerbatim(this.cursor.CharValue);
                    state = State.OutsideToken;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Processes the inside of a token.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessToken()
        {
            // Escaped TokenStart
            if (this.cursor.Value == SpecialCharacter.TokenStartIndicator)
            {
                this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenStartIndicator);
                return State.OutsideToken;
            }

            if (this.cursor.HasEnded())
            {
                this.errorMessage = string.Format(Errors.IllegalCharacterAtStringEnd, '{');
                return State.InvalidString;
            }

            var parsingResult = this.cursor.ParseString();
            if (!parsingResult.Result)
            {
                this.errorMessage = parsingResult.Message;
                return State.InvalidString;
            }

            foreach (var token in parsingResult.Value)
            {
                this.tokenStack.Push(token);
            }

            return State.OutsideToken;
        }
    }
}