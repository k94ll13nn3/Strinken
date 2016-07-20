# Strinken

[![Build status](https://ci.appveyor.com/api/projects/status/038gqsusfw0srmst?svg=true)](https://ci.appveyor.com/project/k94ll13nn3/strinken)

A parametrized string library !

## Installation

Nuget `Install-Package Strinken` ([package](https://www.nuget.org/packages/Strinken/))

## Idea

In [Fun With Named Formats, String Parsing, and Edge Cases](http://haacked.com/archive/2009/01/04/fun-with-named-formats-string-parsing-and-edge-cases.aspx/), some String.Format modification were discussed, and one was named format methods where this was possible: `NamedFormat("{pi} first, {date} second", someObj);`. 
This library is inspired from the format but can only parse known tokens, which allows to not use reflection or DataBinder.

Parsing engine inspired from Henri's solution at [Named Formats Redux](http://haacked.com/archive/2009/01/14/named-formats-redux.aspx/)

## String format

### Tags

Use a tag: 
- `{tag}`

### Filters

Use a filter:
- `{tag:filter}`

Use arguments:
- `{tag:filter+arg}`
- `{tag:filter+arg,arg}`
- `{tag:filter+arg,=argTag}`

Chain filters:
- `{tag:filter:filter}`
- `{tag:filter+arg:filter}`
- `{tag:filter+arg,arg:filter+arg}`
- `{tag:filter:filter+=argTag}`

See [filters](doc/filters.md) for the list of base filters.

## Parser usage

Create a tag:
``` csharp
public class ExampleTag : ITag<ExampleClass>
{
    public string Description => "Example";
    public string Name => "Example";
    public string Resolve(ExampleClass value) => value.Name;
}
```

Create a filter (example taken from the base filters):
``` csharp
public class LowerFilter : IFilter
{
    public string Description => "Converts a string to lowercase.";
    public string Name => "Lower";
    public string Usage => "{tag:Lower}";
    public string Resolve(string value, string[] arguments) => value.ToLowerInvariant();
    public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
}
```

Create a parser:
``` csharp
var parser = ParserBuilder<ExampleClass>
                .Initialize()
                .WithTag(new ExampleTag())
                .Build();
// returns an IParser<ExampleClass>
``` 

Validate a string:
``` csharp
parser.Validate("My name is {Example:Lower}.")
// returns true
```

Parse a string:
``` csharp
var result = parser.Resolve("My name is {Example:Lower}.", new ExampleClass { Name = "Lorem" })
// returns "My name is lorem."
```
