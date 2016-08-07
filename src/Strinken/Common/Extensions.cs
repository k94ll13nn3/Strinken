// stylecop.header
namespace Strinken.Common
{
    /// <summary>
    /// Collection of common extensions methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Tests if a <see cref="char"/> is a valid token name character : a-z, A-Z, 0-9, - and _
        /// </summary>
        /// <param name="c">The <see cref="char"/> to test.</param>
        /// <returns>A value indicating whether the <see cref="char"/> is a valid token name character</returns>
        public static bool IsValidTokenNameCharacter(this char c) => !char.IsLetter(c) && c != '-' && c != '_';
    }
}