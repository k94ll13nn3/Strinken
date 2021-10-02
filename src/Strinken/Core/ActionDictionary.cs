namespace Strinken.Core;

/// <summary>
/// Action dictionary used by the token stack to resolve the string.
/// </summary>
internal class ActionDictionary
{
    /// <summary>
    /// Internal dictionary containing the list of actions and the related token propeties.
    /// </summary>
    private readonly IDictionary<(TokenType Type, char OperatorSymbol, char IndicatorSymbol), Func<string[], string>> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionDictionary"/> class.
    /// </summary>
    public ActionDictionary()
    {
        _items = new Dictionary<(TokenType Type, char OperatorSymbol, char IndicatorSymbol), Func<string[], string>>();
    }

    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    /// <param name="type">The type part of the key of the element to get or set.</param>
    /// <param name="operatorSymbol">The operator symbol part of the key  of the element to get or set.</param>
    /// <param name="indicatorSymbol">The indicator symbol part of the key  of the element to get or set.</param>
    /// <returns>The element with the specified key, or null if the key is not present.</returns>
    public Func<string[], string> this[TokenType type, char operatorSymbol, char indicatorSymbol]
    {
        get => Get((type, operatorSymbol, indicatorSymbol));
        set => _items[(type, operatorSymbol, indicatorSymbol)] = value;
    }

    /// <summary>
    /// Gets a value in the <see cref="ActionDictionary"/>.
    /// </summary>
    /// <param name="key">The key of the element to get or set.</param>
    /// <returns>The element with the specified key, or null if the key is not present.</returns>
    private Func<string[], string> Get((TokenType type, char operatorSymbol, char indicatorSymbol) key)
    {
        return _items.ContainsKey(key) ? _items[key] : (_ => string.Empty);
    }
}
