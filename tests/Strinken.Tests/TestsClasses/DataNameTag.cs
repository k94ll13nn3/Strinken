namespace Strinken.Tests.TestsClasses;

internal sealed class DataNameTag : ITag<Data>
{
    public string Description => "DataName";
    public string Name => "DataName";

    public string Resolve(Data value)
    {
        return value.Name;
    }
}
