using System;
using FluentAssertions;
using Strinken.Machine;

namespace Strinken.Tests
{
    public class StateMachineTests
    {
        [StrinkenTest]
        public void On_StateAlreadyRegistered_ThrowsInvalidOperationException()
        {
            Func<StateMachine> createMachine = () => StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(null)
                .On(State.OutsideToken).Do(null)
                .Build();

            Action act = () => createMachine();

            act.ShouldThrow<InvalidOperationException>().WithMessage("The state OutsideToken was already registered in the machine.");
        }

        [StrinkenTest]
        public void On_StateWithNoAction_ThrowsInvalidOperationException()
        {
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OnTokenEndIndicator)
                .Build();

            Action act = () => machine.Run();

            act.ShouldThrow<InvalidOperationException>().WithMessage("The state OnTokenEndIndicator does not have a corresponding action.");
        }

        [StrinkenTest]
        public void Run_WithSinkState_ReturnsFailure()
        {
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OnTokenEndIndicator)
                .On(State.OnTokenEndIndicator).Sink()
                .Build();

            machine.Run().Should().BeFalse();
        }

        [StrinkenTest]
        public void Run_BaseMachine_ReturnsSuccess()
        {
            var machine = StateMachineBuilder
                .Initialize()
                .StartOn(State.OutsideToken)
                .StopOn(State.OutsideToken)
                .On(State.OutsideToken).Do(() => State.OutsideToken)
                .Build();

            machine.Run().Should().BeTrue();
        }
    }
}