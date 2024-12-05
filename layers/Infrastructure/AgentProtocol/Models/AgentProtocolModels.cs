using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RoyaAi.Layers.Infrastructure.AgentProtocol.Models
{
    // Agent Status Models
    public class AgentStatusResponse
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("model_name")]
        public string ModelName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("agent_type")]
        public string AgentType { get; set; }

        [JsonPropertyName("available_actions")]
        public List<string> AvailableActions { get; set; }
    }

    // Task Models
    public class TaskRequest
    {
        [Required]
        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("additional_input")]
        public Dictionary<string, object> AdditionalInput { get; set; }
    }

    public class TaskResponse
    {
        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("additional_input")]
        public Dictionary<string, object> AdditionalInput { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    // Step Models
    public class StepRequest
    {
        [Required]
        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("additional_input")]
        public Dictionary<string, object> AdditionalInput { get; set; }
    }

    public class StepResponse
    {
        [JsonPropertyName("step_id")]
        public string StepId { get; set; }

        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        [JsonPropertyName("input")]
        public string Input { get; set; }

        [JsonPropertyName("additional_input")]
        public Dictionary<string, object> AdditionalInput { get; set; }

        [JsonPropertyName("output")]
        public string Output { get; set; }

        [JsonPropertyName("additional_output")]
        public Dictionary<string, object> AdditionalOutput { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class StepUpdateRequest
    {
        [JsonPropertyName("output")]
        public string Output { get; set; }

        [JsonPropertyName("additional_output")]
        public Dictionary<string, object> AdditionalOutput { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class StepListResponse
    {
        [JsonPropertyName("steps")]
        public List<StepResponse> Steps { get; set; }
    }

    // Artifact Models
    public class ArtifactRequest
    {
        [Required]
        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("relative_path")]
        public string RelativePath { get; set; }

        [Required]
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

    public class ArtifactResponse
    {
        [JsonPropertyName("artifact_id")]
        public string ArtifactId { get; set; }

        [JsonPropertyName("task_id")]
        public string TaskId { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("relative_path")]
        public string RelativePath { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class ArtifactListResponse
    {
        [JsonPropertyName("artifacts")]
        public List<ArtifactResponse> Artifacts { get; set; }
    }

    // Webhook Models
    public class WebhookRequest
    {
        [Required]
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [Required]
        [JsonPropertyName("events")]
        public List<string> Events { get; set; }
    }

    public class WebhookResponse
    {
        [JsonPropertyName("webhook_id")]
        public string WebhookId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("events")]
        public List<string> Events { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
} 