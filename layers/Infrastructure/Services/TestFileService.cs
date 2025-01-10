using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text;

namespace AgentIO.Tests.Services
{
    /// <summary>
    /// Test implementation of the file service.
    /// </summary>
    public class TestFileService : IFileService
    {
        private readonly Dictionary<string, Dictionary<string, object>> _virtualFileSystem;
        private readonly TestDirectoryService _directoryService;

        public TestFileService(TestDirectoryService directoryService)
        {
            _directoryService = directoryService;
            _virtualFileSystem = GetVirtualFileSystemReference();
        }

        private Dictionary<string, Dictionary<string, object>> GetVirtualFileSystemReference()
        {
            // Access the private _virtualFileSystem field from the TestDirectoryService instance
            // This would normally be done with reflection in a real scenario
            // For this example, we'll assume we can access the same instance
            return new Dictionary<string, Dictionary<string, object>>
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

        public async Task<bool> CreateFileAsync(string filePath, string content, bool createDirectories)
        {
            string[] parts = filePath.Split('/');
            string fileName = parts[parts.Length - 1];
            string directory = string.Join("/", parts.Take(parts.Length - 1));

            try
            {
                // Check if directory exists, create if necessary
                bool directoryExists = await _directoryService.DirectoryExistsAsync(directory);
                if (!directoryExists)
                {
                    if (createDirectories)
                    {
                        bool created = await _directoryService.CreateDirectoryAsync(directory);
                        if (!created)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        throw new DirectoryNotFoundException($"Directory not found: {directory}");
                    }
                }

                // Navigate to the directory
                var current = _virtualFileSystem;
                var currentDir = NavigateToDirectory(directory);

                // Create or update the file
                currentDir[fileName] = content;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            // Implementation of DeleteFileAsync method
            throw new NotImplementedException();
        }
    }
} 