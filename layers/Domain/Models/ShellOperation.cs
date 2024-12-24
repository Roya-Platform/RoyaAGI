using System;

namespace RoyaAI.Layers.Domain.Models
{
    /// <summary>
    /// Represents shell commands executed by the agent
    /// </summary>
    public class ShellOperation
    {
        public Guid Id { get; private set; }
        public string Command { get; private set; }
        public string WorkingDirectory { get; private set; }
        public string Output { get; private set; }
        public int? ExitCode { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public bool IsSuccessful { get; private set; }

        public ShellOperation(string command, string workingDirectory)
        {
            Id = Guid.NewGuid();
            Command = command;
            WorkingDirectory = workingDirectory;
            StartTime = DateTime.UtcNow;
        }

        public void Complete(string output, int exitCode)
        {
            Output = output;
            ExitCode = exitCode;
            EndTime = DateTime.UtcNow;
            IsSuccessful = exitCode == 0;
        }

        public void Fail(string error)
        {
            Output = error;
            EndTime = DateTime.UtcNow;
            IsSuccessful = false;
        }
    }
} 