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
            Func<StateMachine> createMachine = () => StateMachineBuilder
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
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OnTokenEndIndicator)
                .Build();

            Assert.That(() => machine.Run(), Throws.InvalidOperationException.With.Message.EqualTo("The state OnTokenEndIndicator does not have a corresponding action."));
        }

        [Test]
        public void Run_WithSinkState_ReturnsFailure()
        {
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OnTokenEndIndicator)
                .On(State.OnTokenEndIndicator).Sink()
                .Build();

            Assert.That(() => machine.Run(), Is.False);
        }

        [Test]
        public void Run_BaseMachine_ReturnsSuccess()
        {
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OutsideToken)
                .Build();

            Assert.That(() => machine.Run(), Is.True);
        }
    }
}