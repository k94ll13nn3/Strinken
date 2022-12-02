namespace Strinken.Public.Tests.TestsClasses;

internal sealed class DataNameTag : ITag<Data>
{
    public string Description => "DataName";
    public string Name => "DataName";

    public string Resolve(Data value)
    {
        return value.Name;
    }
}

internal sealed class EmptyNameTag : ITag<string>
{
    public string Description => string.Empty;
    public string Name => string.Empty;

    public string Resolve(string value)
    {
        return value;
    }
}

internal sealed class InvalidNameTag : ITag<string>
{
    public string Description => string.Empty;
    public string Name => "dollar$";

    public string Resolve(string value)
    {
        return value;
    }
}
