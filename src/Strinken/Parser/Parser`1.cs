using System.Collections.ObjectModel;
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Strinken;

/// <summary>
/// Strinken parser.
/// </summary>
/// <typeparam name="T">The type related to the parser.</typeparam>
public sealed class Parser<T>
{
    /// <summary>
    /// Filters used by the parser.
    /// </summary>
    private readonly Dictionary<string, IFilter> _filters;

    /// <summary>
    /// Tags used by the parser.
    /// </summary>
    private readonly Dictionary<string, ITag<T>> _tags;

    /// <summary>
    /// Parameter tags used by the parser.
    /// </summary>
    private readonly Dictionary<string, IParameterTag> _parameterTags;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{T}"/> class.
    /// </summary>
    public Parser()
        : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser{T}"/> class.
    /// </summary>
    /// <param name="ignoreBaseFilters">A value indicating whether the base filters should be ignored.</param>
    public Parser(bool ignoreBaseFilters)
    {
        _tags = [];
        _parameterTags = [];
        _filters = [];

        if (!ignoreBaseFilters)
        {
            foreach (IFilter filter in BaseFilters.GetRegisteredFilters())
            {
                AddFilter(filter);
            }
        }
    }

    /// <summary>
    /// Gets the filters used by the parser.
    /// </summary>
#if NET6_0_OR_GREATER
    [SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Won't change to avoid breaking change")]
#endif
    public IReadOnlyCollection<IFilter> GetFilters()
    {
        return new ReadOnlyCollection<IFilter>([.. _filters.Values]);
    }

    /// <summary>
    /// Gets the tags used by the parser.
    /// </summary>
#if NET6_0_OR_GREATER
    [SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Won't change to avoid breaking change")]
#endif
    public IReadOnlyCollection<ITag<T>> GetTags()
    {
        return new ReadOnlyCollection<ITag<T>>([.. _tags.Values]);
    }

    /// <summary>
    /// Gets the parameters tags used by the parser.
    /// </summary>
#if NET6_0_OR_GREATER
    [SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "Won't change to avoid breaking change")]
#endif
    public IReadOnlyCollection<IParameterTag> GetParameterTags()
    {
        return new ReadOnlyCollection<IParameterTag>([.. _parameterTags.Values]);
    }

    /// <summary>
    /// Resolves the input.
    /// </summary>
    /// <param name="input">The input to resolve.</param>
    /// <param name="value">The value to pass to the tags.</param>
    /// <returns>The resolved input.</returns>
    /// <exception cref="FormatException">The input has a wrong format.</exception>
    public string Resolve(string input, T value)
    {
        return Resolve(Compile(input), value);
    }

    /// <summary>
    /// Resolves the compiled string.
    /// </summary>
    /// <param name="compiledString">The compiled string to resolve.</param>
    /// <param name="value">The value to pass to the tags.</param>
    /// <returns>The resolved compiled string.</returns>
    /// <exception cref="ArgumentNullException">The compiled string is null.</exception>
    public string Resolve(CompiledString compiledString, T value)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(compiledString);
#else
        if (compiledString == null)
        {
            throw new ArgumentNullException(nameof(compiledString));
        }
#endif

        ActionDictionary actions = GenerateActionDictionaryForResolution(value);
        return compiledString.Stack.Resolve(actions);
    }

    /// <summary>
    /// Resolves the input.
    /// </summary>
    /// <param name="input">The input to resolve.</param>
    /// <param name="values">The values to pass to the tags.</param>
    /// <returns>The resolved inputs.</returns>
    /// <exception cref="FormatException">The input has a wrong format.</exception>
    /// <exception cref="ArgumentNullException">Values is null.</exception>
    public IEnumerable<string> Resolve(string input, IEnumerable<T> values)
    {
        _ = values ?? throw new ArgumentNullException(nameof(values));

        return ResolveInternal(Compile(input));
        IEnumerable<string> ResolveInternal(CompiledString compiledString)
        {
            foreach (T value in values)
            {
                yield return Resolve(compiledString, value);
            }
        }
    }

    /// <summary>
    /// Resolves the compiled string.
    /// </summary>
    /// <param name="compiledString">The compiled string to resolve.</param>
    /// <param name="values">The values to pass to the tags.</param>
    /// <returns>The resolved compiled strings.</returns>
    /// <exception cref="ArgumentNullException">The compiled string is null.</exception>
    /// <exception cref="ArgumentNullException">Values is null.</exception>
    public IEnumerable<string> Resolve(CompiledString compiledString, IEnumerable<T> values)
    {
        _ = values ?? throw new ArgumentNullException(nameof(values));
        _ = compiledString ?? throw new ArgumentNullException(nameof(compiledString));

        return ResolveInternal();
        IEnumerable<string> ResolveInternal()
        {
            foreach (T value in values)
            {
                yield return Resolve(compiledString, value);
            }
        }
    }

    /// <summary>
    /// Compiles a string for a faster resolution time but without any modification allowed after.
    /// </summary>
    /// <param name="input">The input to compile.</param>
    /// <exception cref="FormatException">The input has a wrong format.</exception>
    public CompiledString Compile(string input)
    {
        EngineResult runResult = StrinkenEngine.Run(input);
        return runResult.Success
            ? new CompiledString(runResult.Stack)
            : throw new FormatException(runResult.ErrorMessage);
    }

    /// <summary>
    /// Validates an input.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>A value indicating whether the input is valid.</returns>
    public ValidationResult Validate(string input)
    {
        EngineResult runResult = StrinkenEngine.Run(input);
        if (!runResult.Success)
        {
            return new ValidationResult(runResult.ErrorMessage, false);
        }

        var tagList = new List<string>();
        var parameterTagList = new List<string>();
        var filterList = new List<(string name, string[] arguments)>();
        ActionDictionary actions = GenerateActionDictionaryForValidation(tagList, parameterTagList, filterList);

        runResult.Stack.Resolve(actions);

        return ValidateLists(tagList, parameterTagList, filterList);
    }

    /// <summary>
    /// Add a filter to the list of filter.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    /// <exception cref="ArgumentException">The filter name is already present in the filter list.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filter"/> is null.</exception>
    public void AddFilter(IFilter filter)
    {
        _ = filter ?? throw new ArgumentNullException(nameof(filter));

        if (_filters.ContainsKey(filter.Name))
        {
            throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
        }

        filter.Name.ThrowIfInvalidName();

        if (!string.IsNullOrWhiteSpace(filter.AlternativeName))
        {
            if (_filters.Values.Select(x => x.AlternativeName).Contains(filter.AlternativeName))
            {
                throw new ArgumentException($"A filter already has {filter.AlternativeName} as its alternative name.");
            }

            filter.AlternativeName.ThrowIfInvalidAlternativeName();
        }

        _filters.Add(filter.Name, filter);
    }

    /// <summary>
    /// Add a tag to the list of tags.
    /// </summary>
    /// <param name="tag">The tag to add.</param>
    /// <exception cref="ArgumentException">The tag name is already present in the tag list.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is null.</exception>
    public void AddTag(ITag<T> tag)
    {
        _ = tag ?? throw new ArgumentNullException(nameof(tag));

        if (_tags.ContainsKey(tag.Name))
        {
            throw new ArgumentException($"{tag.Name} was already registered in the tag list.");
        }

        tag.Name.ThrowIfInvalidName();
        _tags.Add(tag.Name, tag);
    }

    /// <summary>
    /// Add a parameter tag to the list of parameter tags.
    /// </summary>
    /// <param name="parameterTag">The parameter tag to add.</param>
    /// <exception cref="ArgumentException">The parameter tag name is already present in the parameter tag list.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTag"/> is null.</exception>
    public void AddParameterTag(IParameterTag parameterTag)
    {
        _ = parameterTag ?? throw new ArgumentNullException(nameof(parameterTag));

        if (_parameterTags.ContainsKey(parameterTag.Name))
        {
            throw new ArgumentException($"{parameterTag.Name} was already registered in the parameter tag list.");
        }

        parameterTag.Name.ThrowIfInvalidName();
        _parameterTags.Add(parameterTag.Name, parameterTag);
    }

    /// <summary>
    /// Creates a deep copy of the current parser.
    /// </summary>
    /// <returns>A deep copy of the parser.</returns>
    internal Parser<T> DeepCopy()
    {
        var newParser = new Parser<T>(true);
        foreach (ITag<T> tag in _tags.Values)
        {
            newParser.AddTag(tag);
        }

        foreach (IParameterTag parameterTag in _parameterTags.Values)
        {
            newParser.AddParameterTag(parameterTag);
        }

        foreach (IFilter filter in _filters.Values)
        {
            newParser.AddFilter(filter);
        }

        return newParser;
    }

    /// <summary>
    /// Generates the <see cref="ActionDictionary"/> used for string validation.
    /// </summary>
    /// <param name="tagList">The tags to validate.</param>
    /// <param name="parameterTagList">The parameter tags to validate.</param>
    /// <param name="filterList">The filters to validate.</param>
    /// <returns>An <see cref="ActionDictionary"/>.</returns>
    private static ActionDictionary GenerateActionDictionaryForValidation(List<string> tagList, List<string> parameterTagList, List<(string name, string[] arguments)> filterList)
    {
        var actions = new ActionDictionary();
        foreach (Operator op in BaseOperators.RegisteredOperators)
        {
            foreach (Indicator ind in op.Indicators)
            {
                switch (ind.ResolutionMethod)
                {
                    case ResolutionMethod.Tag:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a =>
                        {
                            tagList.Add(a[0]);
                            return string.Empty;
                        };
                        break;

                    case ResolutionMethod.ParameterTag:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a =>
                        {
                            parameterTagList.Add(a[0]);
                            return string.Empty;
                        };
                        break;

                    case ResolutionMethod.Filter:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a =>
                        {
                            filterList.Add((a[0], a.Skip(2).ToArray()));
                            return string.Empty;
                        };
                        break;

                    case ResolutionMethod.Name:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a => a[0];
                        break;
                }
            }
        }

        return actions;
    }

    /// <summary>
    /// Validates lists of tags, parameter tags and filters.
    /// </summary>
    /// <param name="tagList">The tags to validate.</param>
    /// <param name="parameterTagList">The parameter tags to validate.</param>
    /// <param name="filterList">The filters to validate.</param>
    /// <returns>A value indicating whether the input is valid.</returns>
    private ValidationResult ValidateLists(List<string> tagList, List<string> parameterTagList, List<(string name, string[] arguments)> filterList)
    {
        // Find the first tag that was not registered in the parser.
        foreach (string tag in tagList)
        {
            if (!_tags.ContainsKey(tag))
            {
                return new ValidationResult($"{tag} is not a valid tag.", false);
            }
        }

        // Find the first parameter tag that was not registered in the parser.
        foreach (string parameterTag in parameterTagList)
        {
            if (!_parameterTags.ContainsKey(parameterTag))
            {
                return new ValidationResult($"{parameterTag} is not a valid parameter tag.", false);
            }
        }

        // Find the first filter that was not registered in the parser.
        List<string> alternativeNames = [.. _filters.Values.Select(x => x.AlternativeName).Where(x => !string.IsNullOrWhiteSpace(x))];
        foreach ((string name, _) in filterList)
        {
            if (!_filters.ContainsKey(name) && !alternativeNames.Contains(name))
            {
                return new ValidationResult($"{name} is not a valid filter.", false);
            }
        }

        // Find the first filter that has invalid arguments.
        foreach ((string name, string[] arguments) in filterList)
        {
            if ((_filters.ContainsKey(name) && !_filters[name].Validate(arguments))
                || (!_filters.ContainsKey(name) && !_filters.Values.First(x => x.AlternativeName == name).Validate(arguments)))
            {
                return new ValidationResult($"{name} does not have valid arguments.", false);
            }
        }

        return new ValidationResult(string.Empty, true);
    }

    /// <summary>
    /// Generates the <see cref="ActionDictionary"/> used for string resolution.
    /// </summary>
    /// <param name="value">The value passed for resolution.</param>
    /// <returns>An <see cref="ActionDictionary"/>.</returns>
    private ActionDictionary GenerateActionDictionaryForResolution(T value)
    {
        var actions = new ActionDictionary();
        foreach (Operator op in BaseOperators.RegisteredOperators)
        {
            foreach (Indicator ind in op.Indicators)
            {
                switch (ind.ResolutionMethod)
                {
                    case ResolutionMethod.Tag:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a => _tags[a[0]].Resolve(value);
                        break;

                    case ResolutionMethod.ParameterTag:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a => _parameterTags[a[0]].Resolve();
                        break;

                    case ResolutionMethod.Filter:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a =>
                        {
                            return _filters.TryGetValue(a[0], out IFilter? value)
                                ? value.Resolve(a[1], a.Skip(2).ToArray())
                                : _filters.SingleOrDefault(x => x.Value.AlternativeName == a[0]).Value.Resolve(a[1], a.Skip(2).ToArray());
                        };
                        break;

                    case ResolutionMethod.Name:
                        actions[op.TokenType, op.Symbol, ind.Symbol] = a => a[0];
                        break;
                }
            }
        }

        return actions;
    }
}
