using System;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents an artifact generated during task or step execution
    /// </summary>
    public class Artifact
    {
        public Guid Id { get; private set; }
        public Guid TaskId { get; private set; }
        public Guid? StepId { get; private set; }
        public string Name { get; private set; }
        public string FileType { get; private set; }
        public byte[] Content { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Constructor for task artifact
        public Artifact(Guid taskId, string name, string fileType, byte[] content)
        {
            Id = Guid.NewGuid();
            TaskId = taskId;
            StepId = null;
            Name = name;
            FileType = fileType;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        // Constructor for step artifact
        public Artifact(Guid taskId, string name, string fileType, byte[] content, Guid stepId)
        {
            Id = Guid.NewGuid();
            TaskId = taskId;
            StepId = stepId;
            Name = name;
            FileType = fileType;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        // Constructor for recreating from persistence
        private Artifact(Guid id, Guid taskId, Guid? stepId, string name, string fileType, 
            byte[] content, DateTime createdAt)
        {
            Id = id;
            TaskId = taskId;
            StepId = stepId;
            Name = name;
            FileType = fileType;
            Content = content;
            CreatedAt = createdAt;
        }

        public static Artifact Reconstitute(Guid id, Guid taskId, Guid? stepId, string name, 
            string fileType, byte[] content, DateTime createdAt)
        {
            return new Artifact(id, taskId, stepId, name, fileType, content, createdAt);
        }
    }
} 