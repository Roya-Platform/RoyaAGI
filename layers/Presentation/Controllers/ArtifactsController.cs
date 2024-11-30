using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoyaAi.Layers.Presentation.Controllers
{
    /// <summary>
    /// Controller for managing artifacts according to the Agent Protocol specification.
    /// </summary>
    [ApiController]
    [Route("v1/tasks/{taskId}/artifacts")]
    public class ArtifactsController : ControllerBase
    {
        private readonly ILogger<ArtifactsController> _logger;

        public ArtifactsController(ILogger<ArtifactsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Lists all artifacts for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>A list of artifacts for the task.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ListArtifactsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListArtifacts(string taskId)
        {
            try
            {
                _logger.LogInformation("Listing artifacts for task {TaskId}", taskId);

                // TODO: Implement logic to retrieve artifacts for the task
                var artifacts = new List<ArtifactModel>
                {
                    new ArtifactModel
                    {
                        ArtifactId = Guid.NewGuid().ToString(),
                        TaskId = taskId,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                        FileName = "example.txt",
                        FileSize = 1024,
                        MimeType = "text/plain"
                    }
                };

                return Ok(new ListArtifactsResponse
                {
                    Artifacts = artifacts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing artifacts for task {TaskId}", taskId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = $"Failed to list artifacts for task {taskId}",
                        Type = "internal_error"
                    }
                });
            }
        }

        /// <summary>
        /// Gets a specific artifact by ID.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="artifactId">The ID of the artifact to retrieve.</param>
        /// <returns>The artifact.</returns>
        [HttpGet("{artifactId}")]
        [ProducesResponseType(typeof(ArtifactModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetArtifact(string taskId, string artifactId)
        {
            try
            {
                _logger.LogInformation("Getting artifact {ArtifactId} for task {TaskId}", artifactId, taskId);

                // TODO: Implement logic to retrieve a specific artifact
                // This is just mock data for demonstration
                var artifact = new ArtifactModel
                {
                    ArtifactId = artifactId,
                    TaskId = taskId,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                    FileName = "example.txt",
                    FileSize = 1024,
                    MimeType = "text/plain"
                };

                return Ok(artifact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving artifact {ArtifactId} for task {TaskId}", artifactId, taskId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = $"Failed to retrieve artifact {artifactId} for task {taskId}",
                        Type = "internal_error"
                    }
                });
            }
        }

        /// <summary>
        /// Creates a new artifact for a task.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="request">The artifact creation request.</param>
        /// <returns>The created artifact.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ArtifactModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateArtifact(string taskId, [FromBody] CreateArtifactRequest request)
        {
            try
            {
                _logger.LogInformation("Creating artifact for task {TaskId}", taskId);

                // Validate request
                if (string.IsNullOrEmpty(request.FileName))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Error = new ErrorDetails
                        {
                            Message = "File name is required",
                            Type = "invalid_request"
                        }
                    });
                }

                // TODO: Implement logic to create a new artifact
                var artifact = new ArtifactModel
                {
                    ArtifactId = Guid.NewGuid().ToString(),
                    TaskId = taskId,
                    CreatedAt = DateTime.UtcNow,
                    FileName = request.FileName,
                    FileSize = request.FileContent.Length,
                    MimeType = request.MimeType ?? "application/octet-stream"
                };

                return CreatedAtAction(nameof(GetArtifact), new { taskId, artifactId = artifact.ArtifactId }, artifact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating artifact for task {TaskId}", taskId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = $"Failed to create artifact for task {taskId}",
                        Type = "internal_error"
                    }
                });
            }
        }

        /// <summary>
        /// Downloads the content of an artifact.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="artifactId">The ID of the artifact to download.</param>
        /// <returns>The artifact content as a file.</returns>
        [HttpGet("{artifactId}/content")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadArtifact(string taskId, string artifactId)
        {
            try
            {
                _logger.LogInformation("Downloading artifact {ArtifactId} for task {TaskId}", artifactId, taskId);

                // TODO: Implement logic to retrieve artifact content
                // Mock data for demonstration
                var content = "This is an example artifact content.";
                var fileName = "example.txt";
                var mimeType = "text/plain";

                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                return File(bytes, mimeType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading artifact {ArtifactId} for task {TaskId}", artifactId, taskId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = $"Failed to download artifact {artifactId} for task {taskId}",
                        Type = "internal_error"
                    }
                });
            }
        }

        /// <summary>
        /// Deletes an artifact.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="artifactId">The ID of the artifact to delete.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{artifactId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteArtifact(string taskId, string artifactId)
        {
            try
            {
                _logger.LogInformation("Deleting artifact {ArtifactId} for task {TaskId}", artifactId, taskId);

                // TODO: Implement logic to delete the artifact

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting artifact {ArtifactId} for task {TaskId}", artifactId, taskId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse
                {
                    Error = new ErrorDetails
                    {
                        Message = $"Failed to delete artifact {artifactId} for task {taskId}",
                        Type = "internal_error"
                    }
                });
            }
        }
    }

    // Model classes for artifacts
    
    /// <summary>
    /// Represents an artifact in the system.
    /// </summary>
    public class ArtifactModel
    {
        /// <summary>
        /// The unique identifier for the artifact.
        /// </summary>
        [JsonPropertyName("artifact_id")]
        public string ArtifactId { get; set; }

        /// <summary>
        /// The ID of the task this artifact belongs to.
        /// </summary>
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        /// <summary>
        /// When the artifact was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }
    }

    /// <summary>
    /// Represents a request to create a new artifact.
    /// </summary>
    public class CreateArtifactRequest
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        [Required]
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        /// <summary>
        /// The content of the file as a base64 encoded string.
        /// </summary>
        [Required]
        [JsonPropertyName("file_content")]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; }
    }

    /// <summary>
    /// Represents a response containing a list of artifacts.
    /// </summary>
    public class ListArtifactsResponse
    {
        /// <summary>
        /// The list of artifacts.
        /// </summary>
        [JsonPropertyName("artifacts")]
        public List<ArtifactModel> Artifacts { get; set; } = new List<ArtifactModel>();
    }

    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// The error details.
        /// </summary>
        [JsonPropertyName("error")]
        public ErrorDetails Error { get; set; }
    }

    /// <summary>
    /// Represents error details.
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// The type of error.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}

