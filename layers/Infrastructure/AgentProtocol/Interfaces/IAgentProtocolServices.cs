using System.Collections.Generic;
using System.Threading.Tasks;
using RoyaAi.Layers.Infrastructure.AgentProtocol.Models;

namespace RoyaAi.Layers.Infrastructure.AgentProtocol.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateTaskAsync(TaskRequest request);
        Task<TaskResponse> GetTaskAsync(string taskId);
        Task<List<ArtifactResponse>> ListArtifactsAsync(string taskId);
        Task<ArtifactResponse> CreateArtifactAsync(string taskId, ArtifactRequest request);
        Task<ArtifactResponse> GetArtifactAsync(string taskId, string artifactId);
    }

    public interface IStepService
    {
        Task<StepResponse> CreateStepAsync(string taskId, StepRequest request);
        Task<StepResponse> GetStepAsync(string taskId, string stepId);
        Task<StepResponse> UpdateStepAsync(string taskId, string stepId, StepUpdateRequest request);
        Task<List<StepResponse>> ListTaskStepsAsync(string taskId);
    }

    public interface IWebhookService
    {
        Task<WebhookResponse> RegisterWebhookAsync(WebhookRequest request);
        Task UnregisterWebhookAsync(string webhookId);
    }
} 