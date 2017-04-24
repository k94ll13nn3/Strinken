using System;

namespace Strinken.Common
{
    /// <summary>
    /// Collection of common extensions methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Tests if a <see cref="char"/> is an invalid token name character i.e. not a-z, A-Z, 0-9, - or _
        /// </summary>
        /// <param name="c">The <see cref="char"/> to test.</param>
        /// <returns>A value indicating whether the <see cref="char"/> is an invalid token name character</returns>
        public static bool IsInvalidTokenNameCharacter(this char c) => !char.IsLetter(c) && c != '-' && c != '_';

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

            for (var i = 0; i < name.Length; i++)
            {
                if (name[i].IsInvalidTokenNameCharacter())
                {
                    throw new ArgumentException($"{name[i]} is an invalid character for a name.");
                }
            }
        }
    }
}