using System;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents a human-friendly alias for a directory path
    /// </summary>
    public class DirectoryAlias
    {
        public Guid Id { get; private set; }
        public string Alias { get; private set; }
        public string FullPath { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public DirectoryAlias(string alias, string fullPath)
        {
            Id = Guid.NewGuid();
            Alias = alias;
            FullPath = fullPath;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdatePath(string newPath)
        {
            FullPath = newPath;
        }
    }
} 