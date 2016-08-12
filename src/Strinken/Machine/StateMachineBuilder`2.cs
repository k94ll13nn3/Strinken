// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Builder used for creating a state machine.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal class StateMachineBuilder<TState, TParameter> : ICanAddStateOrSteppingMethodOrStop<TState, TParameter>, ICanAddAction<TState, TParameter>, ICanAddStart<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// State machine that is being built.
        /// </summary>
        private readonly StateMachine<TState, TParameter> machine;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBuilder{TState, TParameter}"/> class.
        /// </summary>
        private StateMachineBuilder()
        {
            this.machine = new StateMachine<TState, TParameter>();
        }

        /// <summary>
        /// Initializes the builder.
        /// </summary>
        /// <returns>A <see cref="ICanAddStart{TState, TParameter}"/> for chaining.</returns>
        public static ICanAddStart<TState, TParameter> Initialize() => new StateMachineBuilder<TState, TParameter>();

        /// <inheritdoc/>
        public ICanAddState<TState, TParameter> BeforeEachStep(Action action)
        {
            this.machine.BeforeAction = action;
            return this;
        }

        /// <inheritdoc/>
        public StateMachine<TState, TParameter> Build() => this.machine;

        /// <inheritdoc/>
        public ICanAddState<TState, TParameter> Do(Func<TParameter, TState> action)
        {
            this.machine.StateMapper.Add(this.machine.CurrentState, action);
            return this;
        }

        /// <inheritdoc/>
        public ICanAddAction<TState, TParameter> On(TState state)
        {
            if (this.machine.StateMapper.ContainsKey(state))
            {
                throw new InvalidOperationException($"The state {this.machine.CurrentState} was already registered in the machine.");
            }

            this.machine.CurrentState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddStop<TState, TParameter> StartOn(TState state)
        {
            this.machine.StartingState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddStateOrSteppingMethodOrStop<TState, TParameter> StopOn(TState state)
        {
            this.machine.StoppingStates.Add(state);
            return this;
        }
    }
}