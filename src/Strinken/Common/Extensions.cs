// stylecop.header
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
    }
}