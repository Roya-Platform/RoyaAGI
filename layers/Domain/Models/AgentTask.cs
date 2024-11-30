using System;
using System.Collections.Generic;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents a task for an agent to perform, following Agent Protocol specification
    /// </summary>
    public class AgentTask
    {
        public Guid Id { get; private set; }
        public string Input { get; private set; }
        public TaskStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        private readonly List<AgentStep> _steps = new();
        private readonly List<Artifact> _artifacts = new();

        public IReadOnlyCollection<AgentStep> Steps => _steps.AsReadOnly();
        public IReadOnlyCollection<Artifact> Artifacts => _artifacts.AsReadOnly();

        // Constructor for new tasks
        public AgentTask(string input)
        {
            Id = Guid.NewGuid();
            Input = input;
            Status = TaskStatus.Created;
            CreatedAt = DateTime.UtcNow;
        }

        // Constructor for recreating tasks from persistence
        private AgentTask(Guid id, string input, TaskStatus status, DateTime createdAt, DateTime? updatedAt, 
            List<AgentStep> steps, List<Artifact> artifacts)
        {
            Id = id;
            Input = input;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            _steps = steps;
            _artifacts = artifacts;
        }

        public static AgentTask Reconstitute(Guid id, string input, TaskStatus status, DateTime createdAt, 
            DateTime? updatedAt, List<AgentStep> steps, List<Artifact> artifacts)
        {
            return new AgentTask(id, input, status, createdAt, updatedAt, steps, artifacts);
        }

        public void Start()
        {
            if (Status != TaskStatus.Created)
                throw new InvalidOperationException("Task can only be started when in Created status");
                
            Status = TaskStatus.Running;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Complete()
        {
            if (Status != TaskStatus.Running)
                throw new InvalidOperationException("Task can only be completed when in Running status");
                
            Status = TaskStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
                throw new InvalidOperationException("Cannot cancel a task that is already completed or cancelled");
                
            Status = TaskStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Fail(string reason)
        {
            if (Status == TaskStatus.Completed || Status == TaskStatus.Cancelled)
                throw new InvalidOperationException("Cannot fail a task that is already completed or cancelled");
                
            Status = TaskStatus.Failed;
            UpdatedAt = DateTime.UtcNow;
        }

        public AgentStep AddStep(string input)
        {
            if (Status != TaskStatus.Running)
                throw new InvalidOperationException("Steps can only be added to a running task");
                
            var step = new AgentStep(this.Id, input);
            _steps.Add(step);
            UpdatedAt = DateTime.UtcNow;
            return step;
        }

        public Artifact AddArtifact(string name, string fileType, byte[] content)
        {
            var artifact = new Artifact(this.Id, name, fileType, content);
            _artifacts.Add(artifact);
            UpdatedAt = DateTime.UtcNow;
            return artifact;
        }
    }

    public enum TaskStatus
    {
        Created,
        Running,
        Completed,
        Failed,
        Cancelled
    }
} 