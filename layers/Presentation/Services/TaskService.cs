using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoyaAi.Layers.Presentation.Services.Interfaces;

namespace RoyaAi.Layers.Presentation.Services
{
    /// <summary>
    /// Implementation of the Task service that manages agent tasks
    /// </summary>
    public class TaskService : ITaskService
    {
        // Dictionary structure: agentId -> taskId -> task
        private readonly Dictionary<string, Dictionary<string, object>> _tasks = new();

        /// <summary>
        /// Lists all tasks for an agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <returns>Collection of tasks</returns>
        public Task<IEnumerable<object>> ListTasksAsync(string agentId)
        {
            if (!_tasks.TryGetValue(agentId, out var agentTasks))
            {
                return Task.FromResult<IEnumerable<object>>(new List<object>());
            }

            return Task.FromResult<IEnumerable<object>>(agentTasks.Values);
        }

        /// <summary>
        /// Creates a new task for an agent
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="createRequest">Task creation request data</param>
        /// <returns>The created task information</returns>
        public Task<object> CreateTaskAsync(string agentId, object createRequest)
        {
            if (!_tasks.ContainsKey(agentId))
            {
                _tasks[agentId] = new Dictionary<string, object>();
            }

            var taskId = Guid.NewGuid().ToString();
            var task = new
            {
                id = taskId,
                agentId,
                createdAt = DateTime.UtcNow,
                status = "pending",
                input = createRequest
            };

            _tasks[agentId][taskId] = task;
            return Task.FromResult<object>(task);
        }

        /// <summary>
        /// Gets detailed information about a specific task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Detailed task information</returns>
        public Task<object> GetTaskAsync(string agentId, string taskId)
        {
            if (!_tasks.TryGetValue(agentId, out var agentTasks) || !agentTasks.TryGetValue(taskId, out var task))
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found for agent {agentId}");
            }

            return Task.FromResult(task);
        }

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="updateRequest">Task update request data</param>
        /// <returns>The updated task information</returns>
        public Task<object> UpdateTaskAsync(string agentId, string taskId, object updateRequest)
        {
            if (!_tasks.TryGetValue(agentId, out var agentTasks) || !agentTasks.TryGetValue(taskId, out var existingTask))
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found for agent {agentId}");
            }

            // In a real implementation, we would update the existing task
            var updatedTask = new
            {
                id = taskId,
                agentId,
                updatedAt = DateTime.UtcNow,
                status = "updated",
                input = updateRequest
            };

            agentTasks[taskId] = updatedTask;
            return Task.FromResult<object>(updatedTask);
        }

        /// <summary>
        /// Cancels a running task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>The updated task information after cancellation</returns>
        public Task<object> CancelTaskAsync(string agentId, string taskId)
        {
            if (!_tasks.TryGetValue(agentId, out var agentTasks) || !agentTasks.TryGetValue(taskId, out var existingTask))
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found for agent {agentId}");
            }

            // In a real implementation, we would cancel the task and update its status
            var cancelledTask = new
            {
                id = taskId,
                agentId,
                cancelledAt = DateTime.UtcNow,
                status = "cancelled"
            };

            agentTasks[taskId] = cancelledTask;
            return Task.FromResult<object>(cancelledTask);
        }
    }
}

