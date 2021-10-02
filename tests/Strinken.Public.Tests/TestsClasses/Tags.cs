namespace Strinken.Public.Tests.TestsClasses;

internal class DataNameTag : ITag<Data>
{
    public string Description => "DataName";
    public string Name => "DataName";

    public string Resolve(Data value) => value.Name;
}

internal class EmptyNameTag : ITag<string>
{
    public string Description => string.Empty;
    public string Name => string.Empty;

    public string Resolve(string value) => value;
}

internal class InvalidNameTag : ITag<string>
{
    public string Description => string.Empty;
    public string Name => "dollar$";

    public string Resolve(string value) => value;
}
