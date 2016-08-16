// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Machine
{
    /// <summary>
    /// State machine.
    /// </summary>
    internal class StateMachine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachine"/> class.
        /// </summary>
        public StateMachine()
        {
            this.StateMapper = new Dictionary<State, Func<State>>();
            this.SinkingStates = new List<State>();
            this.StoppingStates = new List<State>();
        }

        /// <summary>
        /// Gets or sets the action to perform before each state of the machine.
        /// </summary>
        public Action BeforeAction { get; set; }

        /// <summary>
        /// Gets or sets the current state added specified by the <see cref="ICanAddState.On(State)"/> method.
        /// </summary>
        public State CurrentState { get; set; }

        /// <summary>
        /// Gets or sets the starting state of the machine.
        /// </summary>
        public State StartingState { get; set; }

        /// <summary>
        /// Gets the mapper used to resolve action depending on state.
        /// </summary>
        public Dictionary<State, Func<State>> StateMapper { get; }

        /// <summary>
        /// Gets the list of states that stops the machine.
        /// </summary>
        public ICollection<State> StoppingStates { get; }

        /// <summary>
        /// Gets the list of states that stops the machine and returns a failure.
        /// </summary>
        public ICollection<State> SinkingStates { get; }

        /// <summary>
        /// Run the machine.
        /// </summary>
        /// <returns>A value indicating whether the machine successfully ran.</returns>
        internal bool Run()
        {
            var runningState = this.StartingState;

            do
            {
                this.BeforeAction?.Invoke();
                if (!this.StateMapper.ContainsKey(runningState))
                {
                    throw new InvalidOperationException($"The state {runningState} does not have a corresponding action.");
                }

                runningState = this.StateMapper[runningState]();

                // if the machine goes into a sink state, stop it and returns false (= failure)
                if (this.SinkingStates.Contains(runningState))
                {
                    return false;
                }
            }
            while (!this.StoppingStates.Contains(runningState));

            return true;
        }
    }
}