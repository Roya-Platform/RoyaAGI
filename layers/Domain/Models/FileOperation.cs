using System;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents file operations that can be performed by the agent
    /// </summary>
    public class FileOperation
    {
        public Guid Id { get; private set; }
        public string Path { get; private set; }
        public OperationType Type { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool IsSuccessful { get; private set; }
        public string ErrorMessage { get; private set; }

        public FileOperation(string path, OperationType type, string content = null)
        {
            Id = Guid.NewGuid();
            Path = path;
            Type = type;
            Content = content;
            Timestamp = DateTime.UtcNow;
            IsSuccessful = false;
        }

        public void MarkAsSuccessful()
        {
            IsSuccessful = true;
        }

        public void MarkAsFailed(string errorMessage)
        {
            IsSuccessful = false;
            ErrorMessage = errorMessage;
        }
    }

    public enum OperationType
    {
        Read,
        Write,
        Delete,
        Create,
        Update,
        List
    }
} 