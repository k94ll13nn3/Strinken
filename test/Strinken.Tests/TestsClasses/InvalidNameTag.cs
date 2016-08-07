using Strinken.Parser;

namespace Strinken.Tests.TestsClasses
{
    public class InvalidNameTag : ITag<string>
    {
        public string Description => string.Empty;
        public string Name => "dollar$";

        public string Resolve(string value) => value;
    }
}