# Strinken

[![NuGet](https://img.shields.io/nuget/v/Strinken.svg)](https://www.nuget.org/packages/Strinken/)
[![GitHub release](https://img.shields.io/github/release/k94ll13nn3/Strinken.svg)](https://github.com/k94ll13nn3/Strinken/releases/latest)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/k94ll13nn3/Strinken/master/LICENSE)
[![Coverage Status](https://coveralls.io/repos/github/k94ll13nn3/Strinken/badge.svg?branch=master)](https://coveralls.io/github/k94ll13nn3/Strinken?branch=master)

| Build server   | Platform     | Status                                                                                                                    |
|----------------|--------------|---------------------------------------------------------------------------------------------------------------------------|
| AppVeyor       | Windows/Linux      | [![AppVeyor](https://ci.appveyor.com/api/projects/status/038gqsusfw0srmst/branch/master?svg=true)](https://ci.appveyor.com/project/k94ll13nn3/strinken) |

A parametrized string library ! ([Documentation](https://k94ll13nn3.github.io/Strinken/))

## Installation

### NuGet

- Grab the latest package on [NuGet](https://www.nuget.org/packages/Strinken/).
- Install it via the Package Manager Console: `Install-Package Strinken`.

### Manual downloads

- Get the latest release on [GitHub](https://github.com/k94ll13nn3/Strinken/releases/latest).
- Get the latest CI build on [AppVeyor](https://ci.appveyor.com/project/k94ll13nn3/strinken) (Visual Studio 2017 image).

## Basic example

1. Create a class that implements `ITag<T>` for the wanted type (a class Person with a Name property for example):

    ``` csharp
    public class NameTag : ITag<Person>
    {
        public string Description => "Returns the name of a Person.";
        public string Name => "Name";
        public string Resolve(Person value) => value.Name.ToString();
    }
    ```

2. Create a `Parser<T>` with this tag:

    ``` csharp
    var parser = new Parser<Person>().WithTag(new NameTag());
    ```

3. Resolve a string with the parser:

    ``` csharp
    var result = parser.Resolve("My name is {Name}.", new Person { Name = "James" });
    // will return "My name is James."
    ```
