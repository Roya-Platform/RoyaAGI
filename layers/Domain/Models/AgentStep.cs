using System;
using System.Collections.Generic;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents a step in an agent task execution, following Agent Protocol specification
    /// </summary>
    public class AgentStep
    {
        public Guid Id { get; private set; }
        public Guid TaskId { get; private set; }
        public string Input { get; private set; }
        public string Output { get; private set; }
        public StepStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        private readonly List<Artifact> _artifacts = new();
        
        public IReadOnlyCollection<Artifact> Artifacts => _artifacts.AsReadOnly();

        // Constructor for new steps
        public AgentStep(Guid taskId, string input)
        {
            Id = Guid.NewGuid();
            TaskId = taskId;
            Input = input;
            Status = StepStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }

        // Constructor for recreating steps from persistence
        private AgentStep(Guid id, Guid taskId, string input, string output, StepStatus status, 
            DateTime createdAt, DateTime? updatedAt, List<Artifact> artifacts)
        {
            Id = id;
            TaskId = taskId;
            Input = input;
            Output = output;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            _artifacts = artifacts;
        }

        public static AgentStep Reconstitute(Guid id, Guid taskId, string input, string output, 
            StepStatus status, DateTime createdAt, DateTime? updatedAt, List<Artifact> artifacts)
        {
            return new AgentStep(id, taskId, input, output, status, createdAt, updatedAt, artifacts);
        }

        public void Start()
        {
            if (Status != StepStatus.Created)
                throw new InvalidOperationException("Step can only be started when in Created status");
                
            Status = StepStatus.Running;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete(string output)
        {
            if (Status != StepStatus.Running)
                throw new InvalidOperationException("Step can only be completed when in Running status");
                
            Output = output;
            Status = StepStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == StepStatus.Completed || Status == StepStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel a step that is already completed or cancelled");
                
            Status = StepStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Fail(string error)
        {
            if (Status == StepStatus.Completed || Status == StepStatus.Cancelled)
                throw new InvalidOperationException("Cannot fail a step that is already completed or cancelled");
                
            Output = error;
            Status = StepStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }

        public Artifact AddArtifact(string name, string fileType, byte[] content)
        {
            var artifact = new Artifact(this.TaskId, name, fileType, content, this.Id);
            _artifacts.Add(artifact);
            UpdatedAt = DateTime.UtcNow;
            return artifact;
        }
    }

    public enum StepStatus
    {
        Created,
        Running,
        Completed,
        Failed,
        Cancelled
    }
} 