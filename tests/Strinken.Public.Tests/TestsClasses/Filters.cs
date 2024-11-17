namespace Strinken.Public.Tests.TestsClasses;

internal sealed class AppendFilter : IFilter
{
    public string Description => "Append";

    public string Name => "Append";

    public string Usage => "";

    public string AlternativeName => "..";

    public string Resolve(string value, string[] arguments)
    {
        return value + string.Concat(arguments);
    }

    public bool Validate(string[] arguments)
    {
        return arguments?.Length >= 1;
    }
}

internal sealed class EmptyNameFilter : IFilter
{
    public string Description => string.Empty;

    public string Name => string.Empty;

    public string Usage => string.Empty;

    public string AlternativeName => string.Empty;

    public string Resolve(string value, string[] arguments)
    {
        return value;
    }

    public bool Validate(string[] arguments)
    {
        return true;
    }
}

internal sealed class InvalidNameFilter : IFilter
{
    public string Description => string.Empty;

    public string Name => "name!";

    public string Usage => string.Empty;

    public string AlternativeName => string.Empty;

    public string Resolve(string value, string[] arguments)
    {
        return value;
    }

    public bool Validate(string[] arguments)
    {
        return true;
    }
}

internal sealed class InvalidAlternativeNameFilter : IFilter
{
    public string Description => string.Empty;

    public string Name => "name";

    public string Usage => string.Empty;

    public string AlternativeName => "??name";

    public string Resolve(string value, string[] arguments)
    {
        return value;
    }

    public bool Validate(string[] arguments)
    {
        return true;
    }
}

internal sealed class SomeFilter : IFilter
{
    public string Description => "Some";

    public string Name => "Some";

    public string Usage => "";

    public string AlternativeName => "!*";

    public string Resolve(string value, string[] arguments)
    {
        return value;
    }

    public bool Validate(string[] arguments)
    {
        return true;
    }
}

internal sealed class SomeBisFilter : IFilter
{
    public string Description => "SomeBis";

    public string Name => "SomeBis";

    public string Usage => "";

    public string AlternativeName => "!*";

    public string Resolve(string value, string[] arguments)
    {
        return value;
    }

    public bool Validate(string[] arguments)
    {
        return true;
    }
}
