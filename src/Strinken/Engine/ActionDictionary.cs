﻿// stylecop.header
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
        private readonly IDictionary<int, Func<string[], string>> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionDictionary"/> class.
        /// </summary>
        public ActionDictionary()
        {
            items = new Dictionary<int, Func<string[], string>>
            {
                // For a base argument, the base action is to return the first element of the arguments list.
                [GetKey(TokenType.Argument, TokenSubtype.Base)] = a => a[0]
            };
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="type">The type part of the key of the element to get or set.</param>
        /// <param name="subtype">The subtype part of the key  of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key is not present.</returns>
        public Func<string[], string> this[TokenType type, TokenSubtype subtype]
        {
            get { return Get(type, subtype); }
            set { items[GetKey(type, subtype)] = value; }
        }

        /// <summary>
        /// Gets a value in the <see cref="ActionDictionary"/>.
        /// </summary>
        /// <param name="type">The type part of the key of the element to get or set.</param>
        /// <param name="subtype">The subtype part of the key  of the element to get or set.</param>
        /// <returns>The element with the specified key, or null if the key is not present.</returns>
        private Func<string[], string> Get(TokenType type, TokenSubtype subtype)
        {
            var key = GetKey(type, subtype);
            return items.ContainsKey(key) ? items[key] : null;
        }

        /// <summary>
        /// Gets the key corresponding to the pair of <see cref="TokenType"/> and <see cref="TokenSubtype"/>.
        /// </summary>
        /// <param name="type">The type part of the key of the element to get or set.</param>
        /// <param name="subtype">The subtype part of the key  of the element to get or set.</param>
        /// <returns>The corresponding key.</returns>
        private static int GetKey(TokenType type, TokenSubtype subtype)
        {
            switch (type)
            {
                case TokenType.Tag when subtype == TokenSubtype.Base:
                case TokenType.Argument when subtype == TokenSubtype.Tag:
                    return 0x1;

                case TokenType.Tag when subtype == TokenSubtype.ParameterTag:
                case TokenType.Argument when subtype == TokenSubtype.ParameterTag:
                    return 0x2;

                case TokenType.Filter:
                    return 0x4;

                case TokenType.Argument:
                    return 0x8;

                case TokenType.None:
                    return 0x0;
            }

            throw new ArgumentException($"({type}, {subtype}) is not a valid pair.");
        }
    }
}