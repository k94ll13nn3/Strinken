namespace Strinken.Public.Tests.TestsClasses
{
    public class AppendFilter : IFilter
    {
        public string Description => "Append";

        public string Name => "Append";

        public string Usage => "";

        public string AlternativeName => "..";

        public string Resolve(string value, string[] arguments) => value + string.Concat(arguments);

        public bool Validate(string[] arguments) => arguments?.Length >= 1;
    }

    public class EmptyNameFilter : IFilter
    {
        public string Description => string.Empty;

        public string Name => string.Empty;

        public string Usage => string.Empty;

        public string AlternativeName => string.Empty;

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }

    public class InvalidNameFilter : IFilter
    {
        public string Description => string.Empty;

        public string Name => "name!";

        public string Usage => string.Empty;

        public string AlternativeName => string.Empty;

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }

    public class InvalidAlternativeNameFilter : IFilter
    {
        public string Description => string.Empty;

        public string Name => "name";

        public string Usage => string.Empty;

        public string AlternativeName => "??name";

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }

    public class SomeFilter : IFilter
    {
        public string Description => "Some";

        public string Name => "Some";

        public string Usage => "";

        public string AlternativeName => "!*";

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }

    public class SomeBisFilter : IFilter
    {
        public string Description => "SomeBis";

        public string Name => "SomeBis";

        public string Usage => "";

        public string AlternativeName => "!*";

        public string Resolve(string value, string[] arguments) => value;

        public bool Validate(string[] arguments) => true;
    }
}
