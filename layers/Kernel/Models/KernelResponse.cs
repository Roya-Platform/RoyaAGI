namespace AgentIO.Models
{
    /// <summary>
    /// Represents a response from the kernel to an agent request.
    /// Contains operation results or error information.
    /// </summary>
    public class KernelResponse
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Indicates whether the response contains an error.
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// The error message, if an error occurred.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The content returned by the operation, such as file content or directory structure.
        /// </summary>
        public string Content { get; set; }
    }
} 