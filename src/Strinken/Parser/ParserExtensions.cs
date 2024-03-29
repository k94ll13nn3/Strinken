namespace Strinken;

/// <summary>
/// Extensions for the <see cref="Parser{T}"/> class.
/// </summary>
public static class ParserExtensions
{
    /// <summary>
    /// Add a filter to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="filter">The filter to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> is null.</exception>
    public static Parser<T> WithFilter<T>(this Parser<T> parser, IFilter filter)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));

        Parser<T> copiedParser = parser.DeepCopy();
        copiedParser.AddFilter(filter);
        return copiedParser;
    }

    /// <summary>
    /// Add filters to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="filters">The filters to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> or <paramref name="filters"/> are null.</exception>
    public static Parser<T> WithFilters<T>(this Parser<T> parser, IEnumerable<IFilter> filters)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));
        _ = filters ?? throw new ArgumentNullException(nameof(filters));

        Parser<T> copiedParser = parser.DeepCopy();
        foreach (IFilter filter in filters)
        {
            copiedParser.AddFilter(filter);
        }

        return copiedParser;
    }

    /// <summary>
    /// Add a tag to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="tag">The tag to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> is null.</exception>
    public static Parser<T> WithTag<T>(this Parser<T> parser, ITag<T> tag)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));

        Parser<T> copiedParser = parser.DeepCopy();
        copiedParser.AddTag(tag);
        return copiedParser;
    }

    /// <summary>
    /// Add a tag to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="tagName">The description of the tag.</param>
    /// <param name="tagDescription">The name of the tag.</param>
    /// <param name="resolveAction">The action linked to the tag.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> is null.</exception>
    public static Parser<T> WithTag<T>(this Parser<T> parser, string tagName, string tagDescription, Func<T, string> resolveAction)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));

        Parser<T> copiedParser = parser.DeepCopy();
        ITag<T> tag = TagFactory.Create(tagName, tagDescription, resolveAction);
        copiedParser.AddTag(tag);
        return copiedParser;
    }

    /// <summary>
    /// Add tags to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="tags">The tags to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> or <paramref name="tags"/> are null.</exception>
    public static Parser<T> WithTags<T>(this Parser<T> parser, IEnumerable<ITag<T>> tags)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));
        _ = tags ?? throw new ArgumentNullException(nameof(tags));

        Parser<T> copiedParser = parser.DeepCopy();
        foreach (ITag<T> tag in tags)
        {
            copiedParser.AddTag(tag);
        }

        return copiedParser;
    }

    /// <summary>
    /// Add a parameter tag to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="parameterTag">The parameter tag to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> is null.</exception>
    public static Parser<T> WithParameterTag<T>(this Parser<T> parser, IParameterTag parameterTag)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));

        Parser<T> copiedParser = parser.DeepCopy();
        copiedParser.AddParameterTag(parameterTag);
        return copiedParser;
    }

    /// <summary>
    /// Add parameter tags to the parser.
    /// </summary>
    /// <param name="parser">The parser on which the method is called.</param>
    /// <param name="parameterTags">The parameter tags to add.</param>
    /// <returns>A <see cref="Parser{T}"/> for chaining.</returns>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> or <paramref name="parameterTags"/> are null.</exception>
    public static Parser<T> WithParameterTags<T>(this Parser<T> parser, IEnumerable<IParameterTag> parameterTags)
    {
        _ = parser ?? throw new ArgumentNullException(nameof(parser));
        _ = parameterTags ?? throw new ArgumentNullException(nameof(parameterTags));

        Parser<T> copiedParser = parser.DeepCopy();
        foreach (IParameterTag parameterTag in parameterTags)
        {
            copiedParser.AddParameterTag(parameterTag);
        }

        return copiedParser;
    }
}
