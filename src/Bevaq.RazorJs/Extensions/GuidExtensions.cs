using System;

namespace Bevaq.RazorJs
{
    /// <summary>
    /// Internal Guid extension methods.
    /// </summary>
    internal static class GuidExtensions
    {
        /// <summary>
        /// Turns a Guid into its string representation, removing hyphens and turning it lowercase.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <returns>The string result representing the provided Guid.</returns>
        public static string Stringify(this Guid guid)
        {
            return guid.ToString().Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}