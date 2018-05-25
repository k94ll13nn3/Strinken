// stylecop.header
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Strinken.Core;

namespace Strinken
{
    /// <summary>
    /// Strinken parser.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    public class Parser<T>
    {
        /// <summary>
        /// Filters used by the parser.
        /// </summary>
        private readonly IDictionary<string, IFilter> filters;

        /// <summary>
        /// Tags used by the parser.
        /// </summary>
        private readonly IDictionary<string, ITag<T>> tags;

        /// <summary>
        /// Parameter tags used by the parser.
        /// </summary>
        private readonly IDictionary<string, IParameterTag> parameterTags;

        /// <summary>
        /// Stack used when a string is compiled.
        /// </summary>
        private TokenStack compiledStack;

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
            tags = new Dictionary<string, ITag<T>>();
            parameterTags = new Dictionary<string, IParameterTag>();
            filters = new Dictionary<string, IFilter>();

            if (!ignoreBaseFilters)
            {
                foreach (IFilter filter in BaseFilters.RegisteredFilters)
                {
                    AddFilter(filter);
                }
            }
        }

        /// <summary>
        /// Gets the filters used by the parser.
        /// </summary>
        public IReadOnlyCollection<IFilter> Filters => new ReadOnlyCollection<IFilter>(filters.Values.ToList());

        /// <summary>
        /// Gets the tags used by the parser.
        /// </summary>
        public IReadOnlyCollection<ITag<T>> Tags => new ReadOnlyCollection<ITag<T>>(tags.Values.ToList());

        /// <summary>
        /// Gets the tags used by the parser.
        /// </summary>
        public IReadOnlyCollection<IParameterTag> ParameterTags => new ReadOnlyCollection<IParameterTag>(parameterTags.Values.ToList());

        /// <summary>
        /// Resolves the input.
        /// </summary>
        /// <param name="input">The input to resolve.</param>
        /// <param name="value">The value to pass to the tags.</param>
        /// <returns>The resolved input.</returns>
        /// <exception cref="FormatException">The input has a wrong format.</exception>
        public string Resolve(string input, T value) => Resolve(Compile(input), value);

        /// <summary>
        /// Resolves the compiled string.
        /// </summary>
        /// <param name="compiledString">The compiled string to resolve.</param>
        /// <param name="value">The value to pass to the tags.</param>
        /// <returns>The resolved compiled string.</returns>
        /// <exception cref="ArgumentNullException">The compiled string is null.</exception>
        public string Resolve(CompiledString compiledString, T value)
        {
            if (compiledString == null)
            {
                throw new ArgumentNullException(nameof(compiledString));
            }

            ActionDictionary actions = GenerateActionDictionaryForResolution(value);
            return compiledString.Stack.Resolve(actions);
        }

        /// <summary>
        /// Compiles a string for a faster resolution time but without any modification allowed after.
        /// </summary>
        /// <param name="input">The input to compile.</param>
        /// <exception cref="FormatException">The input has a wrong format.</exception>
        public CompiledString Compile(string input)
        {
            EngineResult runResult = StrinkenEngine.Run(input);
            if (runResult.Success)
            {
                return new CompiledString(runResult.Stack);
            }

            throw new FormatException(runResult.ErrorMessage);
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
                return new ValidationResult { Message = runResult.ErrorMessage, IsValid = false };
            }

            var tagList = new List<string>();
            var parameterTagList = new List<string>();
            var filterList = new List<Tuple<string, string[]>>();
            ActionDictionary actions = GenerateActionDictionaryForValidation(tagList, parameterTagList, filterList);

            runResult.Stack.Resolve(actions);

            return ValidateLists(tagList, parameterTagList, filterList);
        }

        /// <summary>
        /// Add a filter to the list of filter.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <exception cref="ArgumentException">The filter name is already present in the filter list.</exception>
        public void AddFilter(IFilter filter)
        {
            if (filters.ContainsKey(filter.Name))
            {
                throw new ArgumentException($"{filter.Name} was already registered in the filter list.");
            }

            filter.Name.ThrowIfInvalidName();

            if (!string.IsNullOrWhiteSpace(filter.AlternativeName))
            {
                if (filters.Values.Select(x => x.AlternativeName).Contains(filter.AlternativeName))
                {
                    throw new ArgumentException($"A filter already has {filter.AlternativeName} as its alternative name.");
                }

                filter.AlternativeName.ThrowIfInvalidAlternativeName();
            }

            filters.Add(filter.Name, filter);
        }

        /// <summary>
        /// Add a tag to the list of tags.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <exception cref="ArgumentException">The tag name is already present in the tag list.</exception>
        public void AddTag(ITag<T> tag)
        {
            if (tags.ContainsKey(tag.Name))
            {
                throw new ArgumentException($"{tag.Name} was already registered in the tag list.");
            }

            tag.Name.ThrowIfInvalidName();
            tags.Add(tag.Name, tag);
        }

        /// <summary>
        /// Add a parameter tag to the list of parameter tags.
        /// </summary>
        /// <param name="parameterTag">The parameter tag to add.</param>
        /// <exception cref="ArgumentException">The parameter tag name is already present in the parameter tag list.</exception>
        public void AddParameterTag(IParameterTag parameterTag)
        {
            if (parameterTags.ContainsKey(parameterTag.Name))
            {
                throw new ArgumentException($"{parameterTag.Name} was already registered in the parameter tag list.");
            }

            parameterTag.Name.ThrowIfInvalidName();
            parameterTags.Add(parameterTag.Name, parameterTag);
        }

        /// <summary>
        /// Creates a deep copy of the current parser.
        /// </summary>
        /// <returns>A deep copy of the parser.</returns>
        internal Parser<T> DeepCopy()
        {
            var newParser = new Parser<T>(true);
            foreach (ITag<T> tag in tags.Values)
            {
                newParser.AddTag(tag);
            }

            foreach (IParameterTag parameterTag in parameterTags.Values)
            {
                newParser.AddParameterTag(parameterTag);
            }

            foreach (IFilter filter in filters.Values)
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
        private static ActionDictionary GenerateActionDictionaryForValidation(List<string> tagList, List<string> parameterTagList, List<Tuple<string, string[]>> filterList)
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
                                filterList.Add(Tuple.Create(a[0], a.Skip(2).ToArray()));
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
        private ValidationResult ValidateLists(List<string> tagList, List<string> parameterTagList, List<Tuple<string, string[]>> filterList)
        {
            // Find the first tag that was not registered in the parser.
            foreach (var tag in tagList)
            {
                if (!tags.ContainsKey(tag))
                {
                    return new ValidationResult { Message = $"{tag} is not a valid tag.", IsValid = false };
                }
            }

            // Find the first parameter tag that was not registered in the parser.
            foreach (var parameterTag in parameterTagList)
            {
                if (!parameterTags.ContainsKey(parameterTag))
                {
                    return new ValidationResult { Message = $"{parameterTag} is not a valid parameter tag.", IsValid = false };
                }
            }

            // Find the first filter that was not registered in the parser.
            IEnumerable<string> alternativeNames = filters.Values.Select(x => x.AlternativeName).Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (var filter in filterList)
            {
                if (!filters.ContainsKey(filter.Item1) && !alternativeNames.Contains(filter.Item1))
                {
                    return new ValidationResult { Message = $"{filter.Item1} is not a valid filter.", IsValid = false };
                }
            }

            // Find the first filter that has invalid arguments.
            foreach (var filter in filterList)
            {
                if ((filters.ContainsKey(filter.Item1) && !filters[filter.Item1].Validate(filter.Item2))
                    || (!filters.ContainsKey(filter.Item1) && !filters.Values.FirstOrDefault(x => x.AlternativeName == filter.Item1).Validate(filter.Item2)))
                {
                    return new ValidationResult { Message = $"{filter.Item1} does not have valid arguments.", IsValid = false };
                }
            }

            return new ValidationResult { Message = null, IsValid = true };
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
                            actions[op.TokenType, op.Symbol, ind.Symbol] = a => tags[a[0]].Resolve(value);
                            break;

                        case ResolutionMethod.ParameterTag:
                            actions[op.TokenType, op.Symbol, ind.Symbol] = a => parameterTags[a[0]].Resolve();
                            break;

                        case ResolutionMethod.Filter:
                            actions[op.TokenType, op.Symbol, ind.Symbol] = a =>
                            {
                                if (filters.ContainsKey(a[0]))
                                {
                                    return filters[a[0]].Resolve(a[1], a.Skip(2).ToArray());
                                }
                                else
                                {
                                    return filters.SingleOrDefault(x => x.Value.AlternativeName == a[0]).Value.Resolve(a[1], a.Skip(2).ToArray());
                                }
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
}