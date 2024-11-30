using System.Threading.Tasks;

namespace AgentIO.Services
{
    /// <summary>
    /// Interface for file operations service that performs CRUD operations on files.
    /// This service handles the low-level file system interactions, providing a secure
    /// and controlled way for agents to interact with the file system.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Reads the entire content of a file.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>The content of the file</returns>
        Task<string> ReadFileAsync(string filePath);

        /// <summary>
        /// Updates a file at a specific line number.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <param name="lineNumber">The line number to update</param>
        /// <param name="content">The new content</param>
        /// <returns>True if successful, otherwise false</returns>
        Task<bool> UpdateFileAsync(string filePath, int lineNumber, string content);

        /// <summary>
        /// Creates a new file with the specified content.
        /// </summary>
        /// <param name="filePath">The path to create the file</param>
        /// <param name="content">The content to write</param>
        /// <param name="createDirectories">Whether to create parent directories</param>
        /// <returns>True if successful, otherwise false</returns>
        Task<bool> CreateFileAsync(string filePath, string content, bool createDirectories);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">The path to the file to delete</param>
        /// <returns>True if successful, otherwise false</returns>
        Task<bool> DeleteFileAsync(string filePath);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filePath">The path to check</param>
        /// <returns>True if the file exists, otherwise false</returns>
        Task<bool> FileExistsAsync(string filePath);
    }
} 