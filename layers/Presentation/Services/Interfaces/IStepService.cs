using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoyaAi.Layers.Presentation.Services.Interfaces
{
    /// <summary>
    /// Interface for Step management services according to the Agent Protocol
    /// </summary>
    public interface IStepService
    {
        /// <summary>
        /// Lists all steps for a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <returns>Collection of steps</returns>
        Task<IEnumerable<object>> ListStepsAsync(string agentId, string taskId);

        /// <summary>
        /// Gets detailed information about a specific step
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="stepId">The unique identifier of the step</param>
        /// <returns>Detailed step information</returns>
        Task<object> GetStepAsync(string agentId, string taskId, string stepId);

        /// <summary>
        /// Executes a step in a task
        /// </summary>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="taskId">The unique identifier of the task</param>
        /// <param name="stepId">The unique identifier of the step</param>
        /// <param name="executeRequest">The request data for step execution</param>
        /// <returns>The step execution result</returns>
        Task<object> ExecuteStepAsync(string agentId, string taskId, string stepId, object executeRequest);
    }
}

