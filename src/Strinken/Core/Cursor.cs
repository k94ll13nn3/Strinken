namespace Strinken.Core;

/// <summary>
/// Cursor used to read a string.
/// </summary>
internal sealed class Cursor : IDisposable
{
    /// <summary>
    /// The reader used to read the string.
    /// </summary>
    private readonly StringReader _reader;

    /// <summary>
    /// Gets the current position of the cursor.
    /// </summary>
    private uint _position;

    /// <summary>
    /// Gets the current value of the cursor.
    /// </summary>
    private int _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cursor"/> class.
    /// </summary>
    /// <param name="input">The string to read.</param>
    public Cursor(string input)
    {
        _reader = new StringReader(input);
        _value = _reader.Read();
        _position = 0;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _reader?.Dispose();
    }

    /// <summary>
    /// Parses a string inside a token and returns the first name in it.
    /// </summary>
    /// <param name="ends">A list of valid ends.</param>
    /// <param name="tokenType">The type of the token to parse.</param>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<TokenDefinition> ParseName(ICollection<int> ends, TokenType tokenType)
    {
        var updatedEnd = new List<int> { SpecialCharacter.TokenEndIndicator };
        foreach (int end in ends ?? Enumerable.Empty<int>())
        {
            updatedEnd.Add(end);
        }

        Operator operatorDefined = BaseOperators.RegisteredOperators.FirstOrDefault(x => x.Symbol == GetValueAsChar() && x.TokenType == tokenType);
        if (operatorDefined is not null)
        {
            Next();
        }
        else
        {
            operatorDefined = BaseOperators.RegisteredOperators.Single(x => x.Symbol == '\0' && x.TokenType == tokenType);
        }

        Indicator indicatorDefined = operatorDefined.Indicators.FirstOrDefault(x => x.Symbol == GetValueAsChar());
        if (indicatorDefined is not null)
        {
            Next();
        }
        else
        {
            indicatorDefined = operatorDefined.Indicators.Single(x => x.Symbol == '\0');
        }

        return ParseNameInternal(tokenType, updatedEnd, operatorDefined, indicatorDefined);
    }

    /// <summary>
    /// Parses an argument.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<TokenDefinition> ParseArgument()
    {
        ParseResult<TokenDefinition> result = ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentSeparator }, TokenType.Argument);
        if (result)
        {
            return result;
        }

        return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyArgument);
    }

    /// <summary>
    /// Parses a filter.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<TokenDefinition> ParseFilter()
    {
        ParseResult<TokenDefinition> result = ParseName(new[] { SpecialCharacter.FilterSeparator, SpecialCharacter.ArgumentIndicator }, TokenType.Filter);
        if (result)
        {
            return result;
        }

        return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyFilter);
    }

    /// <summary>
    /// Parses a tag.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<TokenDefinition> ParseTag()
    {
        ParseResult<TokenDefinition> result = ParseName(new[] { SpecialCharacter.FilterSeparator }, TokenType.Tag);
        if (result)
        {
            return result;
        }

        return ParseResult<TokenDefinition>.FailureWithMessage(result.Message != Errors.EmptyName ? result.Message : Errors.EmptyTag);
    }

    /// <summary>
    /// Parses a filter and its possible arguments.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<IEnumerable<TokenDefinition>> ParseFilterAndArguments()
    {
        var tokenList = new List<TokenDefinition>();
        ParseResult<TokenDefinition> filterParseResult = ParseFilter();
        if (!filterParseResult)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(filterParseResult.Message);
        }

        tokenList.Add(filterParseResult.Value);
        while (_value != SpecialCharacter.FilterSeparator && _value != SpecialCharacter.TokenEndIndicator && !HasEnded())
        {
            if (_value != SpecialCharacter.ArgumentIndicator && _value != SpecialCharacter.ArgumentSeparator)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));
            }

            Next();
            ParseResult<TokenDefinition> argumentParseResult = ParseArgument();
            if (!argumentParseResult)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(argumentParseResult.Message);
            }

            tokenList.Add(argumentParseResult.Value);
        }

        return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
    }

    /// <summary>
    /// Parses a token.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<IEnumerable<TokenDefinition>> ParseToken()
    {
        var tokenList = new List<TokenDefinition>();
        ParseResult<TokenDefinition> tagParseResult = ParseTag();
        if (!tagParseResult)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tagParseResult.Message);
        }

        tokenList.Add(tagParseResult.Value);
        while (!HasEnded() && _value != SpecialCharacter.TokenEndIndicator)
        {
            if (_value != SpecialCharacter.FilterSeparator)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));
            }

            Next();
            ParseResult<IEnumerable<TokenDefinition>> filterAndArgumentsParseResult = ParseFilterAndArguments();
            if (filterAndArgumentsParseResult)
            {
                tokenList.AddRange(filterAndArgumentsParseResult.Value ?? Enumerable.Empty<TokenDefinition>());
            }
            else
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(filterAndArgumentsParseResult.Message);
            }
        }

        return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
    }

    /// <summary>
    /// Parses an outside string.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<TokenDefinition> ParseOutsideString()
    {
        var builder = new StringBuilder();
        while (true)
        {
            switch (_value)
            {
                // Escaped indicator
                case SpecialCharacter.TokenStartIndicator when Peek() == SpecialCharacter.TokenStartIndicator:
                case SpecialCharacter.TokenEndIndicator when Peek() == SpecialCharacter.TokenEndIndicator:
                    Next();
                    break;

                // Start of token or end of string
                case SpecialCharacter.TokenStartIndicator:
                case int _ when HasEnded():
                    return ParseResult<TokenDefinition>.Success(new TokenDefinition(builder.ToString(), TokenType.None, '\0', '\0'));

                case SpecialCharacter.TokenEndIndicator when PeekIsEnd():
                    return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacterAtStringEnd, GetValueAsChar()));

                case SpecialCharacter.TokenEndIndicator:
                    return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));
            }

            builder.Append(GetValueAsChar());
            Next();
        }
    }

    /// <summary>
    /// Parses a token and the following outside string.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<IEnumerable<TokenDefinition>> ParseTokenAndOutsideString()
    {
        var tokenList = new List<TokenDefinition>();
        ParseResult<IEnumerable<TokenDefinition>> tokenParseResult = ParseToken();
        if (!tokenParseResult)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tokenParseResult.Message);
        }

        tokenList.AddRange(tokenParseResult.Value ?? Enumerable.Empty<TokenDefinition>());
        if (_value != SpecialCharacter.TokenEndIndicator)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));
        }

        Next();
        ParseResult<TokenDefinition> outsideParseResult = ParseOutsideString();
        if (!outsideParseResult)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(outsideParseResult.Message);
        }

        tokenList.Add(outsideParseResult.Value);
        return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
    }

    /// <summary>
    /// Parses a string.
    /// </summary>
    /// <returns>The result of the parsing.</returns>
    public ParseResult<IEnumerable<TokenDefinition>> ParseString()
    {
        var tokenList = new List<TokenDefinition>();
        ParseResult<TokenDefinition> outsideParseResult = ParseOutsideString();
        if (!outsideParseResult)
        {
            return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(outsideParseResult.Message);
        }

        tokenList.Add(outsideParseResult.Value);
        while (!HasEnded())
        {
            if (_value != SpecialCharacter.TokenStartIndicator)
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));
            }

            Next();
            ParseResult<IEnumerable<TokenDefinition>> tokenParseResult = ParseTokenAndOutsideString();
            if (tokenParseResult)
            {
                foreach (TokenDefinition token in tokenParseResult.Value)
                {
                    tokenList.Add(token);
                }
            }
            else
            {
                return ParseResult<IEnumerable<TokenDefinition>>.FailureWithMessage(tokenParseResult.Message);
            }
        }

        return ParseResult<IEnumerable<TokenDefinition>>.Success(tokenList);
    }

    /// <summary>
    /// Gets the current value of the cursor as a <see cref="char"/>.
    /// </summary>
    private char GetValueAsChar()
    {
        return (char)_value;
    }

    /// <summary>
    /// Indicates if the cursor has reached the end.
    /// </summary>
    /// <returns>A value indicating whether the cursor as reached the end.</returns>
    private bool HasEnded()
    {
        return _value == -1;
    }

    /// <summary>
    /// Moves the cursor.
    /// </summary>
    private void Next()
    {
        _value = _reader.Read();
        _position++;
    }

    /// <summary>
    /// Peeks the next character of the cursor.
    /// </summary>
    /// <returns>The next character of the cursor.</returns>
    private int Peek()
    {
        return _reader.Peek();
    }

    /// <summary>
    /// Indicates if the next character is the end.
    /// </summary>
    /// <returns>A value indicating whether the next character is the end.</returns>
    private bool PeekIsEnd()
    {
        return Peek() == -1;
    }

    /// <summary>
    /// Parses a string inside a token and returns the first name in it.
    /// </summary>
    /// <param name="tokenType">The type of the token to parse.</param>
    /// <param name="updatedEnd">A list of valid ends.</param>
    /// <param name="operatorDefined">The operator defined.</param>
    /// <param name="indicatorDefined">The indicator defined.</param>
    /// <returns>The result of the parsing.</returns>
    private ParseResult<TokenDefinition> ParseNameInternal(TokenType tokenType, List<int> updatedEnd, Operator operatorDefined, Indicator indicatorDefined)
    {
        bool? isSymbolContext = null;
        var builder = new StringBuilder();
        while (true)
        {
            switch (_value)
            {
                case int _ when updatedEnd.Contains(_value):
                    string parsedName = builder.ToString();
                    return !string.IsNullOrEmpty(parsedName) ?
                        ParseResult<TokenDefinition>.Success(new TokenDefinition(parsedName, tokenType, operatorDefined.Symbol, indicatorDefined.Symbol)) :
                        ParseResult<TokenDefinition>.FailureWithMessage(Errors.EmptyName);

                case int _ when HasEnded():
                    return ParseResult<TokenDefinition>.FailureWithMessage(Errors.EndOfString);

                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Name && GetValueAsChar().IsInvalidTokenNameCharacter():
                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Binary && GetValueAsChar() != '0' && GetValueAsChar() != '1':
                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Octal && (GetValueAsChar() < '0' || GetValueAsChar() > '7'):
                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Decimal && (GetValueAsChar() < '0' || GetValueAsChar() > '9'):
                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.Hexadecimal && GetValueAsChar().IsInvalidHexadecimalCharacter():
                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.NameOrSymbol
                    && ((isSymbolContext == false && GetValueAsChar().IsInvalidTokenNameCharacter()) || (isSymbolContext == true && GetValueAsChar().IsInvalidAlternativeNameCharacter())):
                    return ParseResult<TokenDefinition>.FailureWithMessage(string.Format(CultureInfo.InvariantCulture, Errors.IllegalCharacter, GetValueAsChar(), _position));

                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.NameOrSymbol && isSymbolContext == null && !GetValueAsChar().IsInvalidTokenNameCharacter():
                    isSymbolContext = false;
                    break;

                case int _ when indicatorDefined.ParsingMethod == ParsingMethod.NameOrSymbol && isSymbolContext == null && !GetValueAsChar().IsInvalidAlternativeNameCharacter():
                    isSymbolContext = true;
                    break;
            }

            builder.Append(GetValueAsChar());
            Next();
        }
    }
}
