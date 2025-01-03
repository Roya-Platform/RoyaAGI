using System.Threading.Tasks;

namespace AgentIO.Services
{
    /// <summary>
    /// Interface for directory operations service that provides directory structure
    /// and navigation capabilities. This service enables agents to explore and understand
    /// the file system hierarchy in a controlled manner.
    /// </summary>
    public interface IDirectoryService
    {
        /// <summary>
        /// Gets a hierarchical tree representation of a directory.
        /// </summary>
        /// <param name="directory">The directory path</param>
        /// <param name="depth">Optional depth limit</param>
        /// <returns>A formatted tree representation</returns>
        Task<string> GetDirectoryTreeAsync(string directory, int? depth = null);

        /// <summary>
        /// Checks if a directory exists.
        /// </summary>
        /// <param name="directory">The directory path</param>
        /// <returns>True if the directory exists, otherwise false</returns>
        Task<bool> DirectoryExistsAsync(string directory);

        /// <summary>
        /// Creates a directory if it doesn't exist.
        /// </summary>
        /// <param name="directory">The directory path</param>
        /// <returns>True if successful, otherwise false</returns>
        Task<bool> CreateDirectoryAsync(string directory);
    }
} 