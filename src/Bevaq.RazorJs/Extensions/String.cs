namespace Bevaq.RazorJs
{
    /// <summary>
    /// Internal string extension methods.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Syntactic sugar for string.Format
        /// </summary>
        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        /// <summary>
        /// Syntactic sugar for string.IsNullOrEmpty
        /// </summary>
        public static bool NullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
    }
}
