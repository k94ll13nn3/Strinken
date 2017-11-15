Title: Syntax
Description: Syntax of strings parsed by Strinken
Order: 2
---

Strinken is based around parsing a string that defines one or more series of [tokens](/Strinken/documentation/parser/tokens). 
Each series must be enclosed in curly braces (`{...}`) and inside, there tokens must follow a certain pattern:

- The first token must be a tag.
- Then, it can be followed by one or more filters, each starting with a `:`.
- For each filters, if it requires arguments, all the arguments must come before the next filter. To start the argument list, put a `+` after the filter, and separate arguments with a `,`.

Here are some examples:

- `{tag}` (the most basic series)
- `{tag:filter}`
- `{tag:filter+arg}`
- `{tag:filter+arg,arg}`
- `{tag:filter:filter}`
- `{tag:filter+arg:filter}`
- `{tag:filter:filter+arg}`
- `{tag:filter+arg,arg:filter+arg}`

Each token has some restriction on the characters it can contains (see below). None can contain a `}` because it would be considered 
as the end of a series of tokens. 

To use a raw `{` or `}` outside of a token, it must be doubled (`{{` or `}}`).

Here is the list of all the possibles tokens, in a syntax point of vue (for API and features, go to the 
[tokens](/Strinken/documentation/parser/tokens) page).

# Tags

Tags are the most basic tokens, and are the start of any series of tokens.
They are composed of one or more character from this list:

- Any Unicode letter (as defined [here](https://docs.microsoft.com/fr-fr/dotnet/api/system.char.isletter))
- A `_` or a `-`

Additionally, a tag can start with:

- a `!`, in this case, it is treated as a *parameter tag*.
- a `#`, in this case, it is treated as a *number tag*, i.e. a tag that only allow some formats of numbers. It returns directly the value. It is followed by a letter indicating the format:
  - `b` indicates a binary number, only allowing 0 and 1 (ex: `#b1101100`).
  - `o` indicates an octal number, only allowing digits from 0 to 7 (ex: `#o145701`).
  - `d` indicates a decimal number, only allowing digits from 0 to 9 (ex: `#d9857109`).
  - `x` indicates an hexadecimal number, only allowing digits from 0 to 9 and letters from a to f or A to F (ex: `#x125EFF12a`).

A tag can also start with `@`. In this case, there are no restrictions on characters except `}` (by global restriction)
and `:` as it is considered as the start of a filter.

`!`, `@`, `#b`, `#b`, `#b` and `#b` are not part of the tag name (in term of API), there are only indicators.

# Filters

Filters are similar to tags in term of syntax, but they always start with a `:`.
After that, they only allow these characters:

- Any Unicode letter (as defined [here](https://docs.microsoft.com/fr-fr/dotnet/api/system.char.isletter))
- A `_` or a `-`

`:` is not part of the filter name (in term of API), it is only an indicator.

# Arguments

Arguments are more complex, and they are separated in two categories: simple arguments and tag arguments.
Arguments can only follow a filter, the argument list must start with a `+`, and each argument must be separated by a `,`.

## Simple arguments

Simple arguments have little restriction on what they can contain. They can contain any character except characters from this list:

- `}` (by global restriction)
- `,` as it is considered as a separation between arguments
- `:` as it is considered as the start of a new filter
- `=` at the start, as it is the indication of a tag argument

## Tag arguments

Tag arguments are tags that are passed as arguments to a filter. They have the same syntax as normal tags, but must start with a `=`.