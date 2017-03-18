// stylecop.header
using System;

namespace Strinken.Machine
{
    /// <summary>
    /// Builder used for creating a state machine.
    /// </summary>
    internal class StateMachineBuilder : ICanAddStateOrSteppingMethodOrStop, ICanAddAction, ICanAddStart
    {
        /// <summary>
        /// State machine that is being built.
        /// </summary>
        private readonly StateMachine machine;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBuilder"/> class.
        /// </summary>
        private StateMachineBuilder()
        {
            machine = new StateMachine();
        }

        /// <summary>
        /// Initializes the builder.
        /// </summary>
        /// <returns>A <see cref="ICanAddStart"/> for chaining.</returns>
        public static ICanAddStart Initialize() => new StateMachineBuilder();

        /// <inheritdoc/>
        public ICanAddState BeforeEachStep(Action action)
        {
            machine.BeforeAction = action;
            return this;
        }

        /// <inheritdoc/>
        public StateMachine Build() => machine;

        /// <inheritdoc/>
        public ICanAddState Do(Func<State> action)
        {
            machine.StateMapper.Add(machine.CurrentState, action);
            return this;
        }

        /// <inheritdoc/>
        public ICanAddAction On(State state)
        {
            if (machine.StateMapper.ContainsKey(state))
            {
                throw new InvalidOperationException($"The state {machine.CurrentState} was already registered in the machine.");
            }

            machine.CurrentState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddState Sink()
        {
            machine.SinkingStates.Add(machine.CurrentState);
            return this;
        }

        /// <inheritdoc/>
        public ICanAddStop StartOn(State state)
        {
            machine.StartingState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddStateOrSteppingMethodOrStop StopOn(State state)
        {
            machine.StoppingStates.Add(state);
            return this;
        }
    }
}