using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class EmptyNameTag : ITag<string>
    {
        public string Description => string.Empty;
        public string Name => string.Empty;

        public string Resolve(string value) => value;
    }
}