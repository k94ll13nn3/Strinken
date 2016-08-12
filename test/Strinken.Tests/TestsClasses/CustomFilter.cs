using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class CustomFilter : IFilter
    {
        public string Description => "Custom";

        public string Name => "Custom";

        public string Usage => "";

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }
}