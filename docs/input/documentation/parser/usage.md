Description: How to use Strinken
Order: 1
---

The usage of Strinken is generally split into two parts: defining the tokens that will be available to a
parser and the creation of that parser.

# Token creation

Creation of a tag:

``` csharp
public class ExampleTag : ITag<ExampleClass>
{
    public string Description => "Example";
    public string Name => "Example";
    public string Resolve(ExampleClass value) => value.Name;
}
```

Creation of a parameter tag:

``` csharp
public class DateParameterTag : IParameterTag
{
    public string Description => "Date";
    public string Name => "Date";
    public string Resolve() => DateTime.Now.ToString();
}
```

Creation of a filter:

``` csharp
public class TrimFilter : IFilter
{
    public string Description => "Trims a string.";
    public string Name => "Trim";
    public string AlternativeName => null;
    public string Usage => "{tag:Trim}";
    public string Resolve(string value, string[] arguments) => value.Trim();
    public bool Validate(string[] arguments) => arguments == null || arguments.Length == 0;
}
```

# Parser creation

Creation of a parser:

``` csharp
var parser = new Parser<ExampleClass>();
```

## Adding tokens

There is three methods that can be used to add tokens to a parser:

- `AddTag`
- `AddParameterTag`
- `AddFilter`

Each method takes an instance of the corresponding token, and returns void (the addition is done to the caller).

It is also possible to use one of the extension methods defined on the Parser class. Each
of these extension methods returns a new Parser (so they can be chained) and does not modify the
caller.

### WithTag(s) extensions

These methods add one or more tag(s) to the parser.

You can either pass an existing tag, a list of existing tags or you can pass the tag parameters directly (if you
don't want to create a class only for a one time use)

### WithFilter(s) extensions

These methods add one or more filter(s) to the parser.

You can either pass an existing filter or a list of existing filters.

### WithParameterTag(s) extensions

These methods add one or more parameter tag(s) to the parser.

You can either pass an existing parameter tag or a list of existing parameter tags.

# String parsing

The string parsing in Strinken is split into three parts:

- The validation of the format of the string
- The validation of the content of the string (find if there is unknown tokens)
- The resolution of the string

Each of these part must parse the entire string in order to find errors or resolve each tokens. For that reason,
the validation of the content **is only done** by the validation process, the resolution does not verify that
all the tokens are known. The validation of the format is done by each process.

## String validation

Validation of a string:

``` csharp
parser.Validate("My name is {Example:Trim}.")
// returns a ValidationResult with IsValid = true
```

``` csharp
parser.Validate("My name is {Example:Trim+arg}.")
// returns a ValidationResult with IsValid = false and Message = "Lower does not have valid arguments."
```

## String resolution

### Classic resolution

The resolution of a string is done by passing to the `Resolve` method an instance of the class for
which the parser is designed.

``` csharp
var result = parser.Resolve("My name is {Example:Lower}.", new ExampleClass { Name = "Lorem" })
// returns "My name is lorem."
```

The resolution can also be done in bulk using the overload of `Resolve` that takes an `IEnumerable<T>` as values.

### Input compilation

An input can be compiled into a `CompiledString` in order to have a faster resolution time:

``` csharp
var compiledString = parser.Compile("My name is {Example:Lower}.")
```

The resulting `CompiledString` can then be passed to the `Resolve` method:

``` csharp
var result = parser.Resolve(compiledString, new ExampleClass { Name = "Lorem" })
// returns "My name is lorem."
```

Like the `Resolve` method that takes a `string` as input, bulk resolution is available with a `CompiledString`.