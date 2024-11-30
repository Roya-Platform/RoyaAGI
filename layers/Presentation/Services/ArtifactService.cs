using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RoyaAi.Layers.Presentation.Services.Interfaces;

namespace RoyaAi.Layers.Presentation.Services
{
    /// <summary>
    /// Implementation of the Artifact service that manages task artifacts
    /// </summary>
    public class ArtifactService : IArtifactService
    {
        // Dictionary structure: agentId -> taskId -> artifactId -> artifact
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _artifacts = new();
        // Dictionary to store artifact contents: artifactId -> content
        private readonly Dictionary<string, byte[]> _artifactContents = new();

        /// <summary>
        /// Lists all artifacts for a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Collection of artifacts</returns>
        public Task<IEnumerable<object>> ListArtifactsAsync(string agentId, string taskId)
        {
            if (!_artifacts.TryGetValue(agentId, out var agentTasks) ||
                !agentTasks.TryGetValue(taskId, out var taskArtifacts))
            {
                return Task.FromResult<IEnumerable<object>>(new List<object>());
            }

            return Task.FromResult<IEnumerable<object>>(taskArtifacts.Values);
        }

        /// <summary>
        /// Creates a new artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactData">The artifact data to create</param>
        /// <returns>The created artifact information</returns>
        public Task<object> CreateArtifactAsync(string agentId, string taskId, object artifactData)
        {
            if (!_artifacts.ContainsKey(agentId))
            {
                _artifacts[agentId] = new Dictionary<string, Dictionary<string, object>>();
            }

            if (!_artifacts[agentId].ContainsKey(taskId))
            {
                _artifacts[agentId][taskId] = new Dictionary<string, object>();
            }

            // Generate a unique ID for the artifact
            string artifactId = Guid.NewGuid().ToString();

            // Store the artifact data
            _artifacts[agentId][taskId][artifactId] = artifactData;

            // If the artifact contains content, store it separately
            if (artifactData is Dictionary<string, object> artifactDict && 
                artifactDict.TryGetValue("content", out var content) && 
                content is byte[] contentBytes)
            {
                _artifactContents[artifactId] = contentBytes;
            }

            return Task.FromResult(artifactData);
        }

        /// <summary>
        /// Gets detailed information about a specific artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactId">The unique identifier of the artifact</param>
        /// <returns>Detailed artifact information</returns>
        public Task<object> GetArtifactAsync(string agentId, string taskId, string artifactId)
        {
            if (!_artifacts.TryGetValue(agentId, out var agentTasks) ||
                !agentTasks.TryGetValue(taskId, out var taskArtifacts) ||
                !taskArtifacts.TryGetValue(artifactId, out var artifact))
            {
                throw new KeyNotFoundException($"Artifact with ID {artifactId} not found for task {taskId} and agent {agentId}");
            }

            return Task.FromResult(artifact);
        }

        /// <summary>
        /// Downloads the content of an artifact
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="artifactId">The unique identifier of the artifact</param>
        /// <returns>Stream containing the artifact content</returns>
        public Task<Stream> DownloadArtifactAsync(string agentId, string taskId, string artifactId)
        {
            if (!_artifactContents.TryGetValue(artifactId, out var content))
            {
                throw new KeyNotFoundException($"Content for artifact with ID {artifactId} not found");
            }

            return Task.FromResult<Stream>(new MemoryStream(content));
        }
    }
}
