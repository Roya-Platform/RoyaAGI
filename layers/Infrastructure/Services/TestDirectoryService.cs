using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace AgentIO.Tests.Services
{ 
    /// <summary>
    /// Test implementation of the directory service.
    /// </summary>
    public class TestDirectoryService : IDirectoryService
    {
        private Dictionary<string, Dictionary<string, object>> _virtualFileSystem;

        public TestDirectoryService()
        {
            InitializeVirtualFileSystem();
        }

        private void InitializeVirtualFileSystem()
        {
            // Create a virtual file system for testing
            _virtualFileSystem = new Dictionary<string, Dictionary<string, object>>
            {
                ["C:/Users/Jeff/repo"] = new Dictionary<string, object>
                {
                    ["Project1"] = new Dictionary<string, object>
                    {
                        ["src"] = new Dictionary<string, object>
                        {
                            ["main.cs"] = "// Main code\npublic class Main {\n    // Main method\n    public static void Main() {\n        // Code here\n    }\n}",
                            ["helper.cs"] = "// Helper code"
                        },
                        ["tests"] = new Dictionary<string, object>
                        {
                            ["test.cs"] = "// Test code"
                        },
                        ["docs"] = new Dictionary<string, object>
                        {
                            ["implementation.md"] = "# Implementation Details"
                        },
                        ["README.md"] = "# Project1\n\nThis is a sample project demonstrating the agent file operations.\n\n## Features\n- Feature 1\n- Feature 2\n\n## Getting Started\nSee the documentation for more details."
                    },
                    ["Project2"] = new Dictionary<string, object>
                    {
                        ["docs"] = new Dictionary<string, object>
                        {
                            ["api.md"] = "# API Documentation\n\n## Endpoints\n\n### GET /api/v1/users\nReturns a list of users.\n\n### POST /api/v1/users\nCreates a new user."
                        },
                        ["config.json"] = "{\n  \"name\": \"Project2\",\n  \"version\": \"1.0.0\",\n  \"description\": \"Sample configuration\",\n  \"settings\": {\n    \"timeout\": 30,\n    \"maxRetries\": 3,\n    \"debug\": false\n  }\n}"
                    },
                    ["notes.txt"] = "Some notes"
                }
            };
        }

        public Task<bool> CreateDirectoryAsync(string directory)
        {
            string[] parts = directory.Split('/');
            var current = _virtualFileSystem;

            string currentPath = parts[0];
            if (!current.ContainsKey(currentPath))
            {
                current[currentPath] = new Dictionary<string, object>();
            }

            var currentDir = current[currentPath] as Dictionary<string, object>;

            for (int i = 1; i < parts.Length; i++)
            {
                if (currentDir.ContainsKey(parts[i]))
                {
                    if (currentDir[parts[i]] is Dictionary<string, object>)
                    {
                        currentDir = currentDir[parts[i]] as Dictionary<string, object>;
                    }
                    else
                    {
                        // Path exists but is a file, not a directory
                        return Task.FromResult(false);
                    }
                }
                else
                {
                    currentDir[parts[i]] = new Dictionary<string, object>();
                    currentDir = currentDir[parts[i]] as Dictionary<string, object>;
                }
            }

            return Task.FromResult(true);
        }

        public Task<bool> DirectoryExistsAsync(string directory)
        {
            try
            {
                string[] parts = directory.Split('/');
                var current = _virtualFileSystem;

                if (!current.ContainsKey(parts[0]))
                {
                    return Task.FromResult(false);
                }

                var currentDir = current[parts[0]] as Dictionary<string, object>;

                for (int i = 1; i < parts.Length; i++)
                {
                    if (currentDir.ContainsKey(parts[i]) && currentDir[parts[i]] is Dictionary<string, object>)
                    {
                        currentDir = currentDir[parts[i]] as Dictionary<string, object>;
                    }
                    else
                    {
                        return Task.FromResult(false);
                    }
                }

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<string> GetDirectoryTreeAsync(string directory, int? depth = null)
        {
            try
            {
                string[] parts = directory.Split('/');
                var current = _virtualFileSystem;

                if (!current.ContainsKey(parts[0]))
                {
                    throw new DirectoryNotFoundException($"Directory not found: {directory}");
                }

                var currentDir = current[parts[0]] as Dictionary<string, object>;

                for (int i = 1; i < parts.Length; i++)
                {
                    if (currentDir.ContainsKey(parts[i]) && currentDir[parts[i]] is Dictionary<string, object>)
                    {
                        currentDir = currentDir[parts[i]] as Dictionary<string, object>;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory not found: {directory}");
                    }
                }

                return Task.FromResult(GenerateTreeOutput(directory, currentDir, depth));
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DirectoryNotFoundException($"Error accessing directory: {ex.Message}");
            }
        }

        private string GenerateTreeOutput(string rootPath, Dictionary<string, object> directory, int? depth = null, string prefix = "", bool isLast = true)
        {
            StringBuilder result = new StringBuilder($"{rootPath}/\n");
            int currentDepth = 1;

            var entries = directory.OrderBy(kv => !(kv.Value is Dictionary<string, object>)).ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                bool isLastEntry = i == entries.Count - 1;
                string entryPrefix = isLastEntry ? "└── " : "├── ";

                if (entry.Value is Dictionary<string, object> subDir)
                {
                    if (depth.HasValue && currentDepth >= depth.Value)
                    {
                        result.AppendLine($"{prefix}{entryPrefix}{entry.Key}/");
                    }
                    else
                    {
                        string newPrefix = prefix + (isLastEntry ? "    " : "│   ");
                        string subDirPath = $"{rootPath}/{entry.Key}";

                        result.AppendLine($"{prefix}{entryPrefix}{entry.Key}/");

                        foreach (var subEntry in subDir.OrderBy(kv => !(kv.Value is Dictionary<string, object>)))
                        {
                            bool isLastSubEntry = subEntry.Equals(subDir.OrderBy(kv => !(kv.Value is Dictionary<string, object>)).Last());
                            string subEntryPrefix = isLastSubEntry ? "└── " : "├── ";

                            if (subEntry.Value is Dictionary<string, object>)
                            {
                                result.AppendLine($"{newPrefix}{subEntryPrefix}{subEntry.Key}/");
                            }
                            else
                            {
                                result.AppendLine($"{newPrefix}{subEntryPrefix}{subEntry.Key}");
                            }
                        }
                    }
                }
                else
                {
                    result.AppendLine($"{prefix}{entryPrefix}{entry.Key}");
                }
            }

            return result.ToString().TrimEnd();
        }
    }
} 