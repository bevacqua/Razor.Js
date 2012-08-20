using System;

namespace Bevaq.RazorJs
{
    /// <summary>
    /// RazorJs engine exception
    /// </summary>
    public sealed class RazorJsException : Exception
    {
        private const string DEFAULT_MESSAGE = "Error rendering RazorJs template";

        /// <summary>
        /// Initializes a new instance of the RazorJsException class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        internal RazorJsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RazorJsException class with the specified error message.
        /// </summary>
        /// <param name="exception">The exception that is the cause of the current exception.</param>
        internal RazorJsException(Exception exception)
            : base(DEFAULT_MESSAGE, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RazorJsException class with the specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="exception">The exception that is the cause of the current exception.</param>
        internal RazorJsException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}