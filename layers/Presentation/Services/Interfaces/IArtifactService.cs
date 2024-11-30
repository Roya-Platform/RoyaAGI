using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RoyaAi.Layers.Presentation.Services.Interfaces
{
    /// <summary>
    /// Interface for Artifact management services according to the Agent Protocol
    /// </summary>
    public interface IArtifactService
    {
        /// <summary>
        /// Lists all artifacts for a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Collection of artifacts</returns>
        Task<IEnumerable<object>> ListArtifactsAsync(string agentId, string taskId);

        /// <summary>
        /// Creates a new artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactData">The artifact data to create</param>
        /// <returns>The created artifact information</returns>
        Task<object> CreateArtifactAsync(string agentId, string taskId, object artifactData);

        /// <summary>
        /// Gets detailed information about a specific artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactId">The unique identifier of the artifact</param>
        /// <returns>Detailed artifact information</returns>
        Task<object> GetArtifactAsync(string agentId, string taskId, string artifactId);

        /// <summary>
        /// Downloads the content of an artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactId">The unique identifier of the artifact</param>
        /// <returns>Stream containing the artifact content</returns>
        Task<Stream> DownloadArtifactAsync(string agentId, string taskId, string artifactId);
    }
}

