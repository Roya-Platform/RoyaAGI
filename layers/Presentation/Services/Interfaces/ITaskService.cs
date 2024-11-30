using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoyaAi.Layers.Presentation.Services.Interfaces
{
    /// <summary>
    /// Interface for Task management services according to the Agent Protocol
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Lists all tasks for an agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <returns>Collection of tasks</returns>
        Task<IEnumerable<object>> ListTasksAsync(string agentId);

        /// <summary>
        /// Creates a new task for an agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="createRequest">Task creation request data</param>
        /// <returns>The created task information</returns>
        Task<object> CreateTaskAsync(string agentId, object createRequest);

        /// <summary>
        /// Gets detailed information about a specific task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Detailed task information</returns>
        Task<object> GetTaskAsync(string agentId, string taskId);

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="updateRequest">Task update request data</param>
        /// <returns>The updated task information</returns>
        Task<object> UpdateTaskAsync(string agentId, string taskId, object updateRequest);

        /// <summary>
        /// Cancels a running task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>The updated task information after cancellation</returns>
        Task<object> CancelTaskAsync(string agentId, string taskId);
    }
}

