// stylecop.header
using System;
using System.Collections.Generic;

namespace Strinken.Engine
{
    /// <summary>
    /// Action dictionary used by the token stack to resolve the string.
    /// </summary>
    internal class ActionDictionary
    {
        /// <summary>
        /// Internal dictionary containing the list of actions and the related <see cref="TokenType"/> and <see cref="TokenSubtype"/>.
        /// </summary>
        private readonly IDictionary<TokenType, IDictionary<TokenSubtype, Func<string[], string>>> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDictionary"/> class.
        /// </summary>
        public ActionDictionary()
        {
            this.items = new Dictionary<TokenType, IDictionary<TokenSubtype, Func<string[], string>>>();
            foreach (TokenType type in Enum.GetValues(typeof(TokenType)))
            {
                this.items[type] = new Dictionary<TokenSubtype, Func<string[], string>>();
            }

            // For an base argument, the base action is to return the first element of the arguments list.
            this.items[TokenType.Argument][TokenSubtype.Base] = a => a[0];
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="type">The type part of the key of the element to get or set.</param>
        /// <param name="subtype">The subtype part of the key  of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key is not present.</returns>
        public Func<string[], string> this[TokenType type, TokenSubtype subtype]
        {
            get { return this.items[type].ContainsKey(subtype) ? this.items[type][subtype] : null; }
            set { this.items[type][subtype] = value; }
        }
    }
}