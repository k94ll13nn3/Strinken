Description: Tokens listing with their API
Order: 4
---

Almost each token in Strinken has its own API, except the simple arguments, as they are just plain string.
Each token class shares a common interface : [IToken](/Strinken/api/Strinken/IToken/). This interface is composed of two members:

- `Name`: represents the name of the token, as it should appear in the string to process.
- `Description`: a string describing the token. It has no use inside Strinken, but can be used for documentation or displayed in an gui listing tokens.

# Tag tokens

There are two tags token: a simple tag and a parameter tag. The difference between the two is that the simple tag depend on a additional value that 
is pass at the resolution time, where the parameter tag has a fixed value.

## Simple tag

The single tag interface is [ITag<T>](/Strinken/api/Strinken/ITag_1/). In addition to the **IToken** members, it has a `Resolve` method that takes 
a instance of *T* and returns the string to be rendered.

## Parameter tag

The single tag interface is [IParameterTag](/Strinken/api/Strinken/IParameterTag/). Like the simple tag; it has a `Resolve` method that 
returns the string to be rendered but it has no parameter.

# Filter tokens

For now, there is only one filter token. The filter interface is [IFilter](/Strinken/api/Strinken/IFilter/). In addition to the **IToken** members, it has:

- `Usage`: like the description, it has no use inside Strinken but can be used for documentation or displayed in an gui listing tokens.
- `AlternativeName`: an alternative name for the filter.
- `Validate`: a method that takes all the arguments passed to the filter, and returns a **bool** indicating whether they are valid. This is called by the content validation process of the parser.
- `Resolve`: a method that takes two arguments
  - a **string** corresponding to the value on which the filter is applied (a tag or the result of another filter).
  - a **string[]** corresponding to the arguments passed to the filter.

## Filter without arguments

It is possible to easily create filter that takes no arguments by inheriting the
[FilterWithoutArguments](/Strinken/api/Strinken/FilterWithoutArguments/) abstract class. This class has the following abstract members:

- `Name`
- `Description`
- `AlternativeName`
- `Resolve`: a method that takes one argument
  - a **string** corresponding to the value on which the filter is applied (a tag or the result of another filter).

The `Usage` is automaticaly computed as *\{tag:**filterName**\}*.