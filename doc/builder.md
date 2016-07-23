# Builder

In order to create a parser, you need to call the `ParserBuilder` class.

``` csharp
    public class ParserBuilder<T> : ...
    {
        static ICanAddTags<T> Initialize();
        Parser<T> Build();
        ICanAddTagsOrFilters<T> WithFilter(IFilter filter);
        ICanAddTagsOrFilters<T> WithFilters(IEnumerable<IFilter> filters);
        ICanAddTagsOrFilters<T> WithTag(ITag<T> tag);
        ICanAddTagsOrFilters<T> WithTag(string tagName, string tagDescription, Func<T, string> resolveAction);
        ICanAddTagsOrFilters<T> WithTags(IEnumerable<ITag<T>> tags);
    }
```

- The first method to call is `Initialize`
- After the initialization, you have to call one of the `WithTag(s)` methods.
- After, you can call as many of the five `With...` methods as you want.
- The last method to call is `Build`

An example can be seen on the README of the repository.

## WithTag(s)

These methods add one or more tag(s) to the parser.

You can either pass an existing
tag, a list of existing tags or you can pass the tag parameters directly (if you
don't want to create a class only for a one time use)

## WithFilter(s)

These methods add one or more filter(s) to the parser.

You can either pass an existing filter or a list of existing filters.