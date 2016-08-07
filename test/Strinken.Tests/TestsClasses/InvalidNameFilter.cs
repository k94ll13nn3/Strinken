using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class InvalidNameFilter : IFilter
    {
        public string Description => string.Empty;
        public string Name => "name!";
        public string Usage => string.Empty;

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }
}