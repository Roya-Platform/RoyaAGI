using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RoyaAI.Layers.Domain.Models;

namespace RoyaAI.Layers.Domain.Repositories
{
    public interface IAgentTaskRepository
    {
        Task<AgentTask> GetByIdAsync(Guid id);
        Task<IEnumerable<AgentTask>> GetAllAsync();
        Task<AgentTask> AddAsync(AgentTask task);
        Task UpdateAsync(AgentTask task);
        Task<IEnumerable<AgentTask>> GetTasksByStatusAsync(TaskStatus status);
    }
} 