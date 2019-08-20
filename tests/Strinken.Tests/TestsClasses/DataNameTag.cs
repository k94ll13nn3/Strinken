namespace Strinken.Tests.TestsClasses
{
    public class DataNameTag : ITag<Data>
    {
        public string Description => "DataName";
        public string Name => "DataName";

        public string Resolve(Data value) => value.Name;
    }
}
