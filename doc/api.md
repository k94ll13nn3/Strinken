# API

## IToken

``` csharp
public interface IToken
{
    string Name { get; }
    string Description { get; }
}
```

## IFilter

``` csharp
public interface IFilter : IToken
{
    string Usage { get; }
    string Resolve(string value, string[] arguments);
    bool Validate(string[] arguments);
}
```

## ITag\<T>

``` csharp
public interface ITag<in T> : IToken
{
    string Resolve(T value);
}
```

## Parser\<T>

``` csharp
public class Parser<T>
{
    string Resolve(string input, T value);
    bool Validate(string input);
    IReadOnlyCollection<IFilter> Filters { get; }
    IReadOnlyCollection<ITag<T>> Tags { get; }
}
```