# Strinken

[![NuGet](https://img.shields.io/nuget/v/Strinken.svg?maxAge=2592000)](https://www.nuget.org/packages/Strinken/)
[![GitHub release](https://img.shields.io/github/release/k94ll13nn3/Strinken.svg?maxAge=2592000)](https://github.com/k94ll13nn3/Strinken/releases/latest)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/k94ll13nn3/Strinken/master/LICENSE)

| Build server   | Platform     | Status                                                                                                                    |
|----------------|--------------|---------------------------------------------------------------------------------------------------------------------------|
| AppVeyor       | Windows      | [![AppVeyor](https://ci.appveyor.com/api/projects/status/038gqsusfw0srmst?svg=true)](https://ci.appveyor.com/project/k94ll13nn3/strinken) |
| Travis         | Linux        | [![Travis](https://travis-ci.org/k94ll13nn3/Strinken.svg?branch=master)](https://travis-ci.org/k94ll13nn3/Strinken) |

A parametrized string library !

## Installation

Nuget `Install-Package Strinken`

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
// returns a Parser<ExampleClass>
```

Validate a string:

``` csharp
parser.Validate("My name is {Example:Lower}.")
// returns a ValidationResult with IsValid = true
```

Parse a string:

``` csharp
var result = parser.Resolve("My name is {Example:Lower}.", new ExampleClass { Name = "Lorem" })
// returns "My name is lorem."
```
