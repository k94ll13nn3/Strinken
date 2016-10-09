using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class EmptyNameParameterTag : IParameterTag
    {
        public string Description => string.Empty;
        public string Name => string.Empty;

        public string Resolve() => string.Empty;
    }
}