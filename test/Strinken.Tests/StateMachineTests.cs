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
                .On(State.OutsideToken).Do(() => State.InsideArgument)
                .Build();

            Assert.That(() => machine.Run(), Throws.InvalidOperationException.With.Message.EqualTo("The state InsideArgument does not have a corresponding action."));
        }
    }
}