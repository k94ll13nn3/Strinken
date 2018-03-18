using System;
using System.Collections.Generic;

namespace Strinken.Core
{
    /// <summary>
    /// Action dictionary used by the token stack to resolve the string.
    /// </summary>
    internal class ActionDictionary
    {
        /// <summary>
        /// Internal dictionary containing the list of actions and the related token propeties.
        /// </summary>
        private readonly IDictionary<TokenProperties, Func<string[], string>> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDictionary"/> class.
        /// </summary>
        public ActionDictionary()
        {
            items = new Dictionary<TokenProperties, Func<string[], string>>();
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
            get { return Get(type, operatorSymbol, indicatorSymbol); }
            set { items[new TokenProperties(type, operatorSymbol, indicatorSymbol)] = value; }
        }

        /// <summary>
        /// Gets a value in the <see cref="ActionDictionary"/>.
        /// </summary>
        /// <param name="type">Ttype part of the key of the element to get or set.</param>
        /// <param name="operatorSymbol">The operator symbol part of the key  of the element to get or set.</param>
        /// <param name="indicatorSymbol">The indicator symbol part of the key  of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key is not present.</returns>
        private Func<string[], string> Get(TokenType type, char operatorSymbol, char indicatorSymbol)
        {
            var key = new TokenProperties(type, operatorSymbol, indicatorSymbol);
            return items.ContainsKey(key) ? items[key] : null;
        }

        /// <summary>
        /// Represents a key in the action dictionary.
        /// </summary>
        private struct TokenProperties
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TokenProperties"/> class.
            /// </summary>
            /// <param name="type">Ttype part of the key of the element to get or set.</param>
            /// <param name="operatorSymbol">The operator symbol part of the key  of the element to get or set.</param>
            /// <param name="indicatorSymbol">The indicator symbol part of the key  of the element to get or set.</param>
            public TokenProperties(TokenType type, char operatorSymbol, char indicatorSymbol)
            {
                TokenType = type;
                OperatorSymbol = operatorSymbol;
                IndicatorSymbol = indicatorSymbol;
            }

            /// <summary>
            /// Gets the type part of the key of the element to get or set.
            /// </summary>
            public TokenType TokenType { get; }

            /// <summary>
            /// Gets the operator symbol part of the key  of the element to get or set.
            /// </summary>
            public char OperatorSymbol { get; }

            /// <summary>
            /// Gets the indicator symbol part of the key  of the element to get or set.
            /// </summary>
            public char IndicatorSymbol { get; }

            /// <inheritdoc/>
            public override bool Equals(object obj)
            {
                if (!(obj is TokenProperties))
                {
                    return false;
                }

                var properties = (TokenProperties)obj;
                return TokenType == properties.TokenType &&
                       OperatorSymbol == properties.OperatorSymbol &&
                       IndicatorSymbol == properties.IndicatorSymbol;
            }

            /// <inheritdoc/>
            public override int GetHashCode()
            {
                var hashCode = -658720343;
                hashCode = hashCode * -1521134295 + TokenType.GetHashCode();
                hashCode = hashCode * -1521134295 + OperatorSymbol.GetHashCode();
                hashCode = hashCode * -1521134295 + IndicatorSymbol.GetHashCode();
                return hashCode;
            }
        }
    }
}