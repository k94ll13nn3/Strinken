using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class InvalidNameParameterTag : IParameterTag
    {
        public string Description => string.Empty;
        public string Name => "dollar$";

        public string Resolve() => string.Empty;
    }
}