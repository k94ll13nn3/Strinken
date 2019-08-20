using System;
using System.Collections.Generic;

namespace Strinken.Core
{
    /// <summary>
    /// Collection of common extensions methods.
    /// </summary>
    internal static class Extensions
    {
        private static readonly List<char> ValidAlternativeNameCharacter = new List<char>("!%&*./<=>@^|~?$#".ToCharArray());

        /// <summary>
        /// Tests if a <see cref="char"/> is an invalid token name character i.e. not a-z, A-Z, 0-9, - or _.
        /// </summary>
        /// <param name="c">The <see cref="char"/> to test.</param>
        /// <returns>A value indicating whether the <see cref="char"/> is an invalid token name character</returns>
        public static bool IsInvalidTokenNameCharacter(this char c) => !char.IsLetter(c) && c != '-' && c != '_';

        /// <summary>
        /// Tests if a <see cref="char"/> is an invalid alternative name character i.e. not in <see cref="ValidAlternativeNameCharacter"/>.
        /// </summary>
        /// <param name="c">The <see cref="char"/> to test.</param>
        /// <returns>A value indicating whether the <see cref="char"/> is an invalid alternative name character</returns>
        public static bool IsInvalidAlternativeNameCharacter(this char c) => !ValidAlternativeNameCharacter.Contains(c);

        /// <summary>
        /// Tests if a <see cref="char"/> is an invalid hexadecimal character i.e. not a-f, A-F or 0-9.
        /// </summary>
        /// <param name="c">The <see cref="char"/> to test.</param>
        /// <returns>A value indicating whether the <see cref="char"/> is an invalid hexadecimal character</returns>
        public static bool IsInvalidHexadecimalCharacter(this char c) => !((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));

        /// <summary>
        /// Validates a name and throws a <see cref="ArgumentException"/> if the name is invalid.
        /// </summary>
        /// <param name="name">The name to validate.</param>
        /// <exception cref="ArgumentException">When the name is invalid.</exception>
        public static void ThrowIfInvalidName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A name cannot be empty.");
            }

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i].IsInvalidTokenNameCharacter())
                {
                    throw new ArgumentException($"{name[i]} is an invalid character for a name.");
                }
            }
        }

        /// <summary>
        /// Validates an alternative name and throws a <see cref="ArgumentException"/> if the alternative name is invalid.
        /// </summary>
        /// <param name="alternativeName">The alternative name to validate.</param>
        /// <exception cref="ArgumentException">When the alternative name is invalid.</exception>
        public static void ThrowIfInvalidAlternativeName(this string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                return;
            }

            for (int i = 0; i < alternativeName.Length; i++)
            {
                if (alternativeName[i].IsInvalidAlternativeNameCharacter())
                {
                    throw new ArgumentException($"{alternativeName[i]} is an invalid character for an alternative name.");
                }
            }
        }
    }
}
