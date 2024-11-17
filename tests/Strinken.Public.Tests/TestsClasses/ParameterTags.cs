namespace Strinken.Public.Tests.TestsClasses;

internal sealed class BlueParameterTag : IParameterTag
{
    public string Description => "Blue";
    public string Name => "Blue";

    public string Resolve()
    {
        return "Blue";
    }
}

internal sealed class EmptyNameParameterTag : IParameterTag
{
    public string Description => string.Empty;
    public string Name => string.Empty;

    public string Resolve()
    {
        return string.Empty;
    }
}

internal sealed class InvalidNameParameterTag : IParameterTag
{
    public string Description => string.Empty;
    public string Name => "dollar$";

    public string Resolve()
    {
        return string.Empty;
    }
}

internal sealed class RedParameterTag : IParameterTag
{
    public string Description => "Red";
    public string Name => "Red";

    public string Resolve()
    {
        return "Red";
    }
}
