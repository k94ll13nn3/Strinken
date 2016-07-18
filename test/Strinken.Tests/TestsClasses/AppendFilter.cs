using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class AppendFilter : IFilter
    {
        public string Description => "Append";

        public string Name => "Append";

        public string Usage => "";

        public string Resolve(string value, string[] arguments) => value + string.Join("", arguments);

        public bool Validate(string[] arguments) => arguments?.Length >= 1;
    }
}