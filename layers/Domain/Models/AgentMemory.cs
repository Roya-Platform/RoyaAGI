using System;
using System.Collections.Generic;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents the agent's memory store for conversation history and state
    /// </summary>
    public class AgentMemory
    {
        public Guid Id { get; private set; }
        public string AgentId { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public AgentMemory(string agentId, string key, string value)
        {
            Id = Guid.NewGuid();
            AgentId = agentId;
            Key = key;
            Value = value;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateValue(string newValue)
        {
            Value = newValue;
            UpdatedAt = DateTime.UtcNow;
        }
    }
} 