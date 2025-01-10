using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentIO.Services
{
    /// <summary>
    /// Interface for directory alias management service that handles resolution
    /// of directory aliases to physical paths. This service simplifies file access
    /// by providing human-friendly directory names.
    /// </summary>
    public interface IAliasService
    {
        /// <summary>
        /// Resolves a path that might contain a directory alias to a physical path.
        /// </summary>
        /// <param name="path">The path that might contain an alias</param>
        /// <returns>The resolved physical path</returns>
        Task<string> ResolvePathAsync(string path);

        /// <summary>
        /// Gets all configured directory aliases.
        /// </summary>
        /// <returns>A dictionary mapping aliases to physical paths</returns>
        Task<Dictionary<string, string>> GetAliasesAsync();

        /// <summary>
        /// Checks if an alias exists.
        /// </summary>
        /// <param name="alias">The alias to check</param>
        /// <returns>True if the alias exists, otherwise false</returns>
        Task<bool> AliasExistsAsync(string alias);
    }
} 