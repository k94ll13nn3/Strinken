// stylecop.header
using System;
using System.Collections;
using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Action dictionary used by the token stack to resolve the string.
    /// </summary>
    /// <remarks>Collection initializers can be used <see href="http://stackoverflow.com/a/2495801/3836163"/>.</remarks>
    internal class ActionDictionary : IEnumerable<KeyValuePair<TokenType, Func<string[], string>>>
    {
        /// <summary>
        /// Internal dictionary containing the list of actions and the related <see cref="TokenType"/>.
        /// </summary>
        private readonly Dictionary<TokenType, Func<string[], string>> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDictionary"/> class.
        /// </summary>
        public ActionDictionary()
        {
            this.items = new Dictionary<TokenType, Func<string[], string>>();
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key is not present.</returns>
        public Func<string[], string> this[TokenType key]
        {
            get { return this.items.ContainsKey(key) ? this.items[key] : null; }
            set { this.items[key] = value; }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="ActionDictionary"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(TokenType key, Func<string[], string> value)
        {
            this.items[key] = value;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TokenType, Func<string[], string>>> GetEnumerator() => this.items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();
    }
}