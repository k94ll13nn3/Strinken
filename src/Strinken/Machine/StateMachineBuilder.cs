// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Machine
{
    /// <summary>
    /// Builder used for creating a state machine.
    /// </summary>
    internal class StateMachineBuilder : IStateMachine, ICanAddStateOrSteppingMethodOrStop, ICanAddAction, ICanAddStart
    {
        /// <summary>
        /// Mapper used to resolve action depending on state.
        /// </summary>
        private readonly Dictionary<State, Func<State>> stateMapper;

        /// <summary>
        /// List of states that stops the machine.
        /// </summary>
        private readonly ICollection<State> stoppingStates;

        /// <summary>
        /// Current state added specified by the <see cref="ICanAddState.On(State)"/> method.
        /// </summary>
        private State currentState;

        /// <summary>
        /// Action to perform before each state of the machine.
        /// </summary>
        private Action beforeAction;

        /// <summary>
        /// Starting state of the machine.
        /// </summary>
        private State startingState;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineBuilder"/> class.
        /// </summary>
        private StateMachineBuilder()
        {
            this.stateMapper = new Dictionary<State, Func<State>>();
            this.stoppingStates = new List<State>();
        }

        /// <summary>
        /// Initializes the builder.
        /// </summary>
        /// <returns>A <see cref="ICanAddStart"/> for chaining.</returns>
        public static ICanAddStart Initialize() => new StateMachineBuilder();

        /// <inheritdoc/>
        public IStateMachine Build() => this;

        /// <inheritdoc/>
        public ICanAddState Do(Func<State> action)
        {
            this.stateMapper.Add(this.currentState, action);
            return this;
        }

        /// <inheritdoc/>
        public ICanAddAction On(State state)
        {
            if (this.stateMapper.ContainsKey(state))
            {
                throw new InvalidOperationException($"The state {this.currentState} was already registered in the machine.");
            }

            this.currentState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddState BeforeEachStep(Action action)
        {
            this.beforeAction = action;
            return this;
        }

        /// <inheritdoc/>
        public void Run()
        {
            var runningState = this.startingState;

            do
            {
                this.beforeAction?.Invoke();
                if (!this.stateMapper.ContainsKey(runningState))
                {
                    throw new InvalidOperationException($"The state {runningState} does not have a corresponding action.");
                }

                runningState = this.stateMapper[runningState]();
            }
            while (!this.stoppingStates.Contains(runningState));
        }

        /// <inheritdoc/>
        public ICanAddStop StartOn(State state)
        {
            this.startingState = state;
            return this;
        }

        /// <inheritdoc/>
        public ICanAddStateOrSteppingMethodOrStop StopOn(State state)
        {
            this.stoppingStates.Add(state);
            return this;
        }
    }
}