# Strinken

[![NuGet](https://img.shields.io/nuget/v/Strinken.svg)](https://www.nuget.org/packages/Strinken/)
[![GitHub release](https://img.shields.io/github/release/k94ll13nn3/Strinken.svg)](https://github.com/k94ll13nn3/Strinken/releases/latest)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/k94ll13nn3/Strinken/master/LICENSE)

| Build server   | Platform     | Status                                                                                                                    |
|----------------|--------------|---------------------------------------------------------------------------------------------------------------------------|
| AppVeyor       | Windows      | [![AppVeyor](https://ci.appveyor.com/api/projects/status/038gqsusfw0srmst/branch/master?svg=true)](https://ci.appveyor.com/project/k94ll13nn3/strinken) |
| Travis         | Linux        | [![Travis](https://travis-ci.org/k94ll13nn3/Strinken.svg?branch=master)](https://travis-ci.org/k94ll13nn3/Strinken) |

A parametrized string library !

## Installation

Nuget `Install-Package Strinken`

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

Create a parameter tag:

``` csharp
public class DateParameterTag : IParameterTag
{
    public string Description => "Date";
    public string Name => "Date";
    public string Resolve() => DateTime.Now.ToString();
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
var parser = new Parser<ExampleClass>()
                .WithTag(new ExampleTag())
                .WithFilter(new LowerFilter()) // for the example, but the LowerFilter is a base filter.
                .WithParameterTag(new DateParameterTag());
```

Validate a string:

``` csharp
parser.Validate("My name is {Example:Lower}.")
// returns a ValidationResult with IsValid = true
```

``` csharp
parser.Validate("My name is {Example:Lower+arg}.")
// returns a ValidationResult with IsValid = false and Message = "Lower does not have valid arguments."
```

Parse a string:

``` csharp
var result = parser.Resolve("My name is {Example:Lower}.", new ExampleClass { Name = "Lorem" })
// returns "My name is lorem."
```
