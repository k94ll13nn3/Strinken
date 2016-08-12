using NUnit.Framework;
using Strinken.Machine;
using System;

namespace Strinken.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        [Test]
        public void On_StateAlreadyRegistered_ThrowsInvalidOperationException()
        {
            Func<StateMachine<State, BaseParameters>> createMachine = () => StateMachineBuilder<State, BaseParameters>
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(null)
                .On(State.OutsideToken).Do(null)
                .Build();

            Assert.That(() => createMachine(), Throws.InvalidOperationException.With.Message.EqualTo("The state OutsideToken was already registered in the machine."));
        }

        [Test]
        public void On_StateWithNoAction_ThrowsInvalidOperationException()
        {
            var machine = StateMachineBuilder<State, BaseParameters>
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(p => State.InsideArgument)
                .Build();

            Assert.That(() => machine.Run(new BaseParameters()), Throws.InvalidOperationException.With.Message.EqualTo("The state InsideArgument does not have a corresponding action."));
        }

        private class BaseParameters : IParameters<State>
        {
            public string Message { get; set; }
            public State CurrentState { get; set; }
        }
    }
}