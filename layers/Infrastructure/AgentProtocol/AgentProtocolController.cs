using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoyaAi.Layers.Kernel.Interfaces;
using RoyaAi.Layers.Infrastructure.AgentProtocol.Models;
using System.ComponentModel.DataAnnotations;

namespace RoyaAi.Layers.Infrastructure.AgentProtocol
{
    [ApiController]
    [Route("/agent")]
    public class AgentProtocolController : ControllerBase
    {
        private readonly IKernelService _kernelService;
        private readonly ITaskService _taskService;
        private readonly IStepService _stepService;
        private readonly IWebhookService _webhookService;

        public AgentProtocolController(
            IKernelService kernelService,
            ITaskService taskService,
            IStepService stepService,
            IWebhookService webhookService)
        {
            _kernelService = kernelService ?? throw new ArgumentNullException(nameof(kernelService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
            _stepService = stepService ?? throw new ArgumentNullException(nameof(stepService));
            _webhookService = webhookService ?? throw new ArgumentNullException(nameof(webhookService));
        }

        // Agent Status Endpoint
        [HttpGet]
        public ActionResult<AgentStatusResponse> GetAgentStatus()
        {
            var status = new AgentStatusResponse
            {
                Version = "1.0.0",
                ModelName = "RoyaAI Agent",
                Description = "Advanced AGI agent with secure kernel integration",
                AgentType = "system-interaction",
                AvailableActions = new List<string> { "file_operations", "system_monitoring", "shell_commands" }
            };

            return Ok(status);
        }

        // Task Management Endpoints
        [HttpPost("tasks")]
        public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] TaskRequest request)
        {
            var task = await _taskService.CreateTaskAsync(request);
            return Ok(task);
        }

        [HttpGet("tasks/{taskId}")]
        public async Task<ActionResult<TaskResponse>> GetTask(string taskId)
        {
            var task = await _taskService.GetTaskAsync(taskId);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet("tasks/{taskId}/steps")]
        public async Task<ActionResult<StepListResponse>> ListTaskSteps(string taskId)
        {
            var steps = await _stepService.ListTaskStepsAsync(taskId);
            return Ok(new StepListResponse { Steps = steps });
        }

        // Step Management Endpoints
        [HttpPost("tasks/{taskId}/steps")]
        public async Task<ActionResult<StepResponse>> CreateStep(string taskId, [FromBody] StepRequest request)
        {
            var step = await _stepService.CreateStepAsync(taskId, request);
            return Ok(step);
        }

        [HttpGet("tasks/{taskId}/steps/{stepId}")]
        public async Task<ActionResult<StepResponse>> GetStep(string taskId, string stepId)
        {
            var step = await _stepService.GetStepAsync(taskId, stepId);
            if (step == null)
                return NotFound();

            return Ok(step);
        }

        [HttpPost("tasks/{taskId}/steps/{stepId}")]
        public async Task<ActionResult<StepResponse>> UpdateStep(string taskId, string stepId, [FromBody] StepUpdateRequest request)
        {
            var step = await _stepService.UpdateStepAsync(taskId, stepId, request);
            if (step == null)
                return NotFound();

            return Ok(step);
        }

        // Artifact Management Endpoints
        [HttpPost("tasks/{taskId}/artifacts")]
        public async Task<ActionResult<ArtifactResponse>> CreateArtifact(string taskId, [FromBody] ArtifactRequest request)
        {
            var artifact = await _taskService.CreateArtifactAsync(taskId, request);
            return Ok(artifact);
        }

        [HttpGet("tasks/{taskId}/artifacts")]
        public async Task<ActionResult<ArtifactListResponse>> ListArtifacts(string taskId)
        {
            var artifacts = await _taskService.ListArtifactsAsync(taskId);
            return Ok(new ArtifactListResponse { Artifacts = artifacts });
        }

        [HttpGet("tasks/{taskId}/artifacts/{artifactId}")]
        public async Task<ActionResult<ArtifactResponse>> GetArtifact(string taskId, string artifactId)
        {
            var artifact = await _taskService.GetArtifactAsync(taskId, artifactId);
            if (artifact == null)
                return NotFound();

            return Ok(artifact);
        }

        // Webhook Management Endpoints
        [HttpPost("webhooks")]
        public async Task<ActionResult<WebhookResponse>> RegisterWebhook([FromBody] WebhookRequest request)
        {
            var webhook = await _webhookService.RegisterWebhookAsync(request);
            return Ok(webhook);
        }

        [HttpDelete("webhooks/{webhookId}")]
        public async Task<ActionResult> UnregisterWebhook(string webhookId)
        {
            await _webhookService.UnregisterWebhookAsync(webhookId);
            return NoContent();
        }
    }
} 