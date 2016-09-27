// stylecop.header
using System;
using System.Collections;
using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Action dictionary used by the token stack to resolve the string.
    /// </summary>
    internal class ActionDictionary
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
    }
}