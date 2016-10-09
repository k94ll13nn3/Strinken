using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class MachineNameParameterTag : IParameterTag
    {
        public string Description => "MachineName";
        public string Name => "MachineName";

        public string Resolve() => System.Environment.MachineName;
    }
}