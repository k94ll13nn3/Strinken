// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Machine
{
    /// <summary>
    /// State machine.
    /// </summary>
    /// <typeparam name="TState">The type of the states.</typeparam>
    /// <typeparam name="TParameter">The type of the parameters passed to the machine.</typeparam>
    internal class StateMachine<TState, TParameter>
        where TParameter : IParameters<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine{TState, TParameter}"/> class.
        /// </summary>
        public StateMachine()
        {
            this.StateMapper = new Dictionary<TState, Func<TParameter, TState>>();
            this.StoppingStates = new List<TState>();
        }

        /// <summary>
        /// Gets or sets the action to perform before each state of the machine.
        /// </summary>
        public Action BeforeAction { get; set; }

        /// <summary>
        /// Gets or sets the current state added specified by the <see cref="ICanAddState{TState, TParameter}.On(TState)"/> method.
        /// </summary>
        public TState CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the starting state of the machine.
        /// </summary>
        public TState StartingState { get; set; }

        /// <summary>
        /// Gets the mapper used to resolve action depending on state.
        /// </summary>
        public Dictionary<TState, Func<TParameter, TState>> StateMapper { get; }

        /// <summary>
        /// Gets the list of states that stops the machine.
        /// </summary>
        public ICollection<TState> StoppingStates { get; }

        /// <summary>
        /// Run the machine.
        /// </summary>
        /// <param name="parameter">Parameter used by the state machine.</param>
        internal void Run(TParameter parameter)
        {
            var runningState = this.StartingState;

            do
            {
                this.BeforeAction?.Invoke();
                if (!this.StateMapper.ContainsKey(runningState))
                {
                    throw new InvalidOperationException($"The state {runningState} does not have a corresponding action.");
                }

                runningState = this.StateMapper[runningState](parameter);
            }
            while (!this.StoppingStates.Contains(runningState));
        }
    }
}