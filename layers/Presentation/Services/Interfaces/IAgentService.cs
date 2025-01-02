using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoyaAi.Layers.Presentation.Services.Interfaces
{
    /// <summary>
    /// Interface for Agent management services according to the Agent Protocol
    /// </summary>
    public interface IAgentService
    {
        /// <summary>
        /// Gets a list of all available agents
        /// </summary>
        /// <returns>Collection of agent information</returns>
        Task<IEnumerable<object>> GetAgentsAsync();

        /// <summary>
        /// Gets detailed information about a specific agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <returns>Detailed agent information</returns>
        Task<object> GetAgentAsync(string agentId);

        /// <summary>
        /// Creates a new agent instance
        /// </summary>
        /// <param name="createRequest">The agent creation request data</param>
        /// <returns>Information about the created agent</returns>
        Task<object> CreateAgentAsync(object createRequest);

        /// <summary>
        /// Updates an existing agent's configuration
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="updateRequest">The agent update request data</param>
        /// <returns>Updated agent information</returns>
        Task<object> UpdateAgentAsync(string agentId, object updateRequest);

        /// <summary>
        /// Deletes an agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent to delete</param>
        /// <returns>Status of the delete operation</returns>
        Task<bool> DeleteAgentAsync(string agentId);
    }
}

