using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class EmptyNameFilter : IFilter
    {
        public string Description => string.Empty;
        public string Name => string.Empty;
        public string Usage => string.Empty;

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }
}