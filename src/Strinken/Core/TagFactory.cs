namespace Strinken.Core;

/// <summary>
/// Factory that creates tags.
/// </summary>
internal static class TagFactory
{
    /// <summary>
    /// Create a new tag.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    /// <param name="tagName">The description of the tag.</param>
    /// <param name="tagDescription">The name of the tag.</param>
    /// <param name="resolveAction">The action linked to the tag.</param>
    /// <returns>The tag.</returns>
    internal static ITag<T> Create<T>(string tagName, string tagDescription, Func<T, string> resolveAction)
    {
        return new BaseTag<T>(tagName, tagDescription, resolveAction);
    }

    /// <summary>
    /// Private class used to create tags.
    /// </summary>
    /// <typeparam name="T">The type related to the parser.</typeparam>
    private sealed class BaseTag<T> : ITag<T>
    {
        /// <summary>
        /// Action linked to the tag.
        /// </summary>
        private readonly Func<T, string> _resolve;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTag{T}"/> class.
        /// </summary>
        /// <param name="name">The description of the tag.</param>
        /// <param name="description">The name of the tag.</param>
        /// <param name="resolveAction">The action linked to the tag.</param>
        public BaseTag(string name, string description, Func<T, string> resolveAction)
        {
            _resolve = resolveAction;

            Description = description;
            Name = name;
        }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Resolve(T value)
        {
            return _resolve(value);
        }
    }
}
