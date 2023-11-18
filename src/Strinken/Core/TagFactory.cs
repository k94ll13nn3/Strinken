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
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseTag{T}"/> class.
    /// </remarks>
    /// <param name="name">The name of the tag.</param>
    /// <param name="description">The description of the tag.</param>
    /// <param name="resolveAction">The action linked to the tag.</param>
    private sealed class BaseTag<T>(string name, string description, Func<T, string> resolveAction) : ITag<T>
    {
        /// <summary>
        /// Action linked to the tag.
        /// </summary>
        private readonly Func<T, string> _resolve = resolveAction;

        /// <inheritdoc/>
        public string Description { get; } = description;

        /// <inheritdoc/>
        public string Name { get; } = name;

        /// <inheritdoc/>
        public string Resolve(T value)
        {
            return _resolve(value);
        }
    }
}
