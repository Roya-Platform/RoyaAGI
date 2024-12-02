using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoyaAi.Layers.Presentation.Services.Interfaces;

namespace RoyaAi.Layers.Presentation.Services
{
    /// <summary>
    /// Implementation of the Step service that manages task steps
    /// </summary>
    public class StepService : IStepService
    {
        // Dictionary structure: agentId -> taskId -> stepId -> step
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, object>>> _steps = new();

        /// <summary>
        /// Lists all steps for a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Collection of steps</returns>
        public Task<IEnumerable<object>> ListStepsAsync(string agentId, string taskId)
        {
            if (!_steps.TryGetValue(agentId, out var agentTasks) ||
                !agentTasks.TryGetValue(taskId, out var taskSteps))
            {
                return Task.FromResult<IEnumerable<object>>(new List<object>());
            }

            return Task.FromResult<IEnumerable<object>>(taskSteps.Values);
        }

        /// <summary>
        /// Gets detailed information about a specific step
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="stepId">The unique identifier of the step</param>
        /// <returns>Detailed step information</returns>
        public Task<object> GetStepAsync(string agentId, string taskId, string stepId)
        {
            if (!_steps.TryGetValue(agentId, out var agentTasks) ||
                !agentTasks.TryGetValue(taskId, out var taskSteps) ||
                !taskSteps.TryGetValue(stepId, out var step))
            {
                throw new KeyNotFoundException($"Step with ID {stepId} not found for task {taskId} and agent {agentId}");
            }

            return Task.FromResult(step);
        }

        /// <summary>
        /// Executes a step in a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="stepId">The unique identifier of the step</param>
        /// <param name="executeRequest">The request data for step execution</param>
        /// <returns>The step execution result</returns>
        public Task<object> ExecuteStepAsync(string agentId, string taskId, string stepId, object executeRequest)
        {
            if (!_steps.ContainsKey(agentId))
            {
                _steps[agentId] = new Dictionary<string, Dictionary<string, object>>();
            }

            if (!_steps[agentId].ContainsKey(taskId))
            {
                _steps[agentId][taskId] = new Dictionary<string, object>();
            }

            // Get or create the step
            if (!_steps[agentId][taskId].TryGetValue(stepId, out var existingStep))
            {
                existingStep = new
                {
                    id = stepId,
                    taskId,
                    agentId,
                    createdAt = DateTime.UtcNow,
                    status = "created"
                };
                _steps[agentId][taskId][stepId] = existingStep;
            }

            // In a real implementation, we would execute the step based on the request
            var executedStep = new
            {
                id = stepId,
                taskId,
                agentId,
                executedAt = DateTime.UtcNow,
                status = "completed",
                input = executeRequest,
                output = new { result = "Step executed successfully", timestamp = DateTime.UtcNow }
            };

            _steps[agentId][taskId][stepId] = executedStep;
            return Task.FromResult<object>(executedStep);
        }
    }
}

