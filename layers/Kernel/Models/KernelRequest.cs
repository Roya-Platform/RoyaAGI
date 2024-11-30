using System.Collections.Generic;

namespace AgentIO.Models
{
    /// <summary>
    /// Represents a request from an agent to the kernel.
    /// Contains all necessary information for the kernel to execute the requested operation.
    /// </summary>
    public class KernelRequest
    {
        /// <summary>
        /// The category of the request (e.g., "IO", "Shell").
        /// </summary>
        public string RequestCategory { get; set; }

        /// <summary>
        /// The specific operation type within the category (e.g., "Read", "Update", "Tree").
        /// </summary>
        public string RequestType { get; set; }

        /// <summary>
        /// The path to the file for file operations.
        /// Can include directory aliases that will be resolved by the kernel.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The directory path for directory operations.
        /// Can include directory aliases that will be resolved by the kernel.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The line number for file update operations.
        /// Specifies where in the file the content should be inserted or replaced.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// The content for file creation or update operations.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The depth limit for directory tree operations.
        /// If not specified, all directories will be traversed.
        /// </summary>
        public int? Depth { get; set; }

        /// <summary>
        /// Whether to create parent directories for file creation operations.
        /// </summary>
        public bool CreateDirectories { get; set; }
    }
} 