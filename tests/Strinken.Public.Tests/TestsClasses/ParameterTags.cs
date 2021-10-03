namespace Strinken.Public.Tests.TestsClasses;

public class BlueParameterTag : IParameterTag
{
    public string Description => "Blue";
    public string Name => "Blue";

    public string Resolve()
    {
        return "Blue";
    }
}

public class EmptyNameParameterTag : IParameterTag
{
    public string Description => string.Empty;
    public string Name => string.Empty;

    public string Resolve()
    {
        return string.Empty;
    }
}

public class InvalidNameParameterTag : IParameterTag
{
    public string Description => string.Empty;
    public string Name => "dollar$";

    public string Resolve()
    {
        return string.Empty;
    }
}

public class RedParameterTag : IParameterTag
{
    public string Description => "Red";
    public string Name => "Red";

    public string Resolve()
    {
        return "Red";
    }
}
