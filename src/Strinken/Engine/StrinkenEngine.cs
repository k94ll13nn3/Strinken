// stylecop.header
using System;
using Strinken.Common;
using Strinken.Core.Parsing;
using Strinken.Core.Types;
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
        /// Processes the inside of a token.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessToken()
        {
            if (this.cursor.Value == SpecialCharacter.TokenStartIndicator)
            {
                // Escaped TokenEnd
                this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenStartIndicator);
                return State.OutsideToken;
            }

            if (this.cursor.HasEnded())
            {
                this.errorMessage = string.Format(Errors.IllegalCharacterAtStringEnd, '{');
                return State.InvalidString;
            }

            var parsingResult = StringParser.ParseString(this.cursor);
            if (parsingResult.Result)
            {
                foreach (var token in parsingResult.Value)
                {
                    this.tokenStack.Push(token);
                }

                return State.OutsideToken;
            }
            else
            {
                this.errorMessage = parsingResult.Message;
                return State.InvalidString;
            }
        }

        /// <summary>
        /// Processes the <see cref="State.OnTokenEndIndicator"/> state.
        /// </summary>
        /// <returns>The new state.</returns>
        private State ProcessOnTokenEndIndicator()
        {
            State state;
            switch (this.cursor.Value)
            {
                case SpecialCharacter.TokenEndIndicator:
                    // Escaped TokenEnd
                    this.tokenStack.PushVerbatim((char)SpecialCharacter.TokenEndIndicator);
                    state = State.OutsideToken;
                    break;

                default:
                    state = this.RaiseError(string.Format(Errors.IllegalCharacter, (char)SpecialCharacter.TokenEndIndicator, this.cursor.Position - 1));
                    break;
            }

            return state;
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
                    this.tokenStack.PushVerbatim((char)this.cursor.Value);
                    state = State.OutsideToken;
                    break;
            }

            return state;
        }

        /// <summary>
        /// Append the current cursor value to the current token and returns the specified state, or an invalid state if the cursor is not valid.
        /// </summary>
        /// <param name="nextStateIfValid">The <see cref="State"/> to return if the cursor is valid..</param>
        /// <param name="validate">A value indicating whether the cursor must be validated before appending.</param>
        /// <returns>The new state.</returns>
        private State AppendAndMove(State nextStateIfValid, bool validate = false)
        {
            var value = (char)this.cursor.Value;
            if (validate && value.IsInvalidTokenNameCharacter())
            {
                return this.RaiseIllegalCharacterError();
            }

            this.tokenStack.Append((char)this.cursor.Value);
            return nextStateIfValid;
        }

        /// <summary>
        /// Sets the current error message and returns an invalid state.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns><see cref="State.InvalidString"/></returns>
        private State RaiseError(string error)
        {
            this.errorMessage = error;
            return State.InvalidString;
        }

        /// <summary>
        /// Pushes the current token to the stack, resets it and move to a new state.
        /// </summary>
        /// <param name="nextState">The next state.</param>
        /// <param name="tokenType">The type of token to push.</param>
        /// <param name="tokenSubtype">The subtype of token to push.</param>
        /// <returns>The new state.</returns>
        private State PushAndMove(State nextState, TokenType tokenType, TokenSubtype tokenSubtype)
        {
            this.tokenStack.Push(tokenType, tokenSubtype);
            return nextState;
        }

        /// <summary>
        /// Handle the <see cref="RaiseError"/> method for the specified <see cref="TokenType"/>.
        /// </summary>
        /// <param name="tokenType">The type of the token.</param>
        /// <returns><see cref="State.InvalidString"/></returns>
        private State EmptyTokenError(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Argument:
                    return this.RaiseError(Errors.EmptyArgument);

                case TokenType.Filter:
                    return this.RaiseError(Errors.EmptyFilter);

                case TokenType.Tag:
                    return this.RaiseError(Errors.EmptyTag);

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Handle the <see cref="RaiseError"/> method for an illegal character.
        /// </summary>
        /// <returns><see cref="State.InvalidString"/></returns>
        private State RaiseIllegalCharacterError() => this.RaiseError(string.Format(Errors.IllegalCharacter, (char)this.cursor.Value, this.cursor.Position));
    }
}