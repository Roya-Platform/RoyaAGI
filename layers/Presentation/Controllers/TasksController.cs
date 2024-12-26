using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoyaAi.Layers.Presentation.Controllers
{
    /// <summary>
    /// Controller for managing tasks according to the Agent Protocol specification.
    /// </summary>
    [ApiController]
    [Route("v1/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;

        public TasksController(ILogger<TasksController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="request">The task creation request.</param>
        /// <returns>The created task.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
        {
            // TODO: Implement task creation logic
            var taskId = Guid.NewGuid().ToString();
            
            var task = new TaskResponse
            {
                TaskId = taskId,
                Input = request.Input,
                CreatedAt = DateTime.UtcNow,
                Status = "created"
            };
            
            return CreatedAtAction(nameof(GetTask), new { taskId = task.TaskId }, task);
        }

        /// <summary>
        /// Gets a specific task by ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <returns>The task.</returns>
        [HttpGet("{taskId}")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTask(string taskId)
        {
            // TODO: Implement task retrieval logic
            if (string.IsNullOrEmpty(taskId))
            {
                return NotFound();
            }

            var task = new TaskResponse
            {
                TaskId = taskId,
                Input = new Dictionary<string, object>(),
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                Status = "completed",
                Output = new Dictionary<string, object>()
            };

            return Ok(task);
        }

        /// <summary>
        /// Lists all tasks with optional filtering.
        /// </summary>
        /// <returns>A list of tasks.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ListTasksResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListTasks()
        {
            // TODO: Implement task listing logic
            var tasks = new List<TaskResponse>
            {
                new TaskResponse
                {
                    TaskId = Guid.NewGuid().ToString(),
                    Input = new Dictionary<string, object>(),
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    Status = "completed",
                    Output = new Dictionary<string, object>()
                }
            };

            var response = new ListTasksResponse
            {
                Tasks = tasks
            };

            return Ok(response);
        }

        /// <summary>
        /// Cancels a specific task.
        /// </summary>
        /// <param name="taskId">The ID of the task to cancel.</param>
        /// <returns>The updated task.</returns>
        [HttpPost("{taskId}/cancel")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelTask(string taskId)
        {
            // TODO: Implement task cancellation logic
            if (string.IsNullOrEmpty(taskId))
            {
                return NotFound();
            }

            var task = new TaskResponse
            {
                TaskId = taskId,
                Input = new Dictionary<string, object>(),
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                Status = "cancelled"
            };

            return Ok(task);
        }
    }

    // Models required for the Tasks API

    /// <summary>
    /// Represents a request to create a new task.
    /// </summary>
    public class CreateTaskRequest
    {
        /// <summary>
        /// The input for the task, which can be any arbitrary JSON.
        /// </summary>
        [Required]
        [JsonPropertyName("input")]
        public Dictionary<string, object> Input { get; set; }
    }

    /// <summary>
    /// Represents a task response.
    /// </summary>
    public class TaskResponse
    {
        /// <summary>
        /// The unique identifier for the task.
        /// </summary>
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        /// <summary>
        /// The input that was provided when creating the task.
        /// </summary>
        [JsonPropertyName("input")]
        public Dictionary<string, object> Input { get; set; }

        /// <summary>
        /// When the task was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The current status of the task (created, running, completed, failed, cancelled).
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// The output of the task, which can be any arbitrary JSON.
        /// </summary>
        [JsonPropertyName("output")]
        public Dictionary<string, object> Output { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of tasks.
    /// </summary>
    public class ListTasksResponse
    {
        /// <summary>
        /// The list of tasks.
        /// </summary>
        [JsonPropertyName("tasks")]
        public List<TaskResponse> Tasks { get; set; }
    }
}

